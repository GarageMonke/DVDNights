using System;
using UnityEngine;

namespace Rulebound
{
    [Serializable]
    public class CameraPositionData
    {
        [Header("Shoot")]
        public Vector3 cameraPosition;
        public Vector3 cameraRotation;
        
        [Header("Tween")]
        public bool playTween;
        public Vector3 cameraTargetPosition;
        public Vector3 cameraTargetRotation;
        public float timeToPosition = 1f;
        public float timeToRotation = 1f;
    }
}