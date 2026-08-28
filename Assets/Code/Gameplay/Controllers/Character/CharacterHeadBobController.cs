using System;
using CorePatterns.ServiceLocator;
using UnityEngine;

namespace Rulebound
{
    public class CharacterHeadBobController : MonoBehaviour, ICharacterHeadBobController
    {
        [Header("References")]
        [SerializeField] private Transform cameraPivot;

        [Header("HeadBob")]
        [SerializeField] private HeadBobSettings headBob = new();

        private Vector3 _cameraRestLocalPos;
        private float _bobTimer;

        private ICharacterMovementController _characterMovementController;
        private ICharacterSprintController _characterSprintController;
        private ICharacterJumpController _characterJumpController;

        private bool _isEnabled;

        private void Awake()
        {
            if (cameraPivot)
            {
                _cameraRestLocalPos = cameraPivot.localPosition;
            }
            
            EnableController();
        }

        private void Start()
        {
            _characterMovementController = ServiceLocator.GetService<ICharacterMovementController>();
            _characterSprintController = ServiceLocator.GetService<ICharacterSprintController>();
            _characterJumpController = ServiceLocator.GetService<ICharacterJumpController>();
        }

        private void LateUpdate()
        {
            ApplyHeadBob();
        }

        private void ApplyHeadBob()
        {
            if (!_isEnabled|| !cameraPivot)
            {
                return;
            }

            if (_characterMovementController.IsMoving &&
                _characterJumpController.IsGrounded)
            {
                float frequencyMultiplier = _characterSprintController.IsSprinting ? headBob.frequencyMultiplier : 1f;

                _bobTimer += Time.deltaTime * headBob.frequency * frequencyMultiplier;

                float bobY = Mathf.Sin(_bobTimer) * headBob.amplitude;
                float bobX = Mathf.Cos(_bobTimer * 0.5f) * headBob.amplitude;

                Vector3 targetPosition = _cameraRestLocalPos + new Vector3(bobX, bobY, 0f);

                cameraPivot.localPosition = Vector3.Lerp(cameraPivot.localPosition, targetPosition, headBob.smoothing * Time.deltaTime);
            }
            else
            {
                cameraPivot.localPosition = Vector3.Lerp(cameraPivot.localPosition, _cameraRestLocalPos, headBob.smoothing * Time.deltaTime
                );
            }
        }

        public void ResetHeadBob()
        {
            _bobTimer = 0f;

            if (cameraPivot)
            {
                cameraPivot.localPosition = _cameraRestLocalPos;
            }
        }

        public void EnableController()
        {
            _isEnabled = true;
        }

        public void DisableController()
        {
            _isEnabled = false;
        }

        public void ResetController()
        {
            ResetHeadBob();
        }
    }

    public interface ICharacterHeadBobController : ICharacterController
    {
    }

    [Serializable]
    public class HeadBobSettings
    {
        public float frequency = 10f;
        public float frequencyMultiplier = 1.5f;
        public float amplitude = 1f;
        public float smoothing = 10f;
    }
}