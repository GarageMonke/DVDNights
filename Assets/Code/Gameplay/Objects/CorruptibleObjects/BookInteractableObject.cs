using CorePatterns.Managers;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Rulebound
{
    public class BookInteractableObject : CorruptibleInteractableObject
    {
        [Header("References")] 
        [SerializeField] private Rigidbody bookRigidbody;
        [SerializeField] private Collider bookCollider;
        
        [Header("Pull-Sequence")]
        [SerializeField] private float pullDistance = 0.25f;
        [SerializeField] private float pullDuration = 0.4f;
        [SerializeField] private float spinForce = 1f;

        [Header("Push-Sequence")] 
        [SerializeField] private Vector3 returnControlPosition;
        
        [Header("Feedback")] 
        [SerializeField] private AudioClip hitGroundAudioClip;
        
        private bool _isInShelf;

        private Vector3 _originPosition;
        private Quaternion _originRotation;
        
        private Sequence _corruptedSequence;
        
        private void Awake()
        {
            bookRigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
            bookRigidbody.interpolation = RigidbodyInterpolation.Interpolate;
            bookRigidbody.isKinematic = true;
            _originPosition = transform.localPosition;
            _originRotation = transform.localRotation;
            _isInShelf = true;
            bookCollider.enabled = false;
        }
         
        public override string GetInteractionAction()
        {
            return "Return book to shelf";
        }

        public override void Interact()
        {
            if (_isCorrupted && !_isInShelf)
            {
                ClearCorruption();
            }
        }

        public override void Corrupt()
        {
            base.Corrupt();
            ThrowBook();
        }

        public override void ClearCorruption()
        {
            base.ClearCorruption();
            Vector3[] pathToControlPoint = CurveGenerator.GetCurvePoints(transform.localPosition, returnControlPosition, returnControlPosition, 5);
            Vector3[] pathToShelf = CurveGenerator.GetCurvePoints(returnControlPosition, _originPosition, _originPosition, 5);
            
            bookRigidbody.isKinematic = true;
            bookCollider.enabled = false;
            DisableInteraction();
            
            _corruptedSequence?.Kill();
            _corruptedSequence = DOTween.Sequence()
            .Append(transform.DOLocalPath(pathToControlPoint, 2f, PathType.CatmullRom).SetEase(Ease.InOutSine)
            .OnWaypointChange(waypointIndex =>
            {
                if (waypointIndex == 1)
                {
                    transform.DOLocalRotate(_originRotation.eulerAngles, 1.5f).SetEase(Ease.OutSine);
                }
            }))
            .Append(transform.DOLocalPath(pathToShelf, 1f, PathType.CatmullRom).SetEase(Ease.InOutSine)
            .OnWaypointChange(waypointIndex =>
            {
                if (waypointIndex == 2)
                {
                    AudioManager.Instance.PlaySFX(AudioChannelType.DOOR, InteractionAudioClip, volume: 0.75f, pitch: 1f);
                }
                
            })).OnComplete(() =>
            {
                _isInShelf = true;
                _isCorrupted = false;
            });
        }

        public override bool CanBeCorrupted()
        {
            return _isInShelf && !_isCorrupted;
        }
        
        [ContextMenu("Throw Book")]
        public void ThrowBook()
        {
            if (!_isInShelf)
            {
                transform.localPosition = _originPosition;
                transform.localRotation = _originRotation;
                _isInShelf = true;
                bookCollider.enabled = false;
                bookRigidbody.isKinematic = false;
                return;
            }

            _isCorrupted = true;
            bookCollider.enabled = true;
            bookRigidbody.isKinematic = true;

            _corruptedSequence?.Kill();

            _corruptedSequence = DOTween.Sequence()
                .Append(transform.DOLocalMoveX(transform.localPosition.x + pullDistance, pullDuration)
                    .SetEase(Ease.InOutQuart)
                    .OnComplete(() =>
                    {
                        bookRigidbody.isKinematic = false;
                        bookRigidbody.AddTorque(
                            Random.insideUnitSphere * spinForce,
                            ForceMode.Impulse);
                    }));
        }

        private void OnCollisionEnter(Collision other)
        {
            if (!_isInShelf)
            {
                return;
            }
            
            if (other.gameObject.CompareTag("Floor"))
            {
                _isInShelf = false;
                EnableInteraction();
                AudioManager.Instance.PlaySFX(AudioChannelType.DOOR, hitGroundAudioClip, 1f, randomizePitch: true);
            }
        }
    }
}