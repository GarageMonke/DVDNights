using System;
using DG.Tweening;
using UnityEngine;

namespace Code.TestOnly
{
    public class GameTitlePromoHandler : MonoBehaviour
    {
        [SerializeField] private Material forwardMaterial;
        [SerializeField] private GameObject CTAGameObject;
        [SerializeField] private GameObject logosGameObject;
        
        private static readonly int ScrollSpeed = Shader.PropertyToID("_ScrollSpeed");
        private static readonly int DistortionStrength = Shader.PropertyToID("_DistortionStrength");
        private static readonly int ScanlineOpacity = Shader.PropertyToID("_ScanlineOpacity");

        private Sequence _promoSequence;
        private Sequence _blinkSequence;

        private void Start()
        {
            PlayPromoSequence();
        }

        public void SetForwardShader(int level)
        {
            float speed = 0;
            float distortion = 0;
            float opacity = 0;
            
            switch (level)
            {
                case 0:
                    speed = 0.2f;
                    distortion = 0.001f;
                    opacity = 0.001f;
                    break;
                case 1:
                case 2:
                case 3:
                    speed = 0.5f;
                    distortion = 0.01f;
                    opacity = 0.025f;
                    break;
                case 4:
                case 5:
                    speed = 0.75f;
                    distortion = 0.0125f;
                    opacity = 0.05f;
                    break;
                case 6:
                case 7:
                    speed = 1f;
                    distortion = 0.02f;
                    opacity = 0.1f;
                    break;
                case 8:
                case 9:
                    speed = 1.25f;
                    distortion = 0.035f;
                    opacity = 0.15f;
                    break;
                case 10:
                    speed = 3f;
                    distortion = 0.075f;
                    opacity = 0.5f;
                    break;
            }
            
            forwardMaterial.SetFloat(ScrollSpeed, speed);
            forwardMaterial.SetFloat(DistortionStrength, distortion);
            forwardMaterial.SetFloat(ScanlineOpacity, opacity);
        }


        private void PlayPromoSequence()
        {
            _promoSequence?.Kill();
            CTAGameObject.gameObject.SetActive(false);
            logosGameObject.gameObject.SetActive(false);
            _promoSequence = DOTween.Sequence();
            _promoSequence.AppendInterval(1f);
            _promoSequence.AppendCallback(() =>
            {
                SetForwardShader(10);
            });
            _promoSequence.AppendInterval(1f);
            _promoSequence.AppendCallback(() =>
            {
                SetForwardShader(0);
            });
            _promoSequence.AppendInterval(3f);
            _promoSequence.AppendCallback(() =>
            {
                StartBlinkingCTA();
                logosGameObject.SetActive(true);
            });
        }

        private void StartBlinkingCTA()
        {
            _blinkSequence?.Kill();
            _blinkSequence = DOTween.Sequence();

            _blinkSequence.AppendCallback(() => CTAGameObject.SetActive(true));
            _blinkSequence.AppendInterval(1f);
            _blinkSequence.AppendCallback(() => CTAGameObject.SetActive(false));
            _blinkSequence.AppendInterval(1f);

            _blinkSequence.SetLoops(-1);
        }
    }
}