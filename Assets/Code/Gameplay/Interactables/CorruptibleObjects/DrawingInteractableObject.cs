using CorePatterns.Managers;
using CorePatterns.Providers.Implementations;
using DG.Tweening;
using UnityEngine;

namespace DVDNights
{
    public class DrawingInteractableObject : ArtInteractableObject
    {
        [Header("Writing-Sequence")]
        [SerializeField] private Transform pencil;
        [SerializeField] private float width = 0.1f;
        [SerializeField] private float height = 0.1f;
        [SerializeField] private float strokeLength = 15f;
        [SerializeField] private float durationPerStroke = 0.20f;
        [SerializeField] private Vector3 writingStartPosition;
        [SerializeField] private Vector3 writingStartRotation;

        private Vector3 _originalPosition;
        private Vector3 _controlPosition;
        private Vector3 _originalRotation;
        private Sequence _writingSequence;
        private bool _drawn;

        protected override void Awake()
        {
            _originalPosition = pencil.localPosition;
            _originalRotation = pencil.localRotation.eulerAngles;
            _controlPosition = writingStartPosition + new Vector3(0, height / 2f, 0);
            _drawn = false;
        }
        public override void Corrupt()
        {
            StartWriting(4f);
        }

        private void StartWriting(float duration)
        {
            StopWriting();
            
            _writingSequence = DOTween.Sequence();
            
            _writingSequence.AppendCallback(() =>
            {
                pencil.DOLocalMove(_controlPosition, 1f).SetEase(Ease.InOutSine);
                pencil.DOLocalRotate(writingStartRotation, 1f).SetEase(Ease.InOutSine);
            });

            _writingSequence.AppendInterval(1.25f);
            
            _writingSequence.Append(pencil.DOLocalMove(writingStartPosition, 0.2f).SetEase(Ease.InOutSine));
            
            float elapsed = 0;

            while (elapsed < duration)
            {
                if (elapsed > duration / 3 && !_drawn)
                {
                    _drawn = true;
                }
                
                float randomX = Random.Range(-width, width);
                float randomZ = Random.Range(-height, height);
                
                Vector3 randomOffset = new Vector3(randomX, 0, randomZ);
                
                _writingSequence.AppendCallback(() =>
                {
                    AudioClip writingAudioClip = _artCorruptionController.GetDrawingAudioClip();
                    AudioManager.Instance.PlaySFX(AudioChannelType.DIEGETIC, writingAudioClip, 0.05f);
                    
                    pencil.DOLocalMove(writingStartPosition + randomOffset, durationPerStroke)
                        .SetEase(Ease.OutQuad);
                });
                
                _writingSequence.AppendInterval(durationPerStroke);
                
                elapsed += durationPerStroke;
            }

            _writingSequence.AppendCallback(() =>
            {
                base.Corrupt();
                
                pencil.DOLocalMove(_controlPosition, durationPerStroke).SetEase(Ease.InOutSine);
                pencil.DOLocalRotate(writingStartRotation, durationPerStroke).SetEase(Ease.InOutSine);
            });

            _writingSequence.AppendInterval(0.3f);
            
            _writingSequence.AppendCallback(() =>
            {
                pencil.DOLocalRotate(_originalRotation, 0.5f).SetEase(Ease.InOutSine);
                pencil.DOLocalMove(_originalPosition, 0.5f).SetEase(Ease.InOutSine);
            });
        }

        private void StopWriting()
        {
            if (_writingSequence != null)
            {
                _writingSequence.Kill();
                _writingSequence = null;
            }
        }
    }
}