using Common;
using CorePatterns.Managers;
using UnityEngine;

namespace DVDNights
{
    public class MouseLayoutWindow : Window, IMouseLayoutView
    {
        [SerializeField] private GameObject mouseLeftClick;
        [SerializeField] private GameObject mouseWheelClick;
        [SerializeField] private GameObject mouseRightClick;
        
        public void DisplayRegularLayout()
        {
            HideAll();
            mouseLeftClick.SetActive(true);
            mouseRightClick.SetActive(true);
        }

        public void DisplayInspectionLayout()
        { 
            HideAll();
           DisplayRegularLayout();
           mouseWheelClick.SetActive(true);
        }

        private void HideAll()
        {
            mouseLeftClick.SetActive(false);
            mouseWheelClick.SetActive(false);
            mouseRightClick.SetActive(false);
        }

        public override void Close()
        {
            WindowManager.Instance.CloseWindow<MouseLayoutWindow>();
        }
    }

    public interface IMouseLayoutView : IWindow
    {
        public void DisplayRegularLayout();
        public void DisplayInspectionLayout();
    }
}