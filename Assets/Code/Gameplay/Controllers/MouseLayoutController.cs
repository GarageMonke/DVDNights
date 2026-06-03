using CorePatterns.ServiceLocator;
using UnityEngine;

namespace DVDNights
{
    public class MouseLayoutController : MonoBehaviour, IMouseLayoutController
    {
        [SerializeField] private MouseLayoutView mouseLayoutView;
        
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
            mouseLayoutView.Display();
            mouseLayoutView.DisplayRegularLayout();
        }

        public void DisplayInspectionLayout()
        {
            mouseLayoutView.Display();
            mouseLayoutView.DisplayInspectionLayout();
        }

        public void HideMouseLayout()
        {
            mouseLayoutView.Hide();
        }
    }

    public interface IMouseLayoutController
    {
        public void DisplayRegularLayout();
        public void DisplayInspectionLayout();
        public void HideMouseLayout();
    }
}