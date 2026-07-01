using CorePatterns.Managers;
using CorePatterns.Providers.Implementations;
using CorePatterns.ServiceLocator;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;
using Sequence = DG.Tweening.Sequence;

namespace DVDNights
{
    public class CellphoneInteractableObject : CorruptibleInteractableObject
    {
        private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");

        [Header("References")] 
        [SerializeField] private MeshRenderer phoneRenderer;
        [SerializeField] private MeshRenderer screenRenderer;
      
        
        [Header("Corrupt-Configuration")]
        [SerializeField] private Material incomingCallMaterial;
        [SerializeField] private Material blackScreenMaterial;
        [SerializeField] private Color incomingCallColor;
        [SerializeField] private Color wrongCallColor;
        [SerializeField] private Color successColor;
        [SerializeField] private Color idleColor;
        
        [Header("Audio-Feedback")]
        [SerializeField] private AudioClip callSound;
        [SerializeField] private AudioClip endSound;
        [SerializeField] private AudioClip unansweredSound;
        [SerializeField] private AudioClipProvider wrongCallProvider;

        private int _currentRings;
        private Sequence _callSequence;
        private ISanityController _sanityController;
        private AudioClip _previousAudioClip;

        private void Awake()
        {
            TurnOff();
            DisableInteraction();
            wrongCallProvider.InitializeProvider();
        }

        protected override void Start()
        {
            base.Start();
            _sanityController = ServiceLocator.GetService<ISanityController>();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.O))
            {
                Corrupt();
            }
        }

        public override string GetInteractionAction()
        {
            return "Answer Phone";
        }

        public override void Interact()
        {
            if (_isCorrupted)
            {
                AudioManager.Instance.PlaySFX(AudioChannelType.DIEGETIC, InteractionAudioClip);
                DisableInteraction();
                
                if (_currentRings % 2 == 0)
                {
                    _sanityController.TakeSanityImmediate(PenaltyType.MID);
                    PlayWrongSequence();
                    return;
                }
                
                PlaySuccessSequence();
            }
        }

        public override void Corrupt()
        {
            base.Corrupt();
            PlayCallSequence();
            TurnOn();
            EnableInteraction();
        }

        public override void ClearCorruption()
        {
            base.ClearCorruption();
            ChangeScreenColor(incomingCallColor);
            _callSequence?.Kill();
            TurnOff();
            DisableInteraction();
        }

        private void TurnOff()
        {
            screenRenderer.material = blackScreenMaterial;
            phoneRenderer.material.color = idleColor;
        }

        private void TurnOn()
        {
            screenRenderer.material = incomingCallMaterial;
            phoneRenderer.material.color = incomingCallColor;
        }

        private void UnansweredCall()
        {
            Debug.Log("Call dismissed");
            _callSequence?.Kill();
            _callSequence = DOTween.Sequence();
            _callSequence.AppendInterval(0.25f);
            _callSequence.AppendCallback(() =>
            {
                AudioManager.Instance.PlaySFX(AudioChannelType.DIEGETIC, unansweredSound);
            });
            _callSequence.AppendInterval(unansweredSound.length);
            _callSequence.AppendCallback(() =>
            {
                AudioManager.Instance.PlaySFX(AudioChannelType.DIEGETIC, unansweredSound);
            });
            _callSequence.AppendInterval(unansweredSound.length);
            _callSequence.AppendCallback(() =>
            {
                _sanityController.TakeSanityImmediate(PenaltyType.EXTREME);
                ClearCorruption();
            });
        }
        
        private void PlaySuccessSequence()
        {
            ChangeScreenColor(successColor);
            _callSequence?.Kill();
            _callSequence = DOTween.Sequence().SetLoops(4).OnComplete(
                ()=> DOVirtual.DelayedCall(endSound.length, ClearCorruption));
            _callSequence.AppendInterval(InteractionAudioClip.length);
            _callSequence.AppendCallback(() =>
            {
                AudioManager.Instance.PlaySFX(AudioChannelType.DIEGETIC, endSound);
            });
            _callSequence.AppendInterval(endSound.length / 2f);
        }

        private void ChangeScreenColor(Color toChangeColor)
        {
            Material material = phoneRenderer.material; 
            material.SetColor(EmissionColor, toChangeColor);
        }

        private void PlayWrongSequence()
        {
            ChangeScreenColor(wrongCallColor);
            AudioClip randomAudioClip = wrongCallProvider.GetRandomElement();

            if (_previousAudioClip)
            {
                while (randomAudioClip.name == _previousAudioClip.name)
                {
                    randomAudioClip = wrongCallProvider.GetRandomElement();
                }
            }

            _callSequence?.Kill();
            _callSequence = DOTween.Sequence();
            _callSequence.AppendInterval(InteractionAudioClip.length);
            _callSequence.AppendCallback(() =>
            {
                AudioManager.Instance.PlaySFX(AudioChannelType.DIEGETIC, randomAudioClip);
                ShakePhone();
            });
            _callSequence.AppendInterval(randomAudioClip.length);
            _callSequence.AppendCallback(() =>
            {
                AudioManager.Instance.PlaySFX(AudioChannelType.DIEGETIC, endSound);
                ShakePhone();
            });
            _callSequence.AppendInterval(endSound.length);
            _callSequence.AppendCallback(ClearCorruption);
        }

        private void PlayCallSequence()
        {
            _callSequence?.Kill();
            _currentRings = 0;

            _callSequence = DOTween.Sequence().SetLoops(3).OnComplete(UnansweredCall);

            _callSequence.AppendCallback(() =>
            {
                AudioManager.Instance.PlaySFX(AudioChannelType.DIEGETIC, callSound);
                _currentRings++;
                Debug.Log("[Ring] " + _currentRings); 
                ShakePhone();
            });
            
            _callSequence.AppendInterval(callSound.length);
            _callSequence.AppendInterval(Random.Range(1f, 3f));
        }

        private void ShakePhone()
        {
            transform.DOShakePosition(
                callSound.length,
                strength: 0.0005f,
                vibrato: 5,
                randomness: 0,
                snapping: false,
                fadeOut: true);
        }
    }
}