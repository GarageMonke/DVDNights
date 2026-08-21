using System;
using System.Collections.Generic;
using CorePatterns.Managers;
using CorePatterns.Providers.Implementations;
using CorePatterns.ServiceLocator;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DVDNights
{
    public class TrackSelectorController : MonoBehaviour, ITrackSelectionController
    {
        [Header("References")] 
        [SerializeField] private TrackDataProvider trackDataProvider;
        [SerializeField] private GameObject trackSelectionPrefab;
        
        private GameObject _trackObject;
        
        private int _currentTrackIndex = 0;
        private int _previousTrackIndex = -1;
        private bool _shouldPlayFromStart;
        private bool _isPlayingTrack;
        private TrackDataSO _selectedTrackData;
        
        private TrackSelectionWindow _trackSelectionWindow;

        public Action OnTrackPlayRequested { get; set; }
        public Action OnTrackStopRequested { get; set; }
        public Action OnTrackStartPlaying { get; set; }
        public TrackDataSO SelectedTrackData => _selectedTrackData;
        public bool IsPlayingTrack => _isPlayingTrack;
        public bool IsPlayingSameTrack => !_shouldPlayFromStart;

        private List<TrackDataSO> _allUnlockedTracks;

        private void Awake()
        {
            InstallService();
        }

        private void InstallService()
        {
            trackDataProvider.InitializeProvider();
            ServiceLocator.RegisterService<ITrackSelectionController>(this);
            _allUnlockedTracks = new List<TrackDataSO>();
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
            
            _selectedTrackData = trackDataProvider.GetElementById(_currentTrackIndex.ToString());
            
            OnTrackPlayRequested?.Invoke();
        }

        public void PlaySelectedTrack()
        {
            _isPlayingTrack = true;
            
            if (_shouldPlayFromStart)
            {
                TrackDataSO currentTrackData = trackDataProvider.GetElementById(_currentTrackIndex.ToString());
                AudioManager.Instance.PlayOST(AudioChannelType.TURNTABLE, currentTrackData.TrackAudioClip);
                OnTrackStartPlaying?.Invoke();
                return;
            }
            
            AudioManager.Instance.ResumeOST(AudioChannelType.TURNTABLE);
            OnTrackStartPlaying?.Invoke();
        }

        public void StopPlayingTrack()
        {
            PauseSelectedTrack();
            _selectedTrackData = null;
            _previousTrackIndex = -1;
            _isPlayingTrack = false;
            OnTrackStopRequested?.Invoke();
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

        public void SelectRandomTrack()
        {
            int randomTrackIndex = Random.Range(0, _allUnlockedTracks.Count);

            while (randomTrackIndex == _previousTrackIndex)
            {
                randomTrackIndex = Random.Range(0, _allUnlockedTracks.Count);
            }

            _currentTrackIndex = randomTrackIndex;
            
            SelectTrack();
        }
    }

    public interface ITrackSelectionController
    {
        public Action OnTrackPlayRequested { get; set; }
        public Action OnTrackStartPlaying { get; set; }
        public Action OnTrackStopRequested { get; set; }
        public TrackDataSO SelectedTrackData { get; }
        public bool IsPlayingTrack { get; }
        public bool IsPlayingSameTrack { get; }
        public void SelectRandomTrack();
        public void PlaySelectedTrack();
        public void StopPlayingTrack();
    }
}