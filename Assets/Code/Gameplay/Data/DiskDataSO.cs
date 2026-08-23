using UnityEngine;

namespace Rulebound
{
    [CreateAssetMenu(fileName = "-DiskDataSO", menuName = "ScriptableObjects/Disks/DiskDataSO")]
    public class DiskDataSO : ScriptableObject
    {
        [Header("Configuration")]
        [SerializeField] private DiskType diskType;
        [SerializeField] private int diskMultiplier;
        [SerializeField] private Color diskColor;
        [SerializeField] private Material diskMaterial;
        
        public DiskType DiskType => diskType;
        public int DiskMultiplier => diskMultiplier;
        public Color DiskColor => diskColor;
        public Material DiskMaterial => diskMaterial;
    }
}