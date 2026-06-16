using System;
using CorePatterns.Managers;
using TMPro;
using UnityEngine;

namespace DVDNights
{
    public class VolumeWindow : Window, IVolumeWindow
    {
        [Header("References")]
        [SerializeField] private FillView volumeFillView;
        [SerializeField] private TextMeshProUGUI volumeText;
        
        [Header("Configuration")]
        [SerializeField] private AudioChannelType audioChannelType;

        private void Awake()
        {
            volumeFillView.InitializeView(100);
        }

        public void VolumeUp()
        {
            volumeFillView.UpdateFill(volumeFillView.CurrentFill + 1);
            volumeText.text = volumeFillView.CurrentFill.ToString();
        }

        public void VolumeDown()
        {
            volumeFillView.UpdateFill(volumeFillView.CurrentFill - 1);
            volumeText.text = volumeFillView.CurrentFill.ToString();
        }

        public void SetVolume(int volume)
        {
            volumeFillView.UpdateFill(volume);
        }
    }

    public interface IVolumeWindow : IWindow
    {
        public void VolumeUp();
        public void VolumeDown();
        public void SetVolume(int volume);
    }
}