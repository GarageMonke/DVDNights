using CorePatterns.Managers;
using UnityEngine;
using UnityEngine.UI;
using Window = Common.Window;

namespace Code.MainMenu.AccessPoints
{
    public abstract class WindowAccessPoint<T> : AccessPoint where T : Window
    {
        [Header("UI-AccessPoint")]
        [SerializeField] private bool openInContainer;
        [SerializeField] private Button accessPointButton;

        private void Awake()
        {
            if (accessPointButton)
            {
                accessPointButton.onClick.AddListener(Access);
            }
        }

        public override void Access()
        {
            WindowManager.Instance.OpenWindow<T>(gameObject, openInContainer);
        }

        public void EnableAccessPoint()
        {
            accessPointButton.interactable = true;
        }

        public void DisableAccessPoint()
        {
            accessPointButton.interactable = false;
        }
    }
}