using CorePatterns.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace Common
{
    public abstract class Window : MonoBehaviour, IWindow
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
            _isDisplaying = false;
            Close();
        }

        public abstract void Close();
    }
    
    public interface IWindow
    {
        public bool IsDisplaying { get; }
        public void Display();
        public void Hide();
    }
}