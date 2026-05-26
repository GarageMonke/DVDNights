using UnityEngine;

namespace DVDNights
{
    public class Window : MonoBehaviour, IWindow
    {
        public virtual void Display()
        {
            gameObject.SetActive(true);
        }

        public virtual void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}