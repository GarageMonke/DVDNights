using UnityEngine;
using UnityEngine.UI;

namespace DVDNights
{
    public class FillView : MonoBehaviour, IFillView
    {
        [Header("References")] 
        [SerializeField] private Image fillImage;
        
        private float _totalFill;
        private float _currentFill;

        public float CurrentFill => _currentFill;

        public void InitializeView(float totalFill)
        {
            _totalFill = totalFill;
        }

        public void UpdateFill(float fillAmount)
        {
            if(fillAmount <= 0) return;
            _currentFill = fillAmount;
            fillImage.fillAmount = _currentFill / _totalFill;
        }
    }

    public interface IFillView
    {
        public float CurrentFill { get; }
        public void InitializeView(float totalFill);
        public void UpdateFill(float fillAmount);
    }
}