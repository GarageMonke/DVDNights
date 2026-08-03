using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using Code.Common.Database;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

namespace CorePatterns.Managers
{
    public class WindowManager : Manager<WindowManager>
    {
        [Header("References")] 
        [SerializeField] private Camera windowCamera;
        [SerializeField] private Volume windowVolume;
        
        [Header("Windows")]
        [SerializeField] private WindowDatabase windowDatabase;

        private readonly Dictionary<Type, Window> _windowPrefabs = new();
        private Dictionary<Type, WindowEntry> _openedWindows = new();
        
        private Canvas _containerCanvas;

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

        public T OpenWindow<T>(GameObject source, bool openInContainer = false) where T : Window
        {
            Type type = typeof(T);

            if (!_windowPrefabs.TryGetValue(type, out Window prefab))
            {
                Debug.LogError($"Window {type.Name} was not found");
                return null;
            }
            
            Canvas canvas = openInContainer ? _containerCanvas : FindCanvasInHierarchy(source);
            
            if (!canvas)
            {
                Debug.LogError("No Canvas found.");
                return null;
            }

            T instance = Instantiate(prefab, canvas.transform) as T;
            
            _openedWindows.TryAdd(type, new WindowEntry(instance, openInContainer));
            
            RefreshOverlayState();

            instance?.Display();

            return instance;
        }
        
        public void CloseWindow<T>() where T : Window
        {
            _openedWindows.TryGetValue(typeof(T), out WindowEntry windowEntry);

            if (windowEntry != null)
            {
                _openedWindows.Remove(typeof(T));
                Destroy(windowEntry.Window.gameObject);
            }
            
            RefreshOverlayState();
        }
        
        
        public void SetContainerCanvas(Canvas canvas)
        {
            _containerCanvas = canvas;
        }

        private Canvas FindCanvasInHierarchy(GameObject source)
        {
            Scene scene = source.scene;
            return scene.GetRootGameObjects().SelectMany(go => go.GetComponentsInChildren<Canvas>(true)).FirstOrDefault();
        }

        private void RefreshOverlayState()
        {
            bool shouldEnable = _openedWindows.Values.Any(window => window.OpenInContainer);

            windowCamera.enabled = shouldEnable;
            windowVolume.enabled = shouldEnable;
        }
    }
}