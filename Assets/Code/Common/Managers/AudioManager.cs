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
        
        public void PlayOST(AudioClip newClip, float volume = 1f, bool loop = false)
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
                ostSource.volume = volume;
                ostSource.loop = loop;
                ostSource.volume = 0f;
                ostSource.Play();

                _fadeTween = ostSource.DOFade(1f, 2f);
                return;
            }
            
            ostSource.DOFade(0f, 3f).OnComplete(() =>
            {
                ostSource.Stop();
                ostSource.clip = newClip;
                ostSource.volume = volume;
                ostSource.loop = loop;
                ostSource.Play();
                ostSource.DOFade(1f, 3f);
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
    }
}