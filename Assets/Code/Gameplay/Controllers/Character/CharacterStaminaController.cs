using System;
using CorePatterns.ServiceLocator;
using UnityEngine;

namespace Rulebound
{
    public class CharacterStaminaController : MonoBehaviour, ICharacterStaminaController
    {
        [Header("Stamina")]
        [SerializeField] private float maxStamina = 100f;
        [SerializeField] private float staminaDrainRate = 20f;
        [SerializeField] private float staminaRecoveryRate = 15f;
        [SerializeField] private float minimumSprintStamina = 1f;

        private float _currentStamina;
        private bool _isEnabled;

        public float CurrentStamina => _currentStamina;
        public float MaxStamina => maxStamina;
        public float StaminaPercentage => _currentStamina / maxStamina;

        public bool HasStamina => _isEnabled && _currentStamina >= minimumSprintStamina;

        private void Awake()
        {
            _currentStamina = maxStamina;
            ServiceLocator.RegisterService<ICharacterStaminaController>(this);
            EnableController();
        }

        private void Update()
        {
            if (!_isEnabled)
            {
                return;
            }

            RecoverStamina();
        }

        public void ConsumeStamina(float deltaTime)
        {
            if (!_isEnabled)
            {
                return;
            }

            _currentStamina -= staminaDrainRate * deltaTime;
            _currentStamina = Mathf.Max(_currentStamina, 0f);
        }

        private void RecoverStamina()
        {
            _currentStamina += staminaRecoveryRate * Time.deltaTime;
            _currentStamina = Mathf.Min(_currentStamina, maxStamina);
        }

        public void EnableController()
        {
            _isEnabled = true;
        }

        public void DisableController()
        {
            _isEnabled = false;
        }
    }

    public interface ICharacterStaminaController
    {
        public float CurrentStamina { get; }
        public float MaxStamina { get; }
        public float StaminaPercentage { get; }
        public bool HasStamina { get; }

        public void ConsumeStamina(float deltaTime);
        public void EnableController();
        public void DisableController();
    }
}