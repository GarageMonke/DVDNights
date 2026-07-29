using System;
using UnityEngine;

namespace CorePatterns.Managers
{
    public class AudioManager : Manager<AudioManager>
    {
        [Header("Channels")] [SerializeField] private AudioSourceChannel tvAudioChannel;
        [SerializeField] private AudioSourceChannel turntableAudioChannel;
        [SerializeField] private AudioSourceChannel stormAudioChannel;
        [SerializeField] private AudioSourceChannel nonDiegeticAudioChannel;
        [SerializeField] private AudioSourceChannel diegeticAudioChannel;
        [SerializeField] private AudioSourceChannel doorAudioChannel;
        [SerializeField] private AudioSourceChannel phoneAudioChannel;
        [SerializeField] private AudioSourceChannel heartbeatAudioChannel;
        [SerializeField] private AudioSourceChannel breathingAudioChannel;

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


        public void StopSFX(AudioChannelType channelType)
        {
            AudioSourceChannel channel = GetAudioSourceChannelByType(channelType);
            channel.StopSFX();
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
        
        public void DistortAudio(AudioChannelType channelType)
        {
            AudioSourceChannel channel = GetAudioSourceChannelByType(channelType);
            channel.PlayDistortedAudio();
        }

        public void ClearDistortedAudio(AudioChannelType channelType)
        {
            AudioSourceChannel channel = GetAudioSourceChannelByType(channelType);
            channel.ClearDistortedAudio();
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
                case AudioChannelType.DOOR:
                    return doorAudioChannel;
                case AudioChannelType.PHONE:
                    return phoneAudioChannel;
                case AudioChannelType.HEARTBEAT:
                    return heartbeatAudioChannel;
                case AudioChannelType.BREATHING:
                    return breathingAudioChannel;
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
    DIEGETIC,
    DOOR,
    PHONE,
    HEARTBEAT,
    BREATHING
}