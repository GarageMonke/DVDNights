using DG.Tweening;
using UnityEngine;

public class AudioSourceChannel : MonoBehaviour
{
    [Header("Channels-AudioSources")] 
    [SerializeField] private AudioChannelType channelType;

    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource ostSource;

    [SerializeField, Range(0f, 100f)] private float channelVolume = 50f;

    public float ChannelVolume => channelVolume;
    
    private Tween _fadeTween;
    private AudioClip _previousOSTClip;
    private float _previousOstVolume;

    private void Awake()
    {
        if (!sfxSource)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.playOnAwake = false;
        }

        if (!ostSource)
        {
            ostSource = gameObject.AddComponent<AudioSource>();
            ostSource.playOnAwake = false;
        }
    }
    
    /// Converts a 0–100 slider value to a linear amplitude multiplier
    /// using a logarithmic (perceptual) curve.
    ///   0   → silence  (−20 dB)
    ///   100 → unity    (  0 dB)
    public float SliderToLinear(float volume)
    {
        volume = Mathf.Clamp(volume, 0f, 100f);
        if (volume <= 0f)
        {
            return 0f;
        }                       

        float dB = -40f * (1f - volume / 100f);
        return Mathf.Pow(10f, dB / 20f);              
    }
    
    /// Update the channel's slider value at runtime (e.g. from a settings UI).
    public void SetChannelVolume(float newSliderValue)
    {
        channelVolume = Mathf.Clamp(newSliderValue, 0f, 100f);
    }

    /// Result: callerVolume is a fraction of the channel's current volume.
    private float ResolveVolume(float callerVolume)
    {
        return Mathf.Clamp01(callerVolume) * SliderToLinear(channelVolume);
    }

    public void PlaySFX(AudioClip clip, float volume = 1f, float pitch = 1f, bool randomizePitch = false)
    {
        if (!clip)
        {
            return;
        }

        float finalVolume = ResolveVolume(volume);
        sfxSource.pitch = randomizePitch ? Random.Range(pitch - pitch * 0.1f, pitch + pitch * 0.1f) : pitch;
        sfxSource.PlayOneShot(clip, finalVolume);
    }

    public void PlayOST(AudioClip newClip, float volume = 1f, bool loop = false, float pitch = 1f)
    {
        if (!newClip)
        {
            return;
        }

        if (newClip == ostSource.clip)
        {
            return;
        }

        _fadeTween?.Kill();

        float finalVolume = ResolveVolume(volume);
        
        if (!ostSource.isPlaying || !ostSource.clip)
        {
            ostSource.clip = newClip;
            ostSource.pitch = pitch;
            ostSource.loop = loop;
            ostSource.volume = 0f;
            ostSource.Play();

            _fadeTween = ostSource.DOFade(finalVolume, 2f);
            return;
        }

        ostSource.DOFade(0f, 3f).OnComplete(() =>
        {
            ostSource.Stop();
            ostSource.clip = newClip;
            ostSource.pitch = pitch;
            ostSource.volume = finalVolume;
            ostSource.loop = loop;
            ostSource.Play();
            _fadeTween = ostSource.DOFade(finalVolume, 3f);
        });
    }
    
    public void PauseOST(float fadeDuration = 2f)
    {
        _fadeTween?.Kill();
        
        ostSource.DOFade(0f, fadeDuration).OnComplete(() =>
        {
            ostSource.Pause();
        });
    }

    public void ResumeOST()
    {
        ostSource.UnPause();
        ostSource.DOFade(ostSource.volume, 2f);
    }

    public void StopOST(bool fadeOut = true)
    {
        _fadeTween?.Kill();

        if (ostSource.isPlaying)
        {
            if (fadeOut)
            {
                _fadeTween = ostSource.DOFade(0f, 0.5f).OnComplete(() =>
                {
                    ostSource.Stop();
                    ostSource.clip = null;
                });
            }
            else
            {
                ostSource.Stop();
                ostSource.clip = null;
            }
        }
    }

    public void PlayPreview(AudioClip previewClip, float volume = 1f)
    {
        if (!previewClip)
        {
            return;
        }

        float finalVolume = ResolveVolume(volume);
        
        _fadeTween?.Kill();

        _fadeTween = ostSource.DOFade(0f, 1f).OnComplete(() =>
        {
            ostSource.Stop();
            ostSource.clip = previewClip;
            ostSource.volume = finalVolume;
            ostSource.Play();

            _fadeTween = ostSource.DOFade(finalVolume, 1f).OnComplete(() =>
            {
                DOVirtual.DelayedCall(25f,
                    () => { _fadeTween = ostSource.DOFade(0f, 1f).OnComplete(() => { ostSource.Stop(); }); });
            });
        });
    }
}