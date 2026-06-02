using CorePatterns.Managers;
using DG.Tweening;
using UnityEngine;

namespace Code.Gameplay.Misc
{
    public class ThunderLightning : MonoBehaviour
    {
        [Header("References")] [SerializeField]
        private Light thunderLight;

        [SerializeField] private AudioClip[] thunderAudioClips;

        [Header("Lightning Settings")] [SerializeField]
        private float maxIntensity = 50f;

        [SerializeField] private float defaultIntensity;

        private Sequence _lightningSequence;

        private void Awake()
        {
            thunderLight.intensity = defaultIntensity;
        }

        public void Strike()
        {
            _lightningSequence?.Kill();
            _lightningSequence = DOTween.Sequence();
            
            int patternIndex = Random.Range(0, 3);
            
            switch (patternIndex)
            {
                case 0:
                    BuildClassicFlickerSequence(_lightningSequence);
                    break;
                case 1:
                    BuildDoubleStrikeSequence(_lightningSequence);
                    break;
                case 2:
                    BuildShortZapSequence(_lightningSequence);
                    break;
            }
            
            _lightningSequence.OnComplete(PlayRandomThunder);
        }

        // --- PATTERN 1: Your original classic flickering sequence ---
        private void BuildClassicFlickerSequence(Sequence seq)
        {
            seq.Append(thunderLight.DOIntensity(maxIntensity, 0.05f));
            seq.Append(thunderLight.DOIntensity(maxIntensity * 0.2f, 0.04f));
            seq.Append(thunderLight.DOIntensity(maxIntensity * 0.8f, 0.03f));
            seq.Append(thunderLight.DOIntensity(maxIntensity * 0.1f, 0.05f));
            seq.Append(thunderLight.DOIntensity(maxIntensity, 0.02f));
            seq.Append(thunderLight.DOIntensity(defaultIntensity, 0.4f).SetEase(Ease.OutQuad));
        }

        // --- PATTERN 2: Two massive flashes with a pause
        private void BuildDoubleStrikeSequence(Sequence seq)
        {
            seq.Append(thunderLight.DOIntensity(maxIntensity, 0.03f));
            seq.Append(thunderLight.DOIntensity(defaultIntensity, 0.08f));
            
            seq.AppendInterval(0.05f);
            
            seq.Append(thunderLight.DOIntensity(maxIntensity * 1.2f, 0.04f));
            seq.Append(thunderLight.DOIntensity(defaultIntensity, 0.6f).SetEase(Ease.OutCubic));
        }

        //PATTERN 3: The short sheet-lightning zap (Fast, no real flicker)
        private void BuildShortZapSequence(Sequence seq)
        {
            seq.Append(thunderLight.DOIntensity(maxIntensity * 0.7f, 0.02f));
            seq.Append(thunderLight.DOIntensity(defaultIntensity, 0.2f).SetEase(Ease.InQuad));
        }

        private void PlayRandomThunder()
        {
            int randomIndex = Random.Range(0, thunderAudioClips.Length);
            AudioManager.Instance.PlaySFX(thunderAudioClips[randomIndex]);
        }
    }
}