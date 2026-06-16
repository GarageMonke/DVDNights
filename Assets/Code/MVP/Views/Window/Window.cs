using UnityEngine;

namespace DVDNights
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
            gameObject.SetActive(false);
            _isDisplaying = false;
        }
    }
    
    public interface IWindow
    {
        public bool IsDisplaying { get; }
        public void Display();
        public void Hide();
    }
}