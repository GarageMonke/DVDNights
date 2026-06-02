using UnityEngine;

namespace DVDNights
{
    [CreateAssetMenu(fileName = "-TrackDataSO", menuName = "ScriptableObjects/Tracks/TrackDataSO")]
    public class TrackDataSO : ScriptableObject
    {
        [Header("Configuration")] 
        [SerializeField] private AudioClip trackAudioClip;
        [SerializeField] private GameObject trackObject;
        [SerializeField] private string trackTitle;
        [SerializeField] private string coverArt;
        [SerializeField] private string composer;
        
        public AudioClip TrackAudioClip => trackAudioClip;
        public GameObject TrackObject => trackObject;
        public string TrackTitle => trackTitle;
        public string CoverArt => coverArt;
        public string Composer => composer;
    }
}