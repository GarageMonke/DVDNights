using System;
using UnityEngine;

namespace DVDNights
{
    [Serializable]
    public class InteractionData
    {
        [Header("Camera Configuration")]
        [SerializeField] private Vector3 cameraPosition;
        [SerializeField] private Vector3 cameraRotation;
        [SerializeField] private Vector3 cameraEndPosition;
        [SerializeField] private bool overrideCamera;
        [SerializeField] private bool tweenToPosition;
        [SerializeField] private bool unhighlightOnInteraction;
        
        public Vector3 CameraPosition => cameraPosition;
        public Vector3 CameraRotation => cameraRotation;
        public Vector3 CameraEndPosition => cameraEndPosition;
        public bool OverrideCamera => overrideCamera;
        public bool TweenToPosition => tweenToPosition;
        public bool UnhighlightOnInteraction => unhighlightOnInteraction;
    }
}