using Common;
using UnityEngine;

namespace DVDNights
{
    public abstract class TVWindow : MonoBehaviour, IWindow
    {
        public bool IsDisplaying => _isDisplaying;
        private bool _isDisplaying;
        
        public virtual void Display()
        {
            _isDisplaying = true;
           gameObject.SetActive(true);
        }

        public virtual void Hide()
        {
            _isDisplaying = false;
            gameObject.SetActive(false);
        }
    }
}