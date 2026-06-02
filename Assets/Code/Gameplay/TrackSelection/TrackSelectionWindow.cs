using TMPro;
using UnityEngine;

namespace DVDNights
{
    public class TrackSelectionWindow : Window, ITrackSelectionWindow
    {
        [Header("References")] 
        [SerializeField] private TextMeshProUGUI trackTitleText;
        [SerializeField] private TextMeshProUGUI coverArtText;
        [SerializeField] private TextMeshProUGUI composerText;
        

        public void UpdateTrackInfo(string trackTitle, string coverArt, string composer)
        {
            trackTitleText.text = trackTitle;
            coverArtText.text = coverArt;
            composerText.text = composer;
        }
    }

    public interface ITrackSelectionWindow : IWindow
    {
        public void UpdateTrackInfo(string trackTitle, string coverArt, string composer);
    }
}