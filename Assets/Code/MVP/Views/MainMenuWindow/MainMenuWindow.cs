using Common;
using CorePatterns.Managers;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Rulebound
{
    public class MainMenuWindow : Window
    {
        [Header("References")]
        [SerializeField] private SceneLoader sceneLoader;
        [SerializeField] private FadeInOutBlack fadeInOutBlack;
        
        [Header("UI-References")]
        [SerializeField] private Button newGameButton;
        [SerializeField] private Button galleryButton;
        [SerializeField] private SettingsAccessPoint settingsAccessPoint;
        [SerializeField] private CreditsAccessPoint creditsAccessPoint;
        [SerializeField] private Button exitButton;
        [SerializeField] private Button steamButton;
        
        [Header("Audio-Feedback")]
        [SerializeField] private AudioClip mainMenuAudioClip;
        [SerializeField] private AudioClip startGameAudioClip;
        
        
        protected override void Awake()
        {
            base.Awake();
            newGameButton.onClick.AddListener(NewGame);
            exitButton.onClick.AddListener(ExitGame);;
            steamButton.onClick.AddListener(OpenSteamPage);
        }

        public override void Display()
        {
            base.Display();
            
            fadeInOutBlack.FadeOut(2f, Ease.Linear, null);
            
            if (AudioManager.Instance.GetChannelPlayingOST(AudioChannelType.NONDIEGETIC) == mainMenuAudioClip)
            {
                return;
            }
            
            AudioManager.Instance.PlayOST(AudioChannelType.NONDIEGETIC, mainMenuAudioClip, loop: true, fadeIn: false);
        }

        private void EnableAllMenuAccessPoints()
        {
            settingsAccessPoint.EnableAccessPoint();
        }

        private void DisableAllMenuAccessPoints()
        {
            settingsAccessPoint.DisableAccessPoint();
        }

        public override void Close()
        {
            AudioManager.Instance.StopOST(AudioChannelType.NONDIEGETIC);
            WindowManager.Instance.CloseWindow<MainMenuWindow>();
        }

        private void NewGame()
        {
            fadeInOutBlack.FadeIn(startGameAudioClip.length, Ease.Linear, DisplayLoadingWindow);
            AudioManager.Instance.StopOST(AudioChannelType.NONDIEGETIC);
        }
        
        private void OpenSteamPage()
        {
            Application.OpenURL("");
        }
        
        private void ExitGame()
        {
            Application.Quit();
        }

        private void DisplayLoadingWindow()
        {
            WindowManager.Instance.OpenWindow<LoadingWindow>(gameObject, true);
            sceneLoader.LoadScene();
            Hide();
        }
    }
}