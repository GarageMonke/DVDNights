using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DVDNights
{
    public class DVDLogoBouncer : MonoBehaviour
    {
        [Header("Movimiento")] [SerializeField]
        private float baseSpeed = 2f;

        [Header("Área de rebote")] 
        [SerializeField] private Transform bounceArea;

        [SerializeField] private Vector2 bounceAreaSize;
        [SerializeField] private Vector2 logoSize;

        [Header("Debug / Testing")] 
        [SerializeField] private bool forceCornerHit = false;

        [SerializeField] private CornerTarget forcedCorner = CornerTarget.BottomRight;
        [SerializeField] private bool visualizeCorners = true;

        [Header("Corner Detection")] [SerializeField]
        private float cornerThreshold = 0.05f;

        private Vector2 velocity;
        private Vector2 logoHalfSize;
        private Vector2 areaHalfSize;
        private bool isMoving = true;

        public Action OnBorderHit;
        public Action<CornerTarget> OnCornerHit;

        private void Start()
        {
            InitializeSizes();

            if (forceCornerHit)
                LaunchTowardCorner(forcedCorner);
            else
                LaunchRandom();
        }

        private void Update()
        {
            if (!isMoving) return;
            Move();
        }

        private void InitializeSizes()
        {
            var areaBounds = bounceArea.GetComponent<MeshCollider>().bounds;
            var logoBounds = GetComponent<SpriteRenderer>().bounds;

            areaHalfSize = new Vector2(areaBounds.extents.x, areaBounds.extents.y);
            logoHalfSize = new Vector2(logoBounds.extents.x, logoBounds.extents.y);
        }

        private void LaunchRandom()
        {
            float x = Random.value > 0.5f ? 1f : -1f;
            float y = Random.value > 0.5f ? 1f : -1f;
            velocity = new Vector2(x, y).normalized * baseSpeed;

            float px = Random.Range(-areaHalfSize.x + logoHalfSize.x, areaHalfSize.x - logoHalfSize.x);
            float py = Random.Range(-areaHalfSize.y + logoHalfSize.y, areaHalfSize.y - logoHalfSize.y);

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
                    startPos = new Vector2(-areaHalfSize.x + logoHalfSize.x + margin,
                        -areaHalfSize.y + logoHalfSize.y + margin);
                    dir = new Vector2(1f, 1f);
                    break;
                case CornerTarget.TopLeft:
                    startPos = new Vector2(areaHalfSize.x - logoHalfSize.x - margin,
                        -areaHalfSize.y + logoHalfSize.y + margin);
                    dir = new Vector2(-1f, 1f);
                    break;
                case CornerTarget.BottomLeft:
                    startPos = new Vector2(areaHalfSize.x - logoHalfSize.x - margin,
                        areaHalfSize.y - logoHalfSize.y - margin);
                    dir = new Vector2(-1f, -1f);
                    break;
                case CornerTarget.BottomRight:
                default:
                    startPos = new Vector2(-areaHalfSize.x + logoHalfSize.x + margin,
                        areaHalfSize.y - logoHalfSize.y - margin);
                    dir = new Vector2(1f, -1f);
                    break;
            }

            transform.position = bounceArea.position + new Vector3(startPos.x, startPos.y, 0f);
            velocity = dir.normalized * baseSpeed;
        }

        private void Move()
        {
            Vector3 worldPos = transform.position;
            Vector2 localPos = new Vector2(
                worldPos.x - bounceArea.position.x,
                worldPos.y - bounceArea.position.y
            );

            localPos += velocity * Time.deltaTime;

            float minX = -areaHalfSize.x + logoHalfSize.x;
            float maxX = areaHalfSize.x - logoHalfSize.x;
            float minY = -areaHalfSize.y + logoHalfSize.y;
            float maxY = areaHalfSize.y - logoHalfSize.y;

            bool hitX = false;
            bool hitY = false;

            if (localPos.x <= minX)
            {
                localPos.x = minX;
                velocity.x = Mathf.Abs(velocity.x);
                hitX = true;
            }
            else if (localPos.x >= maxX)
            {
                localPos.x = maxX;
                velocity.x = -Mathf.Abs(velocity.x);
                hitX = true;
            }

            if (localPos.y <= minY)
            {
                localPos.y = minY;
                velocity.y = Mathf.Abs(velocity.y);
                hitY = true;
            }
            else if (localPos.y >= maxY)
            {
                localPos.y = maxY;
                velocity.y = -Mathf.Abs(velocity.y);
                hitY = true;
            }

            transform.position = bounceArea.position + new Vector3(localPos.x, localPos.y, 0f);

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
            }
        }

        private bool IsNearCorner(Vector2 pos, float minX, float maxX, float minY, float maxY)
        {
            bool nearX = pos.x <= minX + cornerThreshold || pos.x >= maxX - cornerThreshold;
            bool nearY = pos.y <= minY + cornerThreshold || pos.y >= maxY - cornerThreshold;
            return nearX && nearY;
        }

        public void SetSpeedMultiplier(float multiplier) =>
            velocity = velocity.normalized * (baseSpeed * multiplier);

        public void RandomizeDirection()
        {
            float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
            velocity = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * velocity.magnitude;
        }

        public void SetMoving(bool moving) => isMoving = moving;

        public void DirectTowardCorner(CornerTarget corner)
        {
            Vector2 target = GetCornerLocalPosition(corner);
            Vector2 localPos = new Vector2(
                transform.position.x - bounceArea.position.x,
                transform.position.y - bounceArea.position.y
            );
            velocity = (target - localPos).normalized * velocity.magnitude;
        }
        
        public void DirectTowardNearestCorner()
        {
            Vector2 localPos = new Vector2(
                transform.position.x - bounceArea.position.x,
                transform.position.y - bounceArea.position.y
            );

            float minX = -areaHalfSize.x + logoHalfSize.x;
            float maxX =  areaHalfSize.x - logoHalfSize.x;
            float minY = -areaHalfSize.y + logoHalfSize.y;
            float maxY =  areaHalfSize.y - logoHalfSize.y;

            CornerTarget nearest = CornerTarget.BottomLeft;
            float nearestDist = float.MaxValue;

            foreach (CornerTarget corner in System.Enum.GetValues(typeof(CornerTarget)))
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
            float minX = -areaHalfSize.x + logoHalfSize.x;
            float maxX = areaHalfSize.x - logoHalfSize.x;
            float minY = -areaHalfSize.y + logoHalfSize.y;
            float maxY = areaHalfSize.y - logoHalfSize.y;

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


#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (!visualizeCorners || bounceArea == null) return;

            InitializeSizes();

            Vector3 center = bounceArea.position;

            float minX = -areaHalfSize.x + logoHalfSize.x;
            float maxX = areaHalfSize.x - logoHalfSize.x;
            float minY = -areaHalfSize.y + logoHalfSize.y;
            float maxY = areaHalfSize.y - logoHalfSize.y;

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