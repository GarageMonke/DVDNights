using UnityEngine;

namespace DVDNights
{
    [CreateAssetMenu(fileName = "-InspectableDataSO", menuName = "ScriptableObjects/Inspection/InspectableDataSO")]
    public class InspectableDataSO : ScriptableObject
    {
        [Header("Configuration")] 
        [SerializeField] private GameObject inspectableObject;
        [SerializeField] private string inspectableTitle;
        [SerializeField] private string inspectableDescription;

        [SerializeField] private Vector2 inspectionMaxAngle;
        [SerializeField] private float inspectionMaxZoom;
        [SerializeField] private float inspectionStartSize;
        
        public GameObject InspectableObject => inspectableObject;
        public string InspectableTitle => inspectableTitle;
        public string InspectableDescription => inspectableDescription;
        
        public Vector2 InspectionMaxAngle => inspectionMaxAngle;
        public float InspectionMaxZoom => inspectionMaxZoom;
        public float InspectionStartSize => inspectionStartSize;
    }
}