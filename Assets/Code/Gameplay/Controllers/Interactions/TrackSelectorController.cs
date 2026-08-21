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
        
        private bool _shouldPlayFromStart;
        private bool _isPlayingTrack;
        private TrackDataSO _selectedTrackData;
        private TrackDataSO _previousTrackData;
        
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

            foreach (TrackDataSO trackData in trackDataProvider.GetAllElements())
            {
                if (trackData.IsUnlocked)
                {
                    _allUnlockedTracks.Add(trackData);
                }
            }
        }

        public void PlaySelectedTrack()
        {
            _isPlayingTrack = true;
            
            if (_shouldPlayFromStart)
            {
                AudioManager.Instance.PlayOST(AudioChannelType.TURNTABLE, _selectedTrackData.TrackAudioClip);
                OnTrackStartPlaying?.Invoke();
                return;
            }
            
            AudioManager.Instance.ResumeOST(AudioChannelType.TURNTABLE);
            OnTrackStartPlaying?.Invoke();
        }

        public void StopPlayingTrack()
        {
            PauseSelectedTrack();
            _previousTrackData = _selectedTrackData;
            _selectedTrackData = null;
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

            TrackDataSO randomTrackData = _allUnlockedTracks[randomTrackIndex];
            
            while (randomTrackData == _previousTrackData)
            {
                randomTrackIndex = Random.Range(0, _allUnlockedTracks.Count);
                randomTrackData = _allUnlockedTracks[randomTrackIndex];
            }
            
            _selectedTrackData = randomTrackData;
            
            _shouldPlayFromStart = true;
            OnTrackPlayRequested?.Invoke();
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