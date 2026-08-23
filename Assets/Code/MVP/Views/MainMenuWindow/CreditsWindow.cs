using Common;
using CorePatterns.Managers;
using UnityEngine;

namespace Rulebound
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
            AudioManager.Instance.StopOST(AudioChannelType.NONDIEGETIC, fadeOut: false);
            AudioManager.Instance.PlayOST(AudioChannelType.NONDIEGETIC, creditsAudioClip);
            scrollView.StartScrolling(creditsAudioClip.length * 0.85f, holdTime: 6f);
            scrollView.OnScrollEnded += Close;
        }

        public override void Close()
        {
            scrollView.OnScrollEnded -= Close;
            AudioManager.Instance.StopOST(AudioChannelType.NONDIEGETIC, fadeOut: false);
            WindowManager.Instance.CloseWindow<CreditsWindow>();
        }
    }
}