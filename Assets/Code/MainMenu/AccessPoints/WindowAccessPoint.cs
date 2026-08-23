using System;
using CorePatterns.Managers;
using UnityEngine;
using UnityEngine.UI;
using Window = Common.Window;

namespace Rulebound
{
    public abstract class WindowAccessPoint<T> : AccessPoint where T : Window
    {
        [Header("UI-AccessPoint")]
        [SerializeField] private bool openInContainer;
        [SerializeField] private Button accessPointButton;

        public Action OnWindowAccessed;
        
        private void Awake()
        {
            if (accessPointButton)
            {
                accessPointButton.onClick.AddListener(Access);
            }
        }

        public override void Access()
        {
            OnWindowAccessed?.Invoke();
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