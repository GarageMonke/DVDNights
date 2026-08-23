using System;
using CorePatterns.Managers;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Rulebound
{
    public class LampInteractableObject : CorruptibleInteractableObject
    {
        [Header("References")] 
        [SerializeField] private Light lampLight;
        
        [Header("Feedback")]
        [SerializeField] private AudioClip turnOnLampAudioClip;
        [SerializeField] private AudioClip turnOffLampAudioClip;

        [Header("Materials")] 
        [SerializeField] private Renderer lampRenderer;
        [SerializeField] private Material shadeEmissiveMaterial;
        [SerializeField] private Material shadeMaterial;

        [Header("Flicker Settings")] 
        [SerializeField] private AudioClip flickeringAudioClip;
        [SerializeField] private float minIntensity = 0.015f;
        [SerializeField] private float maxIntensity = 1.5f;
        [SerializeField] private float minStepTime = 0.02f;
        [SerializeField] private float maxStepTime = 0.1f;

        public Action OnLampTurnedOn;

        public bool IsOn => _isOn;
        
        private bool _isOn;
        private Sequence _flickerSequence;
        private float _thresholdIntensity;
        private float _originalIntensity;

        private void Awake()
        {
            _thresholdIntensity = (minIntensity + maxIntensity) / 2f;
            _originalIntensity = lampLight.intensity;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.L))
            {
                Corrupt();
            }
        }

        public override string GetInteractionAction()
        {
            return _isOn ? "Turn Off" : "Turn On";
        }

        public override void Interact()
        {
            _isOn = !_isOn;
            lampLight.enabled = _isOn;

            AudioClip toPlay;
            
            if (_isOn)
            {
                toPlay = turnOnLampAudioClip;
                OverrideShadeMaterial(shadeEmissiveMaterial);
                _rulesViolationController.RemoveRuleViolation(ObjectId);
                OnLampTurnedOn?.Invoke();
            }
            else
            {
                if (_isCorrupted)
                {
                    ClearCorruption();
                }
                
                _rulesViolationController.AddRuleViolation(ObjectId);
                toPlay = turnOffLampAudioClip;
                OverrideShadeMaterial(shadeMaterial);
            }
            
            AudioManager.Instance.PlaySFX(AudioChannelType.NONDIEGETIC, toPlay, volume: 0.5f, pitch: 1.25f);
        }

        private void OverrideShadeMaterial(Material newMaterial)
        {
            Material[] mats = lampRenderer.materials;
            mats[1] = new Material(newMaterial);
            lampRenderer.materials = mats;
        }
        
        public override void Corrupt()
        {
            base.Corrupt();
            StartFlicker();
        }

        public override void ClearCorruption()
        {
            base.ClearCorruption();
            StopFlicker();
        }

        public override bool CanBeCorrupted()
        {
            return _isOn;
        }
        
        private void StartFlicker()
        {
            StopFlicker();

            _flickerSequence = DOTween.Sequence();

            _flickerSequence.SetLoops(-1, LoopType.Restart);

            _flickerSequence.AppendCallback(() =>
            {
                float targetIntensity = Random.Range(minIntensity, maxIntensity);
                
                float duration = Random.Range(minStepTime, maxStepTime);

                lampLight.DOIntensity(targetIntensity, duration)
                    .SetEase(Ease.Flash).OnComplete(() =>
                    {
                        if (targetIntensity > _thresholdIntensity)
                        {
                            float playSfx = Random.value;
                            if (playSfx >= 0.6f)
                            {
                                AudioManager.Instance.PlaySFX(AudioChannelType.NONDIEGETIC, flickeringAudioClip, volume: 0.05f,
                                    pitch: 1f);
                            }
                        }
                    });
            });
            
            _flickerSequence.AppendInterval(Random.Range(minStepTime, maxStepTime));
        }

        public void SetLampIntensity(float intensity)
        {
            lampLight.intensity = intensity;
        }

        public void RestoreLampIntensity()
        {
            lampLight.intensity = _originalIntensity;
        }

        private void StopFlicker()
        {
            if (_flickerSequence != null && _flickerSequence.IsActive())
            {
                _flickerSequence.Kill();
                _flickerSequence = null;
            }

            if (lampLight)
            {
                lampLight.DOKill();
            }
        }
    }
}