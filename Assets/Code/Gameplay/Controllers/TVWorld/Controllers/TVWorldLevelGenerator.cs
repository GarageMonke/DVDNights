using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Rulebound
{
    public class TVWorldLevelGenerator : MonoBehaviour
    {
        [Serializable]
        public class PlatformPrefab
        {
            public GameObject prefab;
            [Min(1)] public int weight = 1;
        }

        [Header("Level")] 
        [SerializeField] private BoxCollider levelBounds;
        [SerializeField] private Transform container;
        [SerializeField] private Transform startPoint;
        [SerializeField] private Transform goalPoint;

        [Header("Platforms")] 
        [SerializeField] private List<PlatformPrefab> platformPrefabs = new();

        [Header("Path Distance")] 
        [SerializeField, Min(0.01f)] private float minimumDistance = 2f;
        [SerializeField, Min(0.01f)] private float maximumDistance = 6f;

        [Header("Path Randomness")] [Tooltip("0 = almost straight. 1 = very random.")] 
        [SerializeField, Range(0f, 1f)] private float randomness = 0.5f;

        [Tooltip("Maximum amount of random sideways movement.")] 
        [SerializeField, Min(0f)] private float maximumSideOffset = 3f;

        [Header("Extra Objects")] 
        [SerializeField, Min(0)] private int extraObjectCount = 10;

        [SerializeField, Min(0f)] private float minimumObjectSpacing = 1f;

        [Header("Generation")] 
        [SerializeField] private bool generateOnStart = true;
        [SerializeField] private bool randomizeSeed = true;
        [SerializeField] private int seed = 12345;
        [SerializeField, Min(1)] private int maximumAttempts = 100;
       
        [Header("Debug")] [SerializeField] private bool drawPath = true;
        [SerializeField] private bool drawBounds = true;
        [SerializeField] private bool verboseLogging = true;

        private readonly List<Vector3> _path = new();
        private readonly List<GameObject> _spawnedObjects = new();

        private System.Random _random;

        private void Start()
        {
            if (generateOnStart)
            {
                GenerateLevel();
            }
        }

        [ContextMenu("Generate Level")]
        public void GenerateLevel()
        {
            if (!ValidateSetup())
                return;

            ClearLevel();

            if (randomizeSeed)
            {
                seed = Random.Range(int.MinValue, int.MaxValue);
            }

            _random = new System.Random(seed);

            if (verboseLogging)
            {
                float straightLineDistance = Vector3.Distance(startPoint.position, goalPoint.position);
                Bounds worldBounds = levelBounds.bounds;

                Debug.Log(
                    $"[LevelGen] Starting generation. Seed: {seed}, " +
                    $"Start-Goal distance: {straightLineDistance:0.00}, " +
                    $"MinDist: {minimumDistance}, MaxDist: {maximumDistance}, " +
                    $"LevelBounds min: {worldBounds.min}, max: {worldBounds.max}, size: {worldBounds.size}",
                    this);

                Debug.Log(
                    $"[LevelGen] Start Point world pos: {startPoint.position}, " +
                    $"inside LevelBounds: {worldBounds.Contains(startPoint.position)}",
                    this);

                Debug.Log(
                    $"[LevelGen] Goal Point world pos: {goalPoint.position}, " +
                    $"inside LevelBounds: {worldBounds.Contains(goalPoint.position)}",
                    this);
            }

            for (int attempt = 0; attempt < maximumAttempts; attempt++)
            {
                _path.Clear();

                if (!GeneratePath())
                {
                    if (verboseLogging)
                    {
                        Debug.Log($"[LevelGen] Attempt {attempt}: GeneratePath failed.", this);
                    }

                    continue;
                }

                if (!SpawnPath())
                {
                    if (verboseLogging)
                    {
                        Debug.Log($"[LevelGen] Attempt {attempt}: SpawnPath failed.", this);
                    }

                    ClearSpawnedOnly();
                    continue;
                }

                if (!SpawnExtraObjects())
                {
                    if (verboseLogging)
                    {
                        Debug.Log($"[LevelGen] Attempt {attempt}: SpawnExtraObjects failed " +
                                  $"(placed {_spawnedObjects.Count - (_path.Count - 2)} of {extraObjectCount}).", this);
                    }

                    ClearSpawnedOnly();
                    continue;
                }

                Debug.Log(
                    $"Level generated successfully. " +
                    $"Platforms: {_path.Count - 2}, Seed: {seed}",
                    this);

                return;
            }

            Debug.LogError(
                "Could not generate a valid level. " +
                "Try lowering Minimum Distance or increasing Maximum Distance. " +
                "Enable Verbose Logging to see which stage is failing and why.",
                this);

            ClearLevel();
        }

        private bool GeneratePath()
        {
            _path.Clear();

            Vector3 current = startPoint.position;
            Vector3 goal = goalPoint.position;

            _path.Add(current);

            float totalDistance =
                Vector3.Distance(current, goal);
            
            if (totalDistance <= maximumDistance)
            {
                if (totalDistance < minimumDistance)
                {
                    if (verboseLogging)
                        Debug.Log(
                            $"[LevelGen] Start and Goal are only {totalDistance:0.00} apart, " +
                            $"which is below Minimum Distance ({minimumDistance}). " +
                            "This setup can never satisfy the minimum distance constraint " +
                            "- move Start/Goal further apart or lower Minimum Distance.",
                            this);

                    return false;
                }

                _path.Add(goal);
                return true;
            }
            
            int segmentCount = Mathf.CeilToInt(totalDistance / maximumDistance);

          
            if (totalDistance < segmentCount * minimumDistance)
            {
                if (verboseLogging)
                    Debug.Log(
                        $"[LevelGen] Impossible segment math: totalDistance {totalDistance:0.00} " +
                        $"< segmentCount {segmentCount} * minimumDistance {minimumDistance} " +
                        $"({segmentCount * minimumDistance:0.00}). Widen the gap between " +
                        "Minimum and Maximum Distance.",
                        this);

                return false;
            }

            Vector3 direction = (goal - current).normalized;

            Vector3 side = Vector3.Cross(direction, Vector3.up);
            
            if (side.sqrMagnitude < 0.001f)
            {
                side = Vector3.right;
            }

            side.Normalize();

            for (int i = 0; i < segmentCount - 1; i++)
            {
                Vector3 toGoal = goal - current;

                float remainingDistance = toGoal.magnitude;

                int remainingSegments = segmentCount - i;
                
                float minStep = minimumDistance;

                float maxStep = maximumDistance;
                
                float maximumAllowedStep = remainingDistance - minimumDistance * (remainingSegments - 1);

                maxStep = Mathf.Min(maxStep, maximumAllowedStep);

                float minimumRequiredStep = remainingDistance - maximumDistance * (remainingSegments - 1);

                minStep = Mathf.Max(minStep, minimumRequiredStep);

                if (minStep > maxStep)
                {
                    if (verboseLogging)
                    {
                        Debug.Log(
                            $"[LevelGen] Segment {i}: no valid step range " +
                            $"(minStep {minStep:0.00} > maxStep {maxStep:0.00}). " +
                            $"remainingDistance: {remainingDistance:0.00}, " +
                            $"remainingSegments: {remainingSegments}.",
                            this);
                    }

                    return false;
                }

                float stepDistance = RandomRange(minStep, maxStep);
                
                Vector3 currentDirection = toGoal.normalized;

                float randomSide = RandomRange(-maximumSideOffset, maximumSideOffset);

                randomSide *= randomness;

                Vector3 candidateDirection = currentDirection + side * randomSide;

                candidateDirection.Normalize();

                Vector3 candidate = current + candidateDirection * stepDistance;
                
                candidate = ClampToBounds(candidate);
                
                float actualDistance = Vector3.Distance(current, candidate);
                
                int segmentsAfterThis = remainingSegments - 1;

                float futureMin = segmentsAfterThis * minimumDistance;

                float futureMax = segmentsAfterThis * maximumDistance;

                float distanceToGoalFromCandidate = Vector3.Distance(candidate, goal);
                
                bool segmentValid = actualDistance >= minimumDistance && actualDistance <= maximumDistance;

                bool futureValid = distanceToGoalFromCandidate >= futureMin && distanceToGoalFromCandidate <= futureMax;

                if (!segmentValid || !futureValid)
                {
                    candidate = current + currentDirection * stepDistance;

                    candidate = ClampToBounds(candidate);

                    actualDistance = Vector3.Distance(current, candidate);

                    distanceToGoalFromCandidate = Vector3.Distance(candidate, goal);

                    bool fallbackSegmentValid = actualDistance >= minimumDistance && actualDistance <= maximumDistance;

                    bool fallbackFutureValid = distanceToGoalFromCandidate >= futureMin && distanceToGoalFromCandidate <= futureMax;

                    if (!fallbackSegmentValid || !fallbackFutureValid)
                    {
                        if (verboseLogging)
                            Debug.Log(
                                $"[LevelGen] Segment {i}: even the straight-line fallback " +
                                $"was invalid after clamping to bounds (distance {actualDistance:0.00}, " +
                                $"remaining-to-goal {distanceToGoalFromCandidate:0.00}, " +
                                $"needed [{futureMin:0.00}, {futureMax:0.00}]). " +
                                "Level Bounds is likely too small/tight for the requested distances - " +
                                "the box is clipping the step before it can reach a valid length.",
                                this);

                        return false;
                    }

                    if (verboseLogging)
                    {
                        Debug.Log(
                            $"[LevelGen] Segment {i}: random offset would have broken feasibility " +
                            "for remaining segments, used straight-line fallback instead.",
                            this);
                    }
                }

                _path.Add(candidate);

                current = candidate;

                direction = (goal - current).normalized;

                side = Vector3.Cross(direction, Vector3.up);

                if (side.sqrMagnitude < 0.001f)
                {
                    side = Vector3.right;
                }

                side.Normalize();
            }
            
            float finalDistance = Vector3.Distance(current, goal);

            if (finalDistance < minimumDistance || finalDistance > maximumDistance)
            {
                if (verboseLogging)
                {
                    Debug.Log(
                        $"[LevelGen] Final segment invalid: distance to goal is {finalDistance:0.00}, " +
                        $"outside [{minimumDistance}, {maximumDistance}]. This usually means earlier " +
                        "steps drifted off the planned line (side offset / bounds clamping).",
                        this);
                }

                return false;
            }

            _path.Add(goal);

            return true;
        }
        

        private bool SpawnPath()
        {
            for (int i = 1; i < _path.Count - 1; i++)
            {
                GameObject prefab = GetRandomPrefab();

                if (!prefab)
                {
                    if (verboseLogging)
                    {
                        Debug.Log(
                            "[LevelGen] GetRandomPrefab returned null while spawning path platform " +
                            $"{i}. Check that Platform Prefabs has at least one entry with a non-null " +
                            "prefab and weight >= 1.",
                            this);
                    }

                    return false;
                }

                GameObject instance = Instantiate(prefab, _path[i], prefab.transform.rotation, container);

                instance.name = $"PathPlatform_{i:00}";

                AlignTopToHeight(instance, _path[i].y);

                if (!IsValidPlacement(instance, out string reason))
                {
                    if (verboseLogging)
                    {
                        Debug.Log($"[LevelGen] Path platform {i} placement rejected: {reason}", this);
                    }

                    DestroyGeneratedObject(instance);
                    return false;
                }

                _spawnedObjects.Add(instance);
            }

            return true;
        }
        
        private bool SpawnExtraObjects()
        {
            Bounds bounds = levelBounds.bounds;
            int spawned = 0;
            int attempts = 0;
            int maxAttempts = Mathf.Max(50, extraObjectCount * 30);
            int rejectedForOverlap = 0;
            int rejectedForBounds = 0;

            while (spawned < extraObjectCount && attempts < maxAttempts)
            {
                attempts++;
                Vector3 position = new Vector3(
                    RandomRange(bounds.min.x, bounds.max.x),
                    RandomRange(bounds.min.y, bounds.max.y),
                    RandomRange(bounds.min.z, bounds.max.z));

                position = ClampToBounds(position);

                GameObject prefab = GetRandomPrefab();

                if (!prefab)
                {
                    continue;
                }

                GameObject instance = Instantiate(prefab, position, prefab.transform.rotation, container);

                instance.name = $"ExtraPlatform_{spawned:00}";

                AlignTopToHeight(instance, position.y);

                if (!IsValidPlacement(instance, out string reason))
                {
                    if (reason == "overlap")
                    {
                        rejectedForOverlap++;
                    }
                    else
                    {
                        rejectedForBounds++;
                    }

                    DestroyGeneratedObject(instance);
                    continue;
                }

                _spawnedObjects.Add(instance);

                spawned++;
            }

            if (spawned != extraObjectCount && verboseLogging)
            {
                Debug.Log(
                    $"[LevelGen] SpawnExtraObjects only placed {spawned}/{extraObjectCount} " +
                    $"in {attempts} attempts (rejected: {rejectedForOverlap} overlap, " +
                    $"{rejectedForBounds} out-of-bounds). If overlap rejections dominate, " +
                    "Minimum Object Spacing is too high or Level Bounds is too small for " +
                    "this many objects - note spacing is effectively enforced twice " +
                    "(once per object's expanded bounds).",
                    this);
            }

            return spawned == extraObjectCount;
        }

        private bool IsValidPlacement(GameObject candidate, out string reason)
        {
            reason = string.Empty;

            Collider[] candidateColliders = candidate.GetComponentsInChildren<Collider>();

            if (candidateColliders.Length == 0)
            {
                reason = "no collider";
                return false;
            }

            Bounds candidateBounds = CalculateBounds(candidateColliders);

            Bounds bounds = levelBounds.bounds;


            bool insideHorizontally =
                candidateBounds.min.x >= bounds.min.x &&
                candidateBounds.max.x <= bounds.max.x &&
                candidateBounds.min.z >= bounds.min.z &&
                candidateBounds.max.z <= bounds.max.z;

            if (!insideHorizontally)
            {
                reason = "outside level bounds";

                if (Application.isEditor)
                {
                    Debug.Log(
                        $"[LevelGen] Horizontal bounds check failed for '{candidate.name}'. " +
                        $"Object bounds min: {candidateBounds.min}, max: {candidateBounds.max} " +
                        $"(size {candidateBounds.size}). " +
                        $"Level bounds min: {bounds.min}, max: {bounds.max}. " +
                        $"Overflow - X: [{Mathf.Max(0f, bounds.min.x - candidateBounds.min.x):0.00} under, " +
                        $"{Mathf.Max(0f, candidateBounds.max.x - bounds.max.x):0.00} over], " +
                        $"Z: [{Mathf.Max(0f, bounds.min.z - candidateBounds.min.z):0.00} under, " +
                        $"{Mathf.Max(0f, candidateBounds.max.z - bounds.max.z):0.00} over]",
                        candidate);
                }

                return false;
            }

            Bounds expandedCandidate = candidateBounds;

            expandedCandidate.Expand(minimumObjectSpacing * 2f);

            foreach (GameObject existing in _spawnedObjects)
            {
                if (!existing) continue;

                Collider[] existingColliders = existing.GetComponentsInChildren<Collider>();

                if (existingColliders.Length == 0) continue;

                Bounds existingBounds = CalculateBounds(existingColliders);

                Bounds expandedExisting = existingBounds;

                expandedExisting.Expand(minimumObjectSpacing * 2f);

                if (expandedCandidate.Intersects(expandedExisting))
                {
                    reason = "overlap";
                    return false;
                }
            }

            return true;
        }

        private Bounds CalculateBounds(Collider[] colliders)
        {
            Bounds bounds = colliders[0].bounds;

            for (int i = 1; i < colliders.Length; i++)
            {
                bounds.Encapsulate(colliders[i].bounds);
            }

            return bounds;
        }

        private void AlignTopToHeight(GameObject instance, float height)
        {
            Collider[] colliders = instance.GetComponentsInChildren<Collider>();

            if (colliders.Length == 0)
            {
                return;
            }

            Bounds bounds = CalculateBounds(colliders);

            float difference = height - bounds.max.y;

            instance.transform.position += Vector3.up * difference;
        }

        private Vector3 ClampToBounds(Vector3 position)
        {
            Bounds bounds = levelBounds.bounds;

            position.x = Mathf.Clamp(position.x, bounds.min.x + 0.5f, bounds.max.x - 0.5f);
            position.y = Mathf.Clamp(position.y, bounds.min.y + 0.5f, bounds.max.y - 0.5f);
            position.z = Mathf.Clamp(position.z, bounds.min.z + 0.5f, bounds.max.z - 0.5f);

            return position;
        }

        private GameObject GetRandomPrefab()
        {
            int totalWeight = 0;

            foreach (PlatformPrefab platform in platformPrefabs)
            {
                if (!platform.prefab) continue;

                totalWeight += Mathf.Max(1, platform.weight);
            }

            if (totalWeight == 0)
            {
                return null;
            }

            int value = _random.Next(0, totalWeight);

            foreach (PlatformPrefab platform in platformPrefabs)
            {
                if (!platform.prefab)
                {
                    continue;
                }

                value -= Mathf.Max(1, platform.weight);

                if (value < 0)
                {
                    return platform.prefab;
                }
            }

            return null;
        }

        private float RandomRange(float min, float max)
        {
            return (float)(min + _random.NextDouble() * (max - min));
        }

        private void DestroyGeneratedObject(GameObject objectToDestroy)
        {
            if (!objectToDestroy)
            {
                return;
            }

#if UNITY_EDITOR
            if (!Application.isPlaying)
                DestroyImmediate(objectToDestroy);
            else
                Destroy(objectToDestroy);
#else
            Destroy(obj);
#endif
        }

        [ContextMenu("Clear Level")]
        public void ClearLevel()
        {
            for (int i = _spawnedObjects.Count - 1; i >= 0; i--)
            {
                if (!_spawnedObjects[i])
                {
                    continue;
                }

                DestroyGeneratedObject(_spawnedObjects[i]);
            }

            _spawnedObjects.Clear();
            _path.Clear();
        }
        
        private void ClearSpawnedOnly()
        {
            for (int i = _spawnedObjects.Count - 1; i >= 0; i--)
            {
                if (!_spawnedObjects[i])
                {
                    continue;
                }

                DestroyGeneratedObject(_spawnedObjects[i]);
            }

            _spawnedObjects.Clear();
        }

        private bool ValidateSetup()
        {
            if (!levelBounds)
            {
                Debug.LogError("Level Bounds is missing.", this);
                return false;
            }

            if (!startPoint)
            {
                Debug.LogError("Start Point is missing.", this);
                return false;
            }

            if (!goalPoint)
            {
                Debug.LogError("Goal Point is missing.", this);

                return false;
            }

            if (platformPrefabs == null ||
                platformPrefabs.Count == 0)
            {
                Debug.LogError("No platform prefabs assigned.", this);

                return false;
            }

            bool hasAnyValidPrefab = false;

            foreach (PlatformPrefab platform in platformPrefabs)
            {
                if (platform.prefab)
                {
                    hasAnyValidPrefab = true;
                    break;
                }
            }

            if (!hasAnyValidPrefab)
            {
                Debug.LogError("Platform Prefabs list has entries, but none of them have a prefab assigned.", this);
                return false;
            }

            if (minimumDistance >= maximumDistance)
            {
                Debug.LogError("Minimum Distance must be smaller than Maximum Distance.", this);
                return false;
            }

            return true;
        }

        private void OnDrawGizmos()
        {
            if (drawBounds && levelBounds)
            {
                Gizmos.matrix = levelBounds.transform.localToWorldMatrix;

                Gizmos.color = Color.white;

                Gizmos.DrawWireCube(levelBounds.center, levelBounds.size);

                Gizmos.matrix = Matrix4x4.identity;
            }

            if (!drawPath || _path.Count < 2)
            {
                return;
            }

            for (int i = 0; i < _path.Count; i++)
            {
                if (i == 0)
                    Gizmos.color = Color.green;
                else if (i == _path.Count - 1)
                    Gizmos.color = Color.red;
                else
                    Gizmos.color = Color.yellow;

                Gizmos.DrawSphere(_path[i], 0.2f);

                if (i > 0)
                {
                    Gizmos.color = Color.cyan;
                    Gizmos.DrawLine(_path[i - 1], _path[i]);
                }
            }
        }
    }
}