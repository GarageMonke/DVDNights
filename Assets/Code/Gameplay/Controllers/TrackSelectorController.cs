using System;
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
        }

        public void OpenTrackSelector()
        {
            DisplayTrack();
            trackSelectionWindow.Display();
            _mouseLayoutController.DisplayRegularLayout();
        }

        private void DisplayTrack()
        {
            TrackDataSO currentTrackData =  trackDataProvider.GetElementById(_currentTrackIndex.ToString());
            _trackObject = Instantiate(currentTrackData.TrackObject, trackOrigin);
            trackSelectionWindow.UpdateTrackInfo(currentTrackData.TrackTitle, currentTrackData.CoverArt, currentTrackData.Composer);
        }

        private void NextTrack()
        {
            _currentTrackIndex++;

            if (_currentTrackIndex > trackDataProvider.GetCount())
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
        
        public void CloseTrackSelector()
        {
            if (_trackObject)
            {
                Destroy(_trackObject);
                _trackObject = null;
            }
            
            _mouseLayoutController.HideMouseLayout();
            trackSelectionWindow.Hide();
        }
    }

    public interface ITrackSelectionController
    {
        public void OpenTrackSelector();
        public void CloseTrackSelector();
    }
}