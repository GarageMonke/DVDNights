using CheatCodes.Definitions;
using CorePatterns.Managers;
using CorePatterns.ServiceLocator;
using DG.Tweening;
using Rulebound;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace CheatCodes.Implementations
{
    public class KonamiCodeImplementation : MonoBehaviour, ICheatCodeImplementation
    {
        [Header("CheatCode")]
        [SerializeField] private KonamiCodeDefinition konamiCodeDefinition;
        
        [Header("References")] 
        [SerializeField] private Volume sceneVolume;
        [SerializeField] private GameObject[] gameObjectsToActivate;
        [SerializeField] private Renderer tvScreenRenderer;
        [SerializeField] private Material tvScreenMaterial;
        
        [Header("Feedback")]
        [SerializeField] private AudioClip cheatActivatedOSTAudioClip;
        [SerializeField] private AudioClip cheatActivatedSFXAudioClip;

        private ICheatCodesController _cheatCodesController;
        private ColorAdjustments _colorAdjustments;
        private Sequence _cheatCodeSequence;

        private bool _isActive;

        private void Start()
        {
            _cheatCodesController = ServiceLocator.GetService<ICheatCodesController>();
            _cheatCodesController.RegisterCodeByImplementation(this);
            _colorAdjustments = PostProcessingManager.Instance.GetVolumeComponent<ColorAdjustments>();
            _isActive = false;
        }

        public string GetCheatName()
        {
            return konamiCodeDefinition.CodeName;
        }

        public void ActivateCheat()
        {
            if (_isActive)
            {
                return;
            }

            _isActive = true;
            
            Debug.Log("Konami Code Activated!");
            
            AudioManager.Instance.PlaySFX(AudioChannelType.NONDIEGETIC, cheatActivatedSFXAudioClip);

            DOVirtual.DelayedCall(cheatActivatedSFXAudioClip.length / 2f, CheatCodeSequence);
        }
        
        public void CheatCodeSequence()
        {
            tvScreenRenderer.material = tvScreenMaterial;

            foreach (GameObject gameObjectToActivate in gameObjectsToActivate)
            {
                gameObjectToActivate.SetActive(true);
            }
            
            AudioManager.Instance.StopOST(AudioChannelType.NONDIEGETIC, fadeOut: false);
            AudioManager.Instance.PlayOST(AudioChannelType.NONDIEGETIC, cheatActivatedOSTAudioClip, loop: true);
            _colorAdjustments.contrast.value = -60f;
            
            Color[] colors =
            {
                Color.red,
                Color.yellow,
                Color.green,
                Color.cyan,
                Color.blue,
                Color.magenta
            };
            
            _cheatCodeSequence?.Kill();
            _cheatCodeSequence = DOTween.Sequence();
            
            foreach (Color color in colors)
            {
                _cheatCodeSequence.AppendCallback(() =>
                {
                    _colorAdjustments.colorFilter.value = color;
                });

                _cheatCodeSequence.AppendInterval(0.5f);
            }
            
            _cheatCodeSequence.SetLoops(-1, LoopType.Restart);
        }
    }

    public interface ICheatCodeImplementation
    {
        public string GetCheatName();
        public void ActivateCheat();
    }
}