using UnityEngine;

namespace Common
{
    public class Window : MonoBehaviour, IWindow
    {
        private bool _isDisplaying;
        public bool IsDisplaying => _isDisplaying;

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