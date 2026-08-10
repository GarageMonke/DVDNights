using System;
using Code.MainMenu.AccessPoints;
using Common;
using CorePatterns.Managers;
using DG.Tweening;
using DVDNights;
using UnityEngine;
using UnityEngine.UI;

namespace Code.MVP
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
            creditsAccessPoint.OnWindowAccessed += Hide;
            settingsAccessPoint.OnWindowAccessed += Hide;
        }

        private void Start()
        {
            AudioManager.Instance.PlayOST(AudioChannelType.NONDIEGETIC, mainMenuAudioClip, loop: true);
            fadeInOutBlack.FadeOut(2f, Ease.Linear, null);
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

        private void OnDestroy()
        {
            creditsAccessPoint.OnWindowAccessed -= Hide;
            settingsAccessPoint.OnWindowAccessed -= Hide;
        }
    }
}