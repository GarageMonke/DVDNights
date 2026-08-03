using System;
using CorePatterns.Managers;
using UnityEngine;

namespace Code.Common.Feedback
{
    public class OSTPlayer : MonoBehaviour
    {
        [Header("OST")]
        [SerializeField] private bool playOnStart;
        [SerializeField] private AudioClip ostAudioClip;


        private void Start()
        {
            if (!playOnStart)
            {
                return;
            }
            
            PlayOST();
        }

        public void PlayOST()
        {
            AudioManager.Instance.PlayOST(AudioChannelType.NONDIEGETIC, ostAudioClip);
        }

        private void OnDestroy()
        {
            AudioManager.Instance.StopOST(AudioChannelType.NONDIEGETIC);
        }
    }
}