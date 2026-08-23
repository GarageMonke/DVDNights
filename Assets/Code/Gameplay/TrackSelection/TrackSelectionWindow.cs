using System;
using Common;
using CorePatterns.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Rulebound
{
    public class TrackSelectionWindow : Window, ITrackSelectionWindow
    {
        [Header("References")] 
        [SerializeField] private TextMeshProUGUI trackTitleText;
        [SerializeField] private TextMeshProUGUI coverArtText;
        [SerializeField] private TextMeshProUGUI composerText;
        [SerializeField] private Image trackImage;

        [SerializeField] private Button nextTrackButton;
        [SerializeField] private Button previousTrackButton;
        [SerializeField] private Button selectTrackButton;
        [SerializeField] private Button stopTrackButton;
        [SerializeField] private Button closeTrackButton;
        
        [SerializeField] private TextMeshProUGUI playButtonText;
        
        public Action OnNextTrackRequested { get; set; }
        public Action OnPreviousTrackRequested { get; set; }
        public Action OnSelectTrackRequested { get; set; }
        public Action OnStopTrackRequested { get; set; }
        public Action OnCloseTrackRequested { get; set; }
        
        protected override void Awake()
        {
            base.Awake();
            nextTrackButton.onClick.AddListener(RequestNextTrack);
            previousTrackButton.onClick.AddListener(RequestPreviousTrack);
            selectTrackButton.onClick.AddListener(RequestSelectTrack);
            stopTrackButton.onClick.AddListener(RequestStopTrack);
            closeTrackButton.onClick.AddListener(RequestCloseTrack);
        }

        public override void Close()
        {
            WindowManager.Instance.CloseWindow<TrackSelectionWindow>();
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
        
        private void RequestStopTrack()
        {
            OnStopTrackRequested?.Invoke();
        }
        
        private void RequestCloseTrack()
        {
            OnCloseTrackRequested?.Invoke();
        }

        public void UpdateTrackInfo(Sprite trackSprite, string trackTitle, string coverArt, string composer)
        {
            trackImage.sprite = trackSprite;
            trackTitleText.text = trackTitle;
            coverArtText.text = coverArt;
            composerText.text = composer;
        }

        public void EnableStopTrackButton()
        {
            stopTrackButton.gameObject.SetActive(true);
        }

        public void DisableStopTrackButton()
        {
            stopTrackButton.gameObject.SetActive(false);
        }

        public void ShowPlayAction()
        {
            playButtonText.text = "PLAY TRACK";
        }

        public void ShowResumeAction()
        {
            playButtonText.text = "RESUME TRACK";
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
        public Action OnStopTrackRequested { get; set; }
        public Action OnCloseTrackRequested { get; set; }

        public void UpdateTrackInfo(Sprite trackSprite, string trackTitle, string coverArt, string composer);
        public void EnableStopTrackButton();
        public void DisableStopTrackButton();

        public void ShowPlayAction();
        public void ShowResumeAction();
    }
}