using UnityEngine;

namespace DVDNights
{
    [CreateAssetMenu(fileName = "-DiskDataSO", menuName = "ScriptableObjects/Disks/DiskDataSO")]
    public class DiskDataSO : ScriptableObject
    {
        [SerializeField] private DiskType diskType;
        [SerializeField] private int diskMultiplier;
        
        public DiskType DiskType => diskType;
        public int DiskMultiplier => diskMultiplier;
    }
}