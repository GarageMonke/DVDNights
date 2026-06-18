using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DVDNights
{
    public class TrackSelectionWindow : Window, ITrackSelectionWindow
    {
        [Header("References")] 
        [SerializeField] private TextMeshProUGUI trackTitleText;
        [SerializeField] private TextMeshProUGUI coverArtText;
        [SerializeField] private TextMeshProUGUI composerText;

        [SerializeField] private Button nextTrackButton;
        [SerializeField] private Button previousTrackButton;
        [SerializeField] private Button selectTrackButton;
        [SerializeField] private Button exitTrackButton;
        
        public Action OnNextTrackRequested { get; set; }
        public Action OnPreviousTrackRequested { get; set; }
        public Action OnSelectTrackRequested { get; set; }
        public Action OnExitTrackRequested { get; set; }
        
        private void Awake()
        {
            nextTrackButton.onClick.AddListener(RequestNextTrack);
            previousTrackButton.onClick.AddListener(RequestPreviousTrack);
            selectTrackButton.onClick.AddListener(RequestSelectTrack);
            exitTrackButton.onClick.AddListener(RequestExitTrack);
        }

        private void RequestNextTrack()
        {
            OnNextTrackRequested?.Invoke();
        }

        private void RequestPreviousTrack()
        {
            OnPreviousTrackRequested?.Invoke();
        }
        
        private void RequestSelectTrack()
        {
            OnSelectTrackRequested?.Invoke();
        }
        
        private void RequestExitTrack()
        {
            OnExitTrackRequested?.Invoke();
        }

        public void UpdateTrackInfo(string trackTitle, string coverArt, string composer)
        {
            trackTitleText.text = trackTitle;
            coverArtText.text = coverArt;
            composerText.text = composer;
        }

        private void OnDestroy()
        {
            OnNextTrackRequested = null;
            OnPreviousTrackRequested = null;
        }
    }

    public interface ITrackSelectionWindow : IWindow
    {
        public Action OnNextTrackRequested { get; set; }
        public Action OnPreviousTrackRequested { get; set; }
        public Action OnSelectTrackRequested { get; set; }
        public Action OnExitTrackRequested { get; set; }
        
        public void UpdateTrackInfo(string trackTitle, string coverArt, string composer);
    }
}