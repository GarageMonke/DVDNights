using CorePatterns.Managers;
using CorePatterns.ServiceLocator;
using UnityEngine;

namespace DVDNights
{
    public class MouseLayoutController : MonoBehaviour, IMouseLayoutController
    {
        private MouseLayoutWindow _mouseLayoutWindow;

        private void Awake()
        {
            InstallService();
        }

        private void InstallService()
        {
            ServiceLocator.RegisterService<IMouseLayoutController>(this);
        }

        public void DisplayRegularLayout()
        {
            if (!_mouseLayoutWindow)
            {
                _mouseLayoutWindow = WindowManager.Instance.OpenWindow<MouseLayoutWindow>(gameObject, openInContainer: false);
            }

            _mouseLayoutWindow.gameObject.SetActive(true);
            _mouseLayoutWindow.DisplayRegularLayout();
        }

        public void DisplayInspectionLayout()
        {
            _mouseLayoutWindow.gameObject.SetActive(true);
            _mouseLayoutWindow.Display();
            _mouseLayoutWindow.DisplayInspectionLayout();
        }

        public void HideMouseLayout()
        {
            _mouseLayoutWindow.gameObject.SetActive(false);
        }
    }

    public interface IMouseLayoutController
    {
        public void DisplayRegularLayout();
        public void DisplayInspectionLayout();
        public void HideMouseLayout();
    }
}