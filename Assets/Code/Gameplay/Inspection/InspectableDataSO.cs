using UnityEngine;

namespace DVDNights
{
    [CreateAssetMenu(fileName = "-InspectableDataSO", menuName = "ScriptableObjects/Inspection/InspectableDataSO")]
    public class InspectableDataSO : ScriptableObject
    {
        [Header("Configuration")] 
        [SerializeField] private GameObject inspectableObject;
        [SerializeField] private string inspectableTitle;
        [SerializeField] private string inspectableSubTitle;
        [SerializeField] private string inspectableDescription;

        [SerializeField] private Vector2 inspectionMaxAngle;
        [SerializeField] private float inspectionMaxZoom;
        [SerializeField] private float inspectionMinZoom = 1f;
        [SerializeField] private float inspectionStartSize;
        [SerializeField] private Vector3 inspectionStartRotation;
        
        public GameObject InspectableObject => inspectableObject;
        public string InspectableTitle => inspectableTitle;
        public string InspectableSubTitle => inspectableSubTitle;
        public string InspectableDescription => inspectableDescription;
        
        public Vector2 InspectionMaxAngle => inspectionMaxAngle;
        public float InspectionMaxZoom => inspectionMaxZoom;
        public float InspectionMinZoom => inspectionMinZoom;
        public float InspectionStartSize => inspectionStartSize;
        
        public Vector3 InspectionStartRotation => inspectionStartRotation;
        
    }
}