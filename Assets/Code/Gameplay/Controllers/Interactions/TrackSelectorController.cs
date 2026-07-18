using System;
using CorePatterns.Managers;
using CorePatterns.Providers.Implementations;
using CorePatterns.ServiceLocator;
using UnityEngine;

namespace DVDNights
{
    public class TrackSelectorController : MonoBehaviour, ITrackSelectionController
    {
        [Header("References")] 
        [SerializeField] private Transform trackOrigin;
        [SerializeField] private TrackSelectionWindow trackSelectionWindow;
        [SerializeField] private TrackDataProvider trackDataProvider;
        
        private GameObject _trackObject;
        
        private int _currentTrackIndex = 0;
        private int _previousTrackIndex = -1;
        private bool _shouldPlayFromStart;
        private bool _isPlayingTrack;

        private IMouseLayoutController _mouseLayoutController;
        private ICameraController _cameraController;

        public Action OnTrackPlayRequested { get; set; }
        public Action OnTrackSelectionCloseRequested { get; set; }
        public Action OnTrackStopRequested { get; set; }
        public bool IsPlayingTrack => _isPlayingTrack;
        public bool IsPlayingSameTrack => !_shouldPlayFromStart;

        private void Awake()
        {
            InstallService();
        }

        private void InstallService()
        {
            trackDataProvider.InitializeProvider();
            ServiceLocator.RegisterService<ITrackSelectionController>(this);
        }

        private void Start()
        {
            _mouseLayoutController = ServiceLocator.GetService<IMouseLayoutController>();
            _cameraController = ServiceLocator.GetService<ICameraController>();
        }

        public void OpenTrackSelector()
        {
            AudioManager.Instance.PauseOST(AudioChannelType.TURNTABLE, fadeDuration: 0f);
            DisplayTrack();
            trackSelectionWindow.Display();
            SubscribeToEvents();
            _mouseLayoutController.DisplayRegularLayout();
            _cameraController.Unfocus();
            CheckPreviousTrack();

            if (_isPlayingTrack)
            {
                trackSelectionWindow.EnableStopTrackButton();
            }
            else
            {
                trackSelectionWindow.DisableStopTrackButton();
            }
        }
     
        private void DisplayTrack()
        {
            DeleteTrack();
            TrackDataSO currentTrackData = trackDataProvider.GetElementById(_currentTrackIndex.ToString());
            _trackObject = Instantiate(currentTrackData.TrackObject, trackOrigin);
            trackSelectionWindow.UpdateTrackInfo(currentTrackData.TrackTitle, currentTrackData.CoverArt, currentTrackData.Composer);
            AudioManager.Instance.PlayPreview(AudioChannelType.NONDIEGETIC, currentTrackData.TrackAudioClip);
        }

        private void NextTrack()
        {
            _currentTrackIndex++;

            if (_currentTrackIndex >= trackDataProvider.GetCount())
            {
                _currentTrackIndex = 0;
            }
            
            DisplayTrack();
            CheckPreviousTrack();
        }

        private void PreviousTrack()
        {
            _currentTrackIndex--;
            
            if (_currentTrackIndex < 0)
            {
                _currentTrackIndex = trackDataProvider.GetCount() - 1;
            }
            
            DisplayTrack();
            CheckPreviousTrack();
        }

        private void SelectTrack()
        {
            if (_currentTrackIndex == _previousTrackIndex)
            {
                _shouldPlayFromStart = false;
            }
            else
            {
                _previousTrackIndex = _currentTrackIndex;
                _shouldPlayFromStart = true;
            }
            
            AudioManager.Instance.StopOST(AudioChannelType.NONDIEGETIC, fadeOut: false);
            OnTrackPlayRequested?.Invoke();
            CloseTrackSelector();
        }

        private void DeleteTrack()
        {
            if (!_trackObject)
            {
                return;
            }
            
            Destroy(_trackObject);
            _trackObject = null;
        }
        
        public void CloseTrackSelector()
        {
            DeleteTrack();
            UnsubscribeToEvents();
            _mouseLayoutController.HideMouseLayout();
            trackSelectionWindow.Hide();
            _cameraController.Focus();
        }

        public void PlaySelectedTrack()
        {
            _isPlayingTrack = true;
            
            if (_shouldPlayFromStart)
            {
                TrackDataSO currentTrackData = trackDataProvider.GetElementById(_currentTrackIndex.ToString());
                AudioManager.Instance.PlayOST(AudioChannelType.TURNTABLE, currentTrackData.TrackAudioClip);
                return;
            }
            
            AudioManager.Instance.ResumeOST(AudioChannelType.TURNTABLE);
        }

        public void PauseSelectedTrack()
        {
            AudioManager.Instance.PauseOST(AudioChannelType.TURNTABLE);
        }

        public void ResumeSelectedTrack()
        {
            if (_isPlayingTrack)
            {
                AudioManager.Instance.ResumeOST(AudioChannelType.TURNTABLE);
            }
        }

        private void ExitTrackSelection()
        {
            OnTrackSelectionCloseRequested?.Invoke();
            AudioManager.Instance.StopOST(AudioChannelType.NONDIEGETIC, fadeOut: false);
            CloseTrackSelector();
        }

        private void StopPlayingTrack()
        {
            _previousTrackIndex = -1;
            _isPlayingTrack = false;
            OnTrackStopRequested?.Invoke();
            ExitTrackSelection();
        }

        private void SubscribeToEvents()
        {
            trackSelectionWindow.OnNextTrackRequested += NextTrack;
            trackSelectionWindow.OnPreviousTrackRequested += PreviousTrack;
            trackSelectionWindow.OnSelectTrackRequested += SelectTrack;
            trackSelectionWindow.OnStopTrackRequested += StopPlayingTrack;
            trackSelectionWindow.OnCloseTrackRequested += ExitTrackSelection;
        }

        private void UnsubscribeToEvents()
        {
            trackSelectionWindow.OnNextTrackRequested -= NextTrack;
            trackSelectionWindow.OnPreviousTrackRequested -= PreviousTrack;
            trackSelectionWindow.OnSelectTrackRequested -= SelectTrack;
            trackSelectionWindow.OnStopTrackRequested -= StopPlayingTrack;
            trackSelectionWindow.OnCloseTrackRequested -= ExitTrackSelection;
        }

        private void CheckPreviousTrack()
        {
            if (_currentTrackIndex == _previousTrackIndex)
            {
                trackSelectionWindow.ShowResumeAction();
            }
            else
            {
                trackSelectionWindow.ShowPlayAction();
            }
        }
    }

    public interface ITrackSelectionController
    {
        public Action OnTrackPlayRequested { get; set; }
        public Action OnTrackSelectionCloseRequested { get; set; }
        public Action OnTrackStopRequested { get; set; }
        public bool IsPlayingTrack { get; }
        public bool IsPlayingSameTrack { get; }
        public void OpenTrackSelector();
        public void CloseTrackSelector();
        public void PlaySelectedTrack();
        public void PauseSelectedTrack();
        public void ResumeSelectedTrack();

    }
}