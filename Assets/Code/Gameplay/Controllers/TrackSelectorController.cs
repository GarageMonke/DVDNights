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

        public Action OnTrackSelectionCloseRequested { get; set; }
        public bool IsPlayingTrack => _isPlayingTrack;

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
            trackSelectionWindow.OnNextTrackRequested += NextTrack;
            trackSelectionWindow.OnPreviousTrackRequested += PreviousTrack;
            trackSelectionWindow.OnSelectTrackRequested += SelectTrack;
            trackSelectionWindow.OnExitTrackRequested += ExitTrackSelection;
            _mouseLayoutController.DisplayRegularLayout();
            _cameraController.Unfocus();
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
        }

        private void PreviousTrack()
        {
            _currentTrackIndex--;
            
            if (_currentTrackIndex < 0)
            {
                _currentTrackIndex = trackDataProvider.GetCount() - 1;
            }
            
            DisplayTrack();
        }

        private void SelectTrack()
        {
            if (_currentTrackIndex == _previousTrackIndex)
            {
                _shouldPlayFromStart = false;
                _previousTrackIndex = _currentTrackIndex;
               
            }
            else
            {
                _shouldPlayFromStart = true;
            }
            
            AudioManager.Instance.StopOST(AudioChannelType.NONDIEGETIC, fadeOut: false);
            _isPlayingTrack = true;
            OnTrackSelectionCloseRequested?.Invoke();
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
            
            trackSelectionWindow.OnNextTrackRequested -= NextTrack;
            trackSelectionWindow.OnPreviousTrackRequested -= PreviousTrack;
            trackSelectionWindow.OnSelectTrackRequested -= SelectTrack;
            
            _mouseLayoutController.HideMouseLayout();
            trackSelectionWindow.Hide();
            _cameraController.Focus();
        }

        public void PlaySelectedTrack()
        {
            if (_shouldPlayFromStart)
            {
                TrackDataSO currentTrackData = trackDataProvider.GetElementById(_currentTrackIndex.ToString());
                AudioManager.Instance.PlayOST(AudioChannelType.TURNTABLE, currentTrackData.TrackAudioClip);
                return;
            }
            
            AudioManager.Instance.ResumeOST(AudioChannelType.TURNTABLE);
        }
        
        private void ExitTrackSelection()
        {
            _isPlayingTrack = false;
            AudioManager.Instance.StopOST(AudioChannelType.NONDIEGETIC, fadeOut: false);
            OnTrackSelectionCloseRequested?.Invoke();
        }
    }

    public interface ITrackSelectionController
    {
        public Action OnTrackSelectionCloseRequested { get; set; }
        public bool IsPlayingTrack { get; }
        public void OpenTrackSelector();
        public void CloseTrackSelector();
        public void PlaySelectedTrack();

    }
}