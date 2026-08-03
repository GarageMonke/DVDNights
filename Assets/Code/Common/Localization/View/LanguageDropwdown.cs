using System;
using CorePatterns.Managers;

namespace Code.Common.Localization
{
    using System.Collections.Generic;
    using UnityEngine;
    using TMPro;

    [RequireComponent(typeof(TMP_Dropdown))]
    public class LanguageDropdown : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TMP_Dropdown dropdown;
        
        private List<string> _languageCodes = new();

        public Action<string> OnLanguageChanged;

        private static readonly Dictionary<string, string> NativeNames = new()
        {
            { "en", "English" },
            { "es", "Español" },
            { "fr", "Français" },
            { "de", "Deutsch" },
            { "ja", "日本語" },
        };

        private void OnEnable()
        {
            if (!LocalizationManager.Instance)
            {
                return;
            }

            if (LocalizationManager.Instance.IsReady)
            {
                Populate();
            }
            else
            {
                LocalizationManager.Instance.OnLocalizationReady += Populate;
            }

            dropdown.onValueChanged.AddListener(OnDropdownChanged);
        }

        private void OnDisable()
        {
            if (LocalizationManager.Instance)
            {
                LocalizationManager.Instance.OnLocalizationReady -= Populate;
            }

            dropdown.onValueChanged.RemoveListener(OnDropdownChanged);
        }

        private void Populate()
        {
            _languageCodes = LocalizationManager.Instance.GetAvailableLanguages();

            List<string> displayNames = new List<string>();
            
            foreach (string code in _languageCodes)
            {
                displayNames.Add(NativeNames.GetValueOrDefault(code, code));
            }

            dropdown.ClearOptions();
            dropdown.AddOptions(displayNames);

            int currentIndex = _languageCodes.IndexOf(LocalizationManager.Instance.CurrentLanguage);
            dropdown.SetValueWithoutNotify(Mathf.Max(currentIndex, 0));
            dropdown.RefreshShownValue();
        }

        private void OnDropdownChanged(int index)
        {
            OnLanguageChanged?.Invoke(_languageCodes[index]);
        }
    }
}