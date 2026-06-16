using UnityEngine;

namespace DVDNights
{
    public class TVVolumeController : MonoBehaviour, ITVVolumeController
    {
        public void VolumeUp()
        {
            throw new System.NotImplementedException();
        }

        public void VolumeDown()
        {
            throw new System.NotImplementedException();
        }

        public void SetVolume(int volume)
        {
            throw new System.NotImplementedException();
        }
    }

    public interface ITVVolumeController
    {
        public void VolumeUp();
        public void VolumeDown();
        public void SetVolume(int volume);
    }
}