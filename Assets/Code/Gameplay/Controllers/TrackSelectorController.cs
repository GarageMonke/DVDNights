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

        private IMouseLayoutController _mouseLayoutController;
        private ICameraController _cameraController;

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
            DisplayTrack();
            trackSelectionWindow.Display();
            trackSelectionWindow.OnNextTrackRequested += NextTrack;
            trackSelectionWindow.OnPreviousTrackRequested += PreviousTrack;
            trackSelectionWindow.OnSelectTrackRequested += SelectTrack;
            _mouseLayoutController.DisplayRegularLayout();
            _cameraController.Unfocus();
        }

        private void DisplayTrack()
        {
            DeleteTrack();
            TrackDataSO currentTrackData =  trackDataProvider.GetElementById(_currentTrackIndex.ToString());
            _trackObject = Instantiate(currentTrackData.TrackObject, trackOrigin);
            trackSelectionWindow.UpdateTrackInfo(currentTrackData.TrackTitle, currentTrackData.CoverArt, currentTrackData.Composer);
            AudioManager.Instance.PlayPreview(currentTrackData.TrackAudioClip);
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
            Debug.Log("Track selected");
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
    }

    public interface ITrackSelectionController
    {
        public void OpenTrackSelector();
        public void CloseTrackSelector();
    }
}