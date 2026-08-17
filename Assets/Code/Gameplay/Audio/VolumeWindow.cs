using Common;
using TMPro;
using UnityEngine;

namespace DVDNights
{
    public class VolumeWindow : TVWindow, IVolumeWindow
    {
        [Header("References")]
        [SerializeField] private FillView volumeFillView;
        [SerializeField] private TextMeshProUGUI volumeText;
        

        public void SetVolumeLimits(int minVolume, int maxVolume)
        {
            volumeFillView.InitializeView(maxVolume, minVolume);
        }

        public int GetCurrentFill()
        {
            return (int)volumeFillView.CurrentFill;
        }

        public void VolumeUp()
        {
            volumeFillView.UpdateFill(volumeFillView.CurrentFill + 1);
            volumeText.text = volumeFillView.CurrentFill.ToString();
            SetVolume(GetCurrentFill());
        }

        public void VolumeDown()
        {
            volumeFillView.UpdateFill(volumeFillView.CurrentFill - 1);
            volumeText.text = volumeFillView.CurrentFill.ToString();
            SetVolume(GetCurrentFill());
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