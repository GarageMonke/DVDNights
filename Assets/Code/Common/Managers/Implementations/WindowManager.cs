using Common;
using System;
using System.Collections.Generic;
using Code.Common.Database;
using UnityEngine;

namespace CorePatterns.Managers
{
    public class WindowManager : Manager<WindowManager>
    {
        [Header("Windows")]
        [SerializeField] private WindowDatabase windowDatabase;

        private readonly Dictionary<Type, Window> _windowPrefabs = new();

        protected override void Awake()
        {
            base.Awake();
            RegisterWindows();
        }

        private void RegisterWindows()
        {
            foreach (Window window in windowDatabase.Database)
            {
                Type type = window.GetType();

                if (!_windowPrefabs.TryAdd(type, window))
                {
                    Debug.LogError($"Window {type.Name} already registered");
                }
            }
        }

        public T OpenWindow<T>() where T : Window
        {
            Type type = typeof(T);

            if (!_windowPrefabs.TryGetValue(type, out Window prefab))
            {
                Debug.LogError($"Window {type.Name} was not found");
                return null;
            }

            Canvas canvas = FindFirstObjectByType<Canvas>();

            if (!canvas)
            {
                Debug.LogError("No Canvas found in the scene.");
                return null;
            }

            T instance = Instantiate(prefab, canvas.transform) as T;

            instance?.Display();

            return instance;
        }
    }
}