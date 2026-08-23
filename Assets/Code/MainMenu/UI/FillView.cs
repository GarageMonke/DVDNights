using UnityEngine;
using UnityEngine.UI;

namespace Rulebound
{
    public class FillView : MonoBehaviour, IFillView
    {
        [Header("References")] 
        [SerializeField] private Image fillImage;
        
        private float _totalFill;
        private float _currentFill;
        private float _minFill;

        public float CurrentFill => _currentFill;

        public void InitializeView(float totalFill, float minFill = 0)
        {
            _minFill = minFill;
            _totalFill = totalFill;
        }

        public void UpdateFill(float fillAmount)
        {
            if (fillAmount <= _minFill)
            {
                fillAmount = _minFill;
            }
            
            if (fillAmount >= _totalFill)
            {
                fillAmount = _totalFill;
            }
            
            _currentFill = fillAmount;
            fillImage.fillAmount = _currentFill / _totalFill;
        }
    }

    public interface IFillView
    {
        public float CurrentFill { get; }
        public void InitializeView(float totalFill, float minFill = 0);
        public void UpdateFill(float fillAmount);
    }
}