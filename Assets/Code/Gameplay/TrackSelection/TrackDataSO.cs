using UnityEngine;

namespace Rulebound
{
    [CreateAssetMenu(fileName = "-TrackDataSO", menuName = "ScriptableObjects/Tracks/TrackDataSO")]
    public class TrackDataSO : ScriptableObject
    {
        [Header("Configuration")] 
        [SerializeField] private string trackTitle;
        [SerializeField] private string coverArt;
        [SerializeField] private string composer;
        [SerializeField] private GameObject trackObject;
        [SerializeField] private AudioClip trackAudioClip;
        [SerializeField] private Material vinylCaseMaterial;
        [SerializeField] private Material vinylMaterial;
        [SerializeField] private Sprite vinylSprite;
        [SerializeField] private bool isUnlocked;
        
        public AudioClip TrackAudioClip => trackAudioClip;
        public GameObject TrackObject => trackObject;
        public string TrackTitle => trackTitle;
        public string CoverArt => coverArt;
        public string Composer => composer;
        public Material VinylCaseMaterial => vinylCaseMaterial;
        public Material VinylMaterial => vinylMaterial;
        public Sprite VinylSprite => vinylSprite;

        public bool IsUnlocked => isUnlocked;

        public void UnlockTrack()
        {
            isUnlocked = true;
        }
    }
}