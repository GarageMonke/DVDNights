using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace Code.Common.Localization
{

    public static class LocalizationLoader
    {
        private const string RemoteUrl = "https://docs.google.com/spreadsheets/d/1JqKTO1BvDcZgUVxXj9Uo0MK69nJ5KAgWbldByLXHJf0/export?format=csv";

        private const string ResourcesPath = "Localization/strings"; // Resources/Localization/strings.csv (bundled fallback)

        private const string CacheFileName = "localization_cache.csv";
        private const int TimeoutSeconds = 5;

        private static string CachePath => Path.Combine(Application.persistentDataPath, CacheFileName);

        public enum Source
        {
            Remote,
            Cache,
            Bundled
        }

        public static IEnumerator Load(Action<Dictionary<string, Dictionary<string, string>>, Source> onComplete)
        {
            //Try remote fetch first (most up-to-date)
            using UnityWebRequest request = UnityWebRequest.Get(RemoteUrl);
            request.timeout = TimeoutSeconds;
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string csvText = request.downloadHandler.text;
                var table = CsvParser.Parse(csvText);

                if (IsValidTable(table))
                {
                    TryWriteCache(csvText);
                    onComplete?.Invoke(table, Source.Remote);
                    yield break;
                }
            }

            Debug.LogWarning($"Localization: remote fetch failed ({request.error}), falling back to cache/bundled.");

            //Try local cache (last successful remote fetch, works offline after first launch)
            if (TryReadCache(out string cachedCsv))
            {
                var table = CsvParser.Parse(cachedCsv);
                if (IsValidTable(table))
                {
                    onComplete?.Invoke(table, Source.Cache);
                    yield break;
                }
            }

            //Fall back to bundled CSV shipped with the build (always guaranteed to exist)
            TextAsset bundled = Resources.Load<TextAsset>(ResourcesPath);
            if (!bundled)
            {
                Debug.LogError($"Localization: no bundled fallback found at Resources/{ResourcesPath}.csv — check the path.");
                onComplete?.Invoke(new Dictionary<string, Dictionary<string, string>>(), Source.Bundled);
                yield break;
            }

            var bundledTable = CsvParser.Parse(bundled.text);
            onComplete?.Invoke(bundledTable, Source.Bundled);
        }

        private static bool IsValidTable(Dictionary<string, Dictionary<string, string>> table)
        {
            if (table == null || table.Count == 0) return false;

            foreach (var kvp in table)
            {
                if (kvp.Value.Count == 0)
                {
                    return false;
                }
            }

            return true;
        }

        private static void TryWriteCache(string csvText)
        {
            try
            {
                File.WriteAllText(CachePath, csvText);
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Localization: failed to write cache: {e.Message}");
            }
        }

        private static bool TryReadCache(out string csvText)
        {
            csvText = null;
            try
            {
                if (!File.Exists(CachePath)) return false;
                csvText = File.ReadAllText(CachePath);
                return !string.IsNullOrWhiteSpace(csvText);
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Localization: failed to read cache: {e.Message}");
                return false;
            }
        }
    }
}