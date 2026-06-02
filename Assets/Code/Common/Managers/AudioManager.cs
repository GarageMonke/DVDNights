using DG.Tweening;
using UnityEngine;

namespace CorePatterns.Managers
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [Header("Audio Source")]
        [SerializeField] private AudioSource sfxSource;
        [SerializeField] private AudioSource ostSource;
        
        private Tween _fadeTween;
        private AudioClip _previousOSTClip;
        private float _previousOstVolume;
        
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            
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
                ostSource.DOFade(volume, 3f);
            });
        }

        public void StopOST()
        {
            _fadeTween?.Kill();
            
            if (ostSource.isPlaying)
            {
                _fadeTween = ostSource.DOFade(0f, 0.5f).OnComplete(() =>
                {
                    ostSource.Stop();
                    ostSource.clip = null;
                });
            }
        }

        public void PlayPreview(AudioClip previewClip, float volume = 1f)
        {
            if (!previewClip)
            {
                return;
            }

            _previousOSTClip = ostSource.clip;
            _previousOstVolume = ostSource.volume;

            _fadeTween?.Kill();
            
            _fadeTween = ostSource.DOFade(0f, 1f).OnComplete(() =>
            {
                ostSource.Stop();
                ostSource.clip = previewClip;
                ostSource.volume = volume;
                ostSource.Play();
                
                _fadeTween = ostSource.DOFade(volume, 1f).OnComplete(() =>
                {
                    // 4. Wait 10 seconds while preview plays
                    DOVirtual.DelayedCall(25f, () =>
                    {
                        _fadeTween = ostSource.DOFade(0f, 1f).OnComplete(() =>
                        {
                            ostSource.Stop();
                            
                            if (_previousOSTClip)
                            {
                                ostSource.clip = _previousOSTClip;
                                ostSource.volume = 0f;
                                ostSource.Play();
                                _fadeTween = ostSource.DOFade(_previousOstVolume, 1f);
                            }
                        });
                    });
                });
            });
        }
    }
}