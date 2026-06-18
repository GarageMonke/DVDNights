using System;
using UnityEngine;

namespace CorePatterns.Managers
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [Header("Channels")] [SerializeField] private AudioSourceChannel tvAudioChannel;
        [SerializeField] private AudioSourceChannel turntableAudioChannel;
        [SerializeField] private AudioSourceChannel stormAudioChannel;
        [SerializeField] private AudioSourceChannel nonDiegeticAudioChannel;
        [SerializeField] private AudioSourceChannel diegeticAudioChannel;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        public void PlaySFX(AudioChannelType channelType, AudioClip clip, float volume = 1f, float pitch = 1f,
            bool randomizePitch = false)
        {
            AudioSourceChannel channel = GetAudioSourceChannelByType(channelType);
            channel.PlaySFX(clip, volume, pitch, randomizePitch);
        }

        public void PlayOST(AudioChannelType channelType, AudioClip newClip, float volume = 1f, bool loop = false,
            float pitch = 1f)
        {
            AudioSourceChannel channel = GetAudioSourceChannelByType(channelType);
            channel.PlayOST(newClip, volume, loop, pitch);
        }

        public void StopOST(AudioChannelType channelType, bool fadeOut = true)
        {
            AudioSourceChannel channel = GetAudioSourceChannelByType(channelType);
            channel.StopOST(fadeOut);
        }
        
        public void PauseOST(AudioChannelType channelType, float fadeDuration = 2f)
        {
            AudioSourceChannel channel = GetAudioSourceChannelByType(channelType);
            channel.PauseOST(fadeDuration);
        }

        public void ResumeOST(AudioChannelType channelType)
        {
            AudioSourceChannel channel = GetAudioSourceChannelByType(channelType);
            channel.ResumeOST();
        }

        public void PlayPreview(AudioChannelType channelType, AudioClip previewClip, float volume = 1f)
        {
            AudioSourceChannel channel = GetAudioSourceChannelByType(channelType);
            channel.PlayPreview(previewClip, volume);
        }

        public void SetChannelVolume(AudioChannelType channelType, float volumeValue)
        {
            AudioSourceChannel channel = GetAudioSourceChannelByType(channelType);
            channel.SetChannelVolume(volumeValue);
        }

        public float GetChannelVolume(AudioChannelType channelType)
        {
            AudioSourceChannel channel = GetAudioSourceChannelByType(channelType);
            return channel.ChannelVolume;
        }

        private AudioSourceChannel GetAudioSourceChannelByType(AudioChannelType audioChannelType)
        {
            switch (audioChannelType)
            {
                case AudioChannelType.TV:
                    return tvAudioChannel;
                case AudioChannelType.TURNTABLE:
                    return turntableAudioChannel;
                case AudioChannelType.STORM:
                    return stormAudioChannel;
                case AudioChannelType.NONDIEGETIC:
                    return nonDiegeticAudioChannel;
                case AudioChannelType.DIEGETIC:
                    return diegeticAudioChannel;
                default:
                    return nonDiegeticAudioChannel;
            }
        }
    }
}

public enum AudioChannelType
{
    TV,
    TURNTABLE,
    STORM,
    NONDIEGETIC,
    DIEGETIC
}