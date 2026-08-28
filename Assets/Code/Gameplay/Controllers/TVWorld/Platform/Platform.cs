using System;
using System.Collections;
using CorePatterns.ServiceLocator;
using UnityEngine;

namespace Rulebound
{
    [RequireComponent(typeof(Collider))]
    public abstract class Platform : MonoBehaviour, IPlatform
    {
        [Header("References")]
        [SerializeField] protected Renderer platformRenderer;
        [SerializeField] protected Collider platformCollider;
        [SerializeField] protected Color platformColor;

        [Header("Movement")]
        [SerializeField] private float downDistance = 2f;
        [SerializeField] private float movementDuration = 0.2f;

        public Action OnPlatformTriggered { get; set; }

        protected bool _isEnabled;

        protected bool _hasCharacter;

        protected IPlatformController _platformController;

        private Vector3 _initialPosition;
        private Coroutine _movementCoroutine;

        protected virtual void Start()
        {
            _initialPosition = transform.position;

            platformRenderer.material.color = platformColor;

            _platformController = ServiceLocator.GetService<IPlatformController>();
            _platformController.RegisterPlatform(this);
        }

        protected virtual void OnCollisionEnter(Collision collision)
        {
            if (!_isEnabled)
            {
                return;
            }

            if (_hasCharacter)
            {
                return;
            }

            ICharacter character = collision.gameObject.GetComponent<ICharacter>();

            if (character == null)
            {
                return;
            }

            _hasCharacter = true;
            character.CharacterTransform.parent = transform;
            MovePlatform(true);
            OnPlatformTriggered?.Invoke();
        }

        protected virtual void OnCollisionExit(Collision collision)
        {
            ICharacter character = collision.gameObject.GetComponent<ICharacter>();

            if (character == null)
            {
                return;
            }

            _hasCharacter = false;
            character.CharacterTransform.parent = character.OriginalParent;
            MovePlatform(false);
        }

        private void MovePlatform(bool down)
        {
            Vector3 targetPosition = down
                ? _initialPosition + Vector3.down * downDistance
                : _initialPosition;

            _movementCoroutine = StartCoroutine(MoveRoutine(targetPosition));
        }

        private IEnumerator MoveRoutine(Vector3 targetPosition)
        {
            Vector3 startPosition = transform.position;

            float elapsed = 0f;

            while (elapsed < movementDuration)
            {
                elapsed += Time.deltaTime;

                float t = elapsed / movementDuration;
                t = Mathf.SmoothStep(0f, 1f, t);

                transform.position = Vector3.Lerp(
                    startPosition,
                    targetPosition,
                    t
                );

                yield return null;
            }

            transform.position = targetPosition;
            _movementCoroutine = null;
        }

        public virtual void EnablePlatform()
        {
            _isEnabled = true;
        }

        public virtual void DisablePlatform()
        {
            _isEnabled = false;
        }

        public abstract void ResetPlatform();
    }

    public interface IPlatform
    {
        public void EnablePlatform();
        public void DisablePlatform();
        public void ResetPlatform();
        public Action OnPlatformTriggered { get; set; }
    }
}