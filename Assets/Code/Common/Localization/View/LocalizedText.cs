using CorePatterns.Managers;

namespace Code.Common.Localization
{
    using UnityEngine;
    using TMPro;

    [RequireComponent(typeof(TMP_Text))]
    public class LocalizedText : MonoBehaviour
    {
        [SerializeField] private string key;
        
        private TMP_Text _label;

        private void Awake()
        {
            _label = GetComponent<TMP_Text>();
        }

        private void OnEnable()
        {
            if (!LocalizationManager.Instance) return;

            LocalizationManager.Instance.OnLanguageChanged += Refresh;
            LocalizationManager.Instance.OnLocalizationReady += Refresh;

            if (LocalizationManager.Instance.IsReady) Refresh();
        }

        private void OnDisable()
        {
            if (!LocalizationManager.Instance) return;

            LocalizationManager.Instance.OnLanguageChanged -= Refresh;
            LocalizationManager.Instance.OnLocalizationReady -= Refresh;
        }

        private void Refresh()
        {
            if (string.IsNullOrEmpty(key))
            {
                return;
            }
            
            _label.text = LocalizationManager.Instance.GetLocalizedText(key);
        }
    }
}