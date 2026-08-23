using System;
using UnityEngine;

namespace Rulebound
{
    [Serializable]
    public class StartToTargetTweenData
    {
        [Header("Position-Configuration")]
        public Vector3 startPosition;
        public Vector3 targetPosition;
        [Header("Rotation-Configuration")]
        public Vector3 startRotation;
        public Vector3 targetRotation;
    }
}