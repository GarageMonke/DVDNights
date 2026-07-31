using System;
using System.Collections.Generic;
using Code.Common.Localization;
using UnityEngine;

namespace CorePatterns.Managers
{
    public class LocalizationManager : Manager<LocalizationManager>
    {
        public event System.Action OnLanguageChanged;
        public event System.Action OnLocalizationReady;

        public bool IsReady { get; private set; }
        public string CurrentLanguage { get; private set; } = "en";
        public const string FallbackLanguage = "en";

        private Dictionary<string, Dictionary<string, string>> allLanguages = new();
        private Dictionary<string, string> currentTable = new();

        private void Start()
        {
            StartCoroutine(LocalizationLoader.Load(OnTablesLoaded));
        }

        public List<string> GetAvailableLanguages() => new(allLanguages.Keys);
        
        private void OnTablesLoaded(Dictionary<string, Dictionary<string, string>> tables, LocalizationLoader.Source source)
        {
            allLanguages = tables;
            IsReady = true;

            Debug.Log($"Localization loaded from: {source}");

            SetLanguage(GetSavedOrSystemLanguage());
            OnLocalizationReady?.Invoke();
        }

        public void SetLanguage(string languageCode)
        {
            if (!allLanguages.ContainsKey(languageCode))
            {
                Debug.LogWarning($"Localization: '{languageCode}' not available, falling back to '{FallbackLanguage}'.");
                languageCode = FallbackLanguage;
            }

            CurrentLanguage = languageCode;
            currentTable = allLanguages.TryGetValue(languageCode, out var table)
                ? table
                : new Dictionary<string, string>();

            PlayerPrefs.SetString("language", languageCode);
            OnLanguageChanged?.Invoke();
        }

        public string GetLocalizedText(string key)
        {
            if (currentTable.TryGetValue(key, out string value) && !string.IsNullOrEmpty(value))
                return value;

            if (CurrentLanguage != FallbackLanguage
                && allLanguages.TryGetValue(FallbackLanguage, out var fallbackTable)
                && fallbackTable.TryGetValue(key, out string fallback)
                && !string.IsNullOrEmpty(fallback))
            {
                return fallback;
            }

            Debug.LogWarning($"Missing localization key: {key}");
            return $"#{key}#";
        }

        public string GetLocalizedText(string key, params object[] args)
        {
            string raw = GetLocalizedText(key);
            
            try
            {
                return string.Format(raw, args);
            }
            catch
            {
                return raw;
            }
        }

        private string GetSavedOrSystemLanguage()
        {
            if (SettingsManager.Instance.Current != null)
            {
                string saved = SettingsManager.Instance.Current.languageCode;
                
                if (allLanguages.ContainsKey(saved))
                {
                    return saved;
                }
            }

            string systemLang = Application.systemLanguage switch
            {
                SystemLanguage.Spanish => "es",
                SystemLanguage.French => "fr",
                SystemLanguage.German => "de",
                SystemLanguage.Japanese => "ja",
                _ => FallbackLanguage
            };

            return allLanguages.ContainsKey(systemLang) ? systemLang : FallbackLanguage;
        }
    }
}