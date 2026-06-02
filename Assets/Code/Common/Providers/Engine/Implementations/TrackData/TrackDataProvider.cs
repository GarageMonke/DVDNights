using DVDNights;
using UnityEngine;

namespace CorePatterns.Providers.Implementations
{
    [CreateAssetMenu(fileName = "-TrackDataProviderSO", menuName = "ScriptableObjects/Tracks/TrackDataProviderSO")]
    public class TrackDataProvider : Provider<TrackDataSO>
    {
    }
}