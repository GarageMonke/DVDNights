using System;
using UnityEngine;
using UnityEngine.UI;

namespace Common
{
    public class Window : MonoBehaviour, IWindow
    {
        [Header("Close-Button")]
        [SerializeField] private Button closeButton;
        
        private bool _isDisplaying;
        public bool IsDisplaying => _isDisplaying;

        protected virtual void Awake()
        {
            if (closeButton)
            {
                closeButton.onClick.AddListener(Hide);
            }
        }

        public virtual void Display()
        {
            gameObject.SetActive(true);
            _isDisplaying = true;
        }

        public virtual void Hide()
        {
            Destroy(gameObject);
        }
    }
    
    public interface IWindow
    {
        public bool IsDisplaying { get; }
        public void Display();
        public void Hide();
    }
}