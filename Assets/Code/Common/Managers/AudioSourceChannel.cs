using DG.Tweening;
using UnityEngine;
using UnityEngine.Audio;

public class AudioSourceChannel : MonoBehaviour
{
    [Header("Channels-AudioSources")] 
    [SerializeField] private AudioChannelType channelType;

    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource ostSource;
    [SerializeField] private AudioMixer mixer;

    [SerializeField, Range(0f, 100f)] private float channelVolume = 50f;

    
    [Header("Distortion-Configuration")]
    [SerializeField] private float hauntedDistortion = 70f;
    [SerializeField] private float hauntedLowPass = 2500f;
    [SerializeField] private string distortionParameter = "Distortion";
    [SerializeField] private string lowPassParameter = "LowPassCutoff";
    
    public float ChannelVolume => channelVolume;
    
    private float _previousDistortion;
    private float _previousLowPass;
    private Tween _distortionTween;
    
    private Tween _fadeTween;
    private AudioClip _previousOSTClip;
    private float _previousOstVolume;
    
    private bool _isDistorted;

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
            _previousOstVolume = finalVolume;
            _fadeTween = ostSource.DOFade(finalVolume, 2f);
            return;
        }

        ostSource.DOFade(0f, 3f).OnComplete(() =>
        {
            ostSource.Stop();
            ostSource.clip = newClip;
            ostSource.pitch = pitch;
            ostSource.volume = finalVolume;
            _previousOstVolume = finalVolume;
            ostSource.loop = loop;
            ostSource.Play();
            _fadeTween = ostSource.DOFade(finalVolume, 3f);
        });
    }
    
    public void PauseOST(float fadeDuration = 2f)
    {
        _fadeTween?.Kill();
        
        _fadeTween = ostSource.DOFade(0f, fadeDuration).OnComplete(() =>
        {
            ostSource.Pause();
        });
    }

    public void ResumeOST()
    {
        _fadeTween?.Kill();
        ostSource.UnPause();
        _fadeTween = ostSource.DOFade(_previousOstVolume, 2f);
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

    public void StopSFX()
    {
        sfxSource.Stop();
    }

    public void PlayDistortedAudio()
    {
        if (_isDistorted)
        {
            return;
        }

        _isDistorted = true;

        mixer.GetFloat(distortionParameter, out _previousDistortion);
        mixer.GetFloat(lowPassParameter, out _previousLowPass);

        _distortionTween?.Kill();

        float startDistortion = _previousDistortion;
        float startLowPass = _previousLowPass;
        float randomDuration = Random.Range(1f, 3f);
        
        _distortionTween = DOVirtual.Float(0f, 1f, randomDuration, t =>
        {
            float distortion = Mathf.Lerp(
                startDistortion,
                hauntedDistortion,
                t);

            float lowPass = Mathf.Lerp(
                startLowPass,
                hauntedLowPass,
                t);

            mixer.SetFloat(distortionParameter, distortion);
            mixer.SetFloat(lowPassParameter, lowPass);

            // subtle pitch wobble
            ostSource.pitch = 1f + Mathf.Sin(Time.time * 4f) * 0.02f;
            randomDuration = Random.Range(1f, 3f);
        }).SetLoops(-1);
    }

    public void ClearDistortedAudio()
    {
        if (!_isDistorted)
        {
            return;
        }

        _isDistorted = false;
        _distortionTween?.Kill();
        mixer.ClearFloat(distortionParameter);
        mixer.ClearFloat(lowPassParameter);
        ostSource.pitch = 1f;
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