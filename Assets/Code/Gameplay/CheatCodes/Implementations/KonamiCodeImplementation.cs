using System;
using CheatCodes.Definitions;
using Code.MVP;
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
        
        //Original State
        private Material _originalTVScreenMaterial;
        private float _originalContrastValue;
        private Color _originalColorValue;
        private AudioClip _originalAudioClip;

        private void Start()
        {
            _cheatCodesController = ServiceLocator.GetService<ICheatCodesController>();
            _cheatCodesController.RegisterCodeByImplementation(this);
            _colorAdjustments = PostProcessingManager.Instance.GetVolumeComponent<ColorAdjustments>();
            
            _originalContrastValue = _colorAdjustments.contrast.value;
            _originalTVScreenMaterial = tvScreenRenderer.material;
            _originalColorValue = _colorAdjustments.colorFilter.value;
            
            _isActive = false;

            WindowManager.Instance.OnWindowClosed += CheckToDeactivateCheat;
        }

        private void CheckToDeactivateCheat()
        {
            if (!WindowManager.Instance.IsWindowOpen<MainMenuWindow>())
            {
                DeactivateCheat();
            }
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

        private void DeactivateCheat()
        {
            _isActive = false;
            _cheatCodeSequence?.Kill();
            
            tvScreenRenderer.material = _originalTVScreenMaterial;
            _colorAdjustments.contrast.value = _originalContrastValue;
            _colorAdjustments.colorFilter.value = _originalColorValue;
            
            AudioManager.Instance.PlayOST(AudioChannelType.NONDIEGETIC, _originalAudioClip, loop: true, fadeIn: false);
           
            foreach (GameObject gameObjectToDeactivate in gameObjectsToActivate)
            {
                gameObjectToDeactivate.SetActive(false);
            }
        }
        
        public void CheatCodeSequence()
        {
            tvScreenRenderer.material = tvScreenMaterial;

            foreach (GameObject gameObjectToActivate in gameObjectsToActivate)
            {
                gameObjectToActivate.SetActive(true);
            }

            _originalAudioClip = AudioManager.Instance.GetChannelPlayingOST(AudioChannelType.NONDIEGETIC);
            
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

        private void OnDestroy()
        {
            if (WindowManager.Instance)
            {
                WindowManager.Instance.OnWindowClosed -= CheckToDeactivateCheat;
            }
        }
    }

    public interface ICheatCodeImplementation
    {
        public string GetCheatName();
        public void ActivateCheat();
    }
}