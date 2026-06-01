using UnityEngine;

namespace DVDNights
{
    public class MouseLayoutView : MonoBehaviour, IMouseLayoutView
    {
        [SerializeField] private GameObject mouseLeftClick;
        [SerializeField] private GameObject mouseWheelClick;
        [SerializeField] private GameObject mouseRightClick;
        
        public void ShowRegularLayout()
        {
            HideAll();
            mouseLeftClick.SetActive(true);
            mouseRightClick.SetActive(true);
        }

        public void ShowZoomLayout()
        { 
            HideAll();
           ShowRegularLayout();
           mouseWheelClick.SetActive(true);
        }

        private void HideAll()
        {
            mouseLeftClick.SetActive(false);
            mouseWheelClick.SetActive(false);
            mouseRightClick.SetActive(false);
        }
    }

    public interface IMouseLayoutView
    {
        public void ShowRegularLayout();
        public void ShowZoomLayout();
    }
}