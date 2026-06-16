using DG.Tweening;
using UnityEngine;

public class AudioSourceChannel : MonoBehaviour
{
    [Header("Channels-AudioSources")] 
    [SerializeField] private AudioChannelType channelType;

    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource ostSource;

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

    public void PlaySFX(AudioClip clip, float volume = 1f, float pitch = 1f, bool randomizePitch = false)
    {
        if (!clip)
        {
            return;
        }

        sfxSource.pitch = randomizePitch ? Random.Range(pitch - pitch * 0.1f, pitch + pitch * 0.1f) : pitch;
        sfxSource.PlayOneShot(clip, volume);
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

        if (!ostSource.isPlaying || !ostSource.clip)
        {
            ostSource.clip = newClip;
            ostSource.pitch = pitch;
            ostSource.loop = loop;
            ostSource.volume = 0f;
            ostSource.Play();

            _fadeTween = ostSource.DOFade(volume, 2f);
            return;
        }

        ostSource.DOFade(0f, 3f).OnComplete(() =>
        {
            ostSource.Stop();
            ostSource.clip = newClip;
            ostSource.pitch = pitch;
            ostSource.volume = volume;
            ostSource.loop = loop;
            ostSource.Play();
            _fadeTween = ostSource.DOFade(volume, 3f);
        });
    }
    
    public void PauseOST()
    {
        _fadeTween?.Kill();
        
        ostSource.DOFade(0f, 2f).OnComplete(() =>
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

        _fadeTween?.Kill();

        _fadeTween = ostSource.DOFade(0f, 1f).OnComplete(() =>
        {
            ostSource.Stop();
            ostSource.clip = previewClip;
            ostSource.volume = volume;
            ostSource.Play();

            _fadeTween = ostSource.DOFade(volume, 1f).OnComplete(() =>
            {
                DOVirtual.DelayedCall(25f,
                    () => { _fadeTween = ostSource.DOFade(0f, 1f).OnComplete(() => { ostSource.Stop(); }); });
            });
        });
    }
}