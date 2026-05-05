using UnityEngine;

namespace DVDNights
{
    [CreateAssetMenu(fileName = "-DiskDataSO", menuName = "ScriptableObjects/Disks/DiskDataSO")]
    public class DiskDataSO : ScriptableObject
    {
        [Header("Configuration")]
        [SerializeField] private DiskType diskType;
        [SerializeField] private int diskMultiplier;
        [SerializeField] private Color diskColor;
        
        public DiskType DiskType => diskType;
        public int DiskMultiplier => diskMultiplier;
        public Color DiskColor => diskColor;
    }
}