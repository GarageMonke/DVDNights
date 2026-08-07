using Common;
using CorePatterns.Managers;
using DVDNights;
using UnityEngine;

namespace Code.MVP
{
    public class CreditsWindow : Window
    {
        [Header("Reference")]
        [SerializeField] private AutomaticScrollView scrollView;
        
        [Header("Audio-Feedback")]
        [SerializeField] private AudioClip creditsAudioClip;

        public override void Display()
        {
            base.Display();
            AudioManager.Instance.PlayOST(AudioChannelType.NONDIEGETIC, creditsAudioClip);
            scrollView.StartScrolling(creditsAudioClip.length, holdTime: 2f);
            scrollView.OnScrollEnded += Close;
        }

        public override void Close()
        {
            scrollView.OnScrollEnded -= Close;
            WindowManager.Instance.CloseWindow<CreditsWindow>();
        }
    }
}