using DVDNights;
using UnityEngine;

namespace CorePatterns.Providers.Implementations
{
    [CreateAssetMenu(fileName = "-DiskDataProviderSO", menuName = "ScriptableObjects/Disks/DiskDataProviderSO")]
    public class DiskDataProvider : Provider<DiskDataSO>
    {
    }
}