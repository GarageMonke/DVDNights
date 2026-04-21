using System;
using CorePatterns.Managers;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DVDNights
{
    public class DVDDiskBouncer : MonoBehaviour
    {
        [Header("Configuration")] 
        [SerializeField] private float baseSpeed = 2f;
        [SerializeField] private float cornerThreshold = 0.05f;

        [Header("References")] 
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Transform bounceArea;
        
        [Header("Retro Effect")]
        [SerializeField] [Range(0, 50)] private int frameSkip; // 0 = smooth
        private int _frameCounter;
        private Vector3 _displayPosition;
        private float _displayRotation;

        [Header("Feedback")] 
        [SerializeField] private Color[] feedbackColors;
        [SerializeField] private AudioClip audioClip;
        [SerializeField] private float volume;
        [SerializeField] private float pitch;

        [Header("Debug")] 
        [SerializeField] private CornerTarget forcedCorner = CornerTarget.BottomRight;
        [SerializeField] private bool visualizeCorners = true;
        
        private Vector2 _velocity;
        private Vector2 _logoHalfSize;
        private Vector2 _areaHalfSize;
        private bool _isMoving = true;
        private float _spinSpeed;

        private int _currentColorIndex;
        public Action OnBorderHit;
        public Action<CornerTarget> OnCornerHit;

        private void Start()
        {
            InitializeSizes();
            LaunchRandom();
        }

        private void Update()
        {
            if (!_isMoving) return;

            if (frameSkip == 0)
            {
                Move();
                SpinDisk();
                return;
            }

            _frameCounter++;
            
            if (_frameCounter > frameSkip)
            {
                _frameCounter = 0;
        
                // Simulate all skipped frames at once
                int framesToSimulate = frameSkip + 1;
                for (int i = 0; i < framesToSimulate; i++)
                {
                    SimulateMove();
                    SimulateSpin();
                }

                transform.position = _displayPosition;
                transform.rotation = Quaternion.Euler(0, 0, _displayRotation);
            }
        }

        private void InitializeSizes()
        {
            var areaBounds = bounceArea.GetComponent<MeshCollider>().bounds;
            var logoBounds = GetComponent<SpriteRenderer>().bounds;

            _areaHalfSize = new Vector2(areaBounds.extents.x, areaBounds.extents.y);
            _logoHalfSize = new Vector2(logoBounds.extents.x, logoBounds.extents.y);
            
            ApplySpeed();
        }
        
        public float BaseSpeed
        {
            get => baseSpeed;
            set
            {
                baseSpeed = value;
                ApplySpeed();
            }
        }

        private void OnValidate()
        {
            ApplySpeed();
        }

        private void ApplySpeed()
        {
            _spinSpeed = Mathf.Max(1, baseSpeed / 100f);
            
            if (_velocity == Vector2.zero) return;
            _velocity = _velocity.normalized * baseSpeed;
        }

        private void LaunchRandom()
        {
            float x = Random.value > 0.5f ? 1f : -1f;
            float y = Random.value > 0.5f ? 1f : -1f;
            _velocity = new Vector2(x, y).normalized * baseSpeed;

            float px = Random.Range(-_areaHalfSize.x + _logoHalfSize.x, _areaHalfSize.x - _logoHalfSize.x);
            float py = Random.Range(-_areaHalfSize.y + _logoHalfSize.y, _areaHalfSize.y - _logoHalfSize.y);

            transform.position = bounceArea.position + new Vector3(px, py, 0f);
        }

        private void LaunchTowardCorner(CornerTarget corner)
        {
            float margin = 0.1f;

            Vector2 startPos;
            Vector2 dir;

            switch (corner)
            {
                case CornerTarget.TopRight:
                    startPos = new Vector2(-_areaHalfSize.x + _logoHalfSize.x + margin,
                        -_areaHalfSize.y + _logoHalfSize.y + margin);
                    dir = new Vector2(1f, 1f);
                    break;
                case CornerTarget.TopLeft:
                    startPos = new Vector2(_areaHalfSize.x - _logoHalfSize.x - margin,
                        -_areaHalfSize.y + _logoHalfSize.y + margin);
                    dir = new Vector2(-1f, 1f);
                    break;
                case CornerTarget.BottomLeft:
                    startPos = new Vector2(_areaHalfSize.x - _logoHalfSize.x - margin,
                        _areaHalfSize.y - _logoHalfSize.y - margin);
                    dir = new Vector2(-1f, -1f);
                    break;
                case CornerTarget.BottomRight:
                default:
                    startPos = new Vector2(-_areaHalfSize.x + _logoHalfSize.x + margin,
                        _areaHalfSize.y - _logoHalfSize.y - margin);
                    dir = new Vector2(1f, -1f);
                    break;
            }

            transform.position = bounceArea.position + new Vector3(startPos.x, startPos.y, 0f);
            _velocity = dir.normalized * baseSpeed;
        }
        
        private void SimulateMove()
        {
            Vector3 worldPos = transform.position;
            Vector2 localPos = new Vector2(
                worldPos.x - bounceArea.position.x,
                worldPos.y - bounceArea.position.y
            );

            if (frameSkip > 0)
            {
                localPos += _velocity * (Time.deltaTime * frameSkip);
            }
            else
            {
                localPos += _velocity * Time.deltaTime;
            }

            float minX = -_areaHalfSize.x + _logoHalfSize.x;
            float maxX =  _areaHalfSize.x - _logoHalfSize.x;
            float minY = -_areaHalfSize.y + _logoHalfSize.y;
            float maxY =  _areaHalfSize.y - _logoHalfSize.y;

            bool hitX = false;
            bool hitY = false;

            if (localPos.x <= minX) { localPos.x = minX; _velocity.x =  Mathf.Abs(_velocity.x); hitX = true; }
            else if (localPos.x >= maxX) { localPos.x = maxX; _velocity.x = -Mathf.Abs(_velocity.x); hitX = true; }

            if (localPos.y <= minY) { localPos.y = minY; _velocity.y =  Mathf.Abs(_velocity.y); hitY = true; }
            else if (localPos.y >= maxY) { localPos.y = maxY; _velocity.y = -Mathf.Abs(_velocity.y); hitY = true; }

            // Store display position for when frame renders
            _displayPosition = bounceArea.position + new Vector3(localPos.x, localPos.y, 0f);

            // Events still fire every frame — no missed hits
            if (hitX || hitY)
            {
                if ((hitX && hitY) || IsNearCorner(localPos, minX, maxX, minY, maxY))
                {
                    CornerTarget corner = DetectCorner(localPos, minX, maxX, minY, maxY);
                    OnCornerHit?.Invoke(corner);
                }
                else
                {
                    OnBorderHit?.Invoke();
                }

                PlayFeedback();
            }
        }

        private void SimulateSpin()
        {
            _displayRotation += _spinSpeed;
        }

        private void Move()
        {
            SimulateMove();
            transform.position = _displayPosition;
        }

        private void SpinDisk()
        {
            SimulateSpin();
            transform.rotation = Quaternion.Euler(0, 0, _displayRotation);
        }

        private bool IsNearCorner(Vector2 pos, float minX, float maxX, float minY, float maxY)
        {
            bool nearX = pos.x <= minX + cornerThreshold || pos.x >= maxX - cornerThreshold;
            bool nearY = pos.y <= minY + cornerThreshold || pos.y >= maxY - cornerThreshold;
            return nearX && nearY;
        }

        public void SetSpeedMultiplier(float multiplier)
        {
            _velocity = _velocity.normalized * (baseSpeed * multiplier);
        }

        public void RandomizeDirection()
        {
            float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
            _velocity = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * _velocity.magnitude;
        }

        public void SetMoving(bool moving) => _isMoving = moving;

        public void DirectTowardCorner(CornerTarget corner)
        {
            Vector2 target = GetCornerLocalPosition(corner);
            Vector2 localPos = new Vector2(
                transform.position.x - bounceArea.position.x,
                transform.position.y - bounceArea.position.y
            );
            _velocity = (target - localPos).normalized * _velocity.magnitude;
        }
        
        public void DirectTowardNearestCorner()
        {
            Vector2 localPos = new Vector2(
                transform.position.x - bounceArea.position.x,
                transform.position.y - bounceArea.position.y
            );
            

            CornerTarget nearest = CornerTarget.BottomLeft;
            float nearestDist = float.MaxValue;

            foreach (CornerTarget corner in Enum.GetValues(typeof(CornerTarget)))
            {
                float dist = Vector2.Distance(localPos, GetCornerLocalPosition(corner));
                if (dist < nearestDist)
                {
                    nearestDist = dist;
                    nearest = corner;
                }
            }

            DirectTowardCorner(nearest);
        }

        private Vector2 GetCornerLocalPosition(CornerTarget corner)
        {
            float minX = -_areaHalfSize.x + _logoHalfSize.x;
            float maxX = _areaHalfSize.x - _logoHalfSize.x;
            float minY = -_areaHalfSize.y + _logoHalfSize.y;
            float maxY = _areaHalfSize.y - _logoHalfSize.y;

            return corner switch
            {
                CornerTarget.TopLeft => new Vector2(minX, maxY),
                CornerTarget.TopRight => new Vector2(maxX, maxY),
                CornerTarget.BottomLeft => new Vector2(minX, minY),
                CornerTarget.BottomRight => new Vector2(maxX, minY),
                _ => Vector2.zero
            };
        }
        
        private CornerTarget DetectCorner(Vector2 pos, float minX, float maxX, float minY, float maxY)
        {
            bool isLeft  = pos.x <= minX + cornerThreshold;
            bool isBottom = pos.y <= minY + cornerThreshold;

            if (isLeft)
                return isBottom ? CornerTarget.BottomLeft : CornerTarget.TopLeft;
            else
                return isBottom ? CornerTarget.BottomRight : CornerTarget.TopRight;
        }

        private void PlayFeedback()
        {
            AudioManager.Instance.PlaySFX(audioClip, volume, pitch, true);
        }

        private void ChangeToRandomColor()
        {
            int randomColorIndex = Random.Range(0, feedbackColors.Length);

            while (randomColorIndex == _currentColorIndex)
            {
                randomColorIndex = Random.Range(0, feedbackColors.Length);
            }
            
            _currentColorIndex = randomColorIndex;
            spriteRenderer.color = feedbackColors[randomColorIndex];
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (!visualizeCorners || bounceArea == null) return;

            InitializeSizes();

            Vector3 center = bounceArea.position;

            float minX = -_areaHalfSize.x + _logoHalfSize.x;
            float maxX = _areaHalfSize.x - _logoHalfSize.x;
            float minY = -_areaHalfSize.y + _logoHalfSize.y;
            float maxY = _areaHalfSize.y - _logoHalfSize.y;

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(center, new Vector3(maxX - minX, maxY - minY, 0f));

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(center + new Vector3(minX, minY), cornerThreshold);
            Gizmos.DrawWireSphere(center + new Vector3(maxX, minY), cornerThreshold);
            Gizmos.DrawWireSphere(center + new Vector3(minX, maxY), cornerThreshold);
            Gizmos.DrawWireSphere(center + new Vector3(maxX, maxY), cornerThreshold);
        }
        
        [ContextMenu("Force Nearest Corner")]
        private void DEBUG_ForceNearestCorner()
        {
            InitializeSizes();
            DirectTowardNearestCorner();
        }
#endif
    }
}