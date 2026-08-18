using CorePatterns.Managers;
using DG.Tweening;
using UnityEngine;

namespace DVDNights
{
    public class ToyInteractableObject : CorruptibleInteractableObject
    {
        [Header("References")] 
        [SerializeField] private Rigidbody toyRigidbody;
        [SerializeField] private Collider toyCollider;
        
        [Header("Pull-Sequence")]
        [SerializeField] private float pullDistance = 0.25f;
        [SerializeField] private float pullDuration = 0.4f;
        [SerializeField] private float spinForce = 1f;

        [Header("Push-Sequence")] 
        [SerializeField] private Vector3 returnControlPosition;
        
        private Vector3 _originPosition;
        private Quaternion _originRotation;
        
        private Sequence _corruptedSequence;
        private bool _isInShelf;

        private void Awake()
        {
            toyRigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
            toyRigidbody.interpolation = RigidbodyInterpolation.Interpolate;
            toyRigidbody.isKinematic = true;
            _originPosition = transform.localPosition;
            _originRotation = transform.localRotation;
            _isInShelf = true;
            toyCollider.enabled = false;
        }
        public override string GetInteractionAction()
        {
            return "Return toy to shelf";
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
            ThrowToy();
        }

        public override void ClearCorruption()
        {
            base.ClearCorruption();
            Vector3[] pathToControlPoint = CurveGenerator.GetCurvePoints(transform.localPosition, returnControlPosition, returnControlPosition, 5);
            Vector3[] pathToShelf = CurveGenerator.GetCurvePoints(returnControlPosition, _originPosition, _originPosition, 5);
            
            toyRigidbody.isKinematic = true;
            toyCollider.enabled = false;
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
                    AudioManager.Instance.PlaySFX(AudioChannelType.DIEGETIC, InteractionAudioClip, volume: 0.75f, pitch: 1.75f, randomizePitch: true);
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
        
        [ContextMenu("Throw Toy")]
        public void ThrowToy()
        {
            if (!_isInShelf)
            {
                transform.localPosition = _originPosition;
                transform.localRotation = _originRotation;
                _isInShelf = true;
                toyCollider.enabled = false;
                toyRigidbody.isKinematic = false;
                return;
            }

            _isCorrupted = true;
            toyRigidbody.isKinematic = true;

            _corruptedSequence?.Kill();

            _corruptedSequence = DOTween.Sequence()
                .Append(transform.DOLocalMoveX(transform.localPosition.x + pullDistance, pullDuration)
                    .SetEase(Ease.InOutQuart)
                    .OnComplete(() =>
                    {
                        toyRigidbody.isKinematic = false;
                        toyRigidbody.useGravity = false;
                        _isInShelf =  false;
                        toyCollider.enabled = true;
                        EnableInteraction();
                        
                        toyRigidbody.AddTorque(
                            Random.insideUnitSphere * spinForce,
                            ForceMode.Impulse);
                    }))
                .Append(transform.DOLocalMoveY(transform.localPosition.y + Random.Range(-0.25f, 0.5f), pullDuration * 10f));
        }
    }
}