using UnityEngine;

namespace DVDNights
{
    [CreateAssetMenu(fileName = "-ArtDataSO", menuName = "ScriptableObjects/Art/ArtDataSO")]
    public class ArtDataSO : ScriptableObject
    {
        [Header("Configuration")] 
        [SerializeField] private string artistName;
        [SerializeField] private Sprite artSprite;
        
        public string ArtistName => artistName;
        public Sprite ArtSprite => artSprite;
    }
}