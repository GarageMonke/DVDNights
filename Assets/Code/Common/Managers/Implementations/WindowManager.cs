using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using Code.Common.Database;
using Rulebound;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

namespace CorePatterns.Managers
{
    public class WindowManager : Manager<WindowManager>
    {
        [Header("References")] 
        [SerializeField] private Camera windowCamera;
        [SerializeField] private Volume windowVolume;
        [SerializeField] private Transform worldTransform;
        [SerializeField] private InputActionSO closeInputActionSO;
        
        [Header("Windows")]
        [SerializeField] private WindowDatabase windowDatabase;

        private readonly Dictionary<Type, Window> _windowPrefabs = new();
        private Dictionary<Type, WindowEntry> _openedWindows = new();
        private readonly List<Type> _windowStack = new();
        
        private Canvas _containerCanvas;
        private GameObject _instantiatedInWorld;
        
        private InputAction _closeInputAction;
        
        public Action OnWindowOpened;
        public Action OnWindowClosed;

        protected override void Awake()
        {
            base.Awake();
            RegisterWindows();
            _closeInputAction = closeInputActionSO.GetInputAction();
            _closeInputAction.performed += ShortcutToCloseTopWindow;
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

            GameObject instantiatedInWorld = null;
            
            if (_openedWindows.TryAdd(type, new WindowEntry(instance, openInContainer)))
            {
               _windowStack.Add(type);
            }
            
            _openedWindows.TryAdd(type, new WindowEntry(instance, openInContainer));
            
            RefreshOverlayState();

            instance?.Display();
            
            OnWindowOpened?.Invoke();

            return instance;
        }
        
        public void CloseWindow<T>() where T : Window
        {
            CloseWindowByType(typeof(T));
        }

        
        private void CloseWindowByType(Type type)
        {
            if (_openedWindows.Remove(type, out WindowEntry windowEntry))
            {
                _windowStack.Remove(type);
                Destroy(windowEntry.Window.gameObject);
            }
            
            RefreshOverlayState();
            
            OnWindowClosed?.Invoke();
        }

        private void ShortcutToCloseTopWindow(InputAction.CallbackContext obj)
        {
            Type topWindowType = GetTopWindowType();

            if (topWindowType == null)
            {
                return;
            }
            
            if (!IsWindowOpen(topWindowType))
            {
                return;
            }
            
            if (CanBeClosedByShortcut(topWindowType))
            {
                CloseTopWindow();
            }
        }
        
        public void CloseTopWindow()
        {
            Type latestOpenedType = GetTopWindowType();
            CloseWindowByType(latestOpenedType);
        }

        private Type GetTopWindowType()
        {
            if (_windowStack.Count == 0)
            {
                return null;
            }
            
            return _windowStack[^1];
        }
        
        public void CloseAllWindows()
        {
            List<Type> windowsInOrder = new(_windowStack);
            windowsInOrder.Reverse();

            foreach (Type type in windowsInOrder)
            {
                CloseWindowByType(type);
            }
        }
        
        public void SetContainerCanvas(Canvas canvas)
        {
            _containerCanvas = canvas;
        }

        public bool CanBeClosedByShortcut(Type windowType)
        {
            _openedWindows.TryGetValue(windowType, out WindowEntry windowEntry);
            return windowEntry != null && windowEntry.Window.CloseByShortcut;
        }

        public bool IsWindowOpen<T>() where T : Window
        {
            _openedWindows.TryGetValue(typeof(T), out WindowEntry windowEntry);
            return windowEntry != null;
        }

        public bool IsWindowOpen(Type type)
        {
            _openedWindows.TryGetValue(type, out WindowEntry windowEntry);
            return windowEntry != null;
        }

        public bool IsWindowOnTop<T>() where T : Window
        {
            Type topWindowType = GetTopWindowType();
            return typeof(T) == topWindowType;
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

        public bool HasOpenedWindows()
        {
            return _openedWindows.Count > 0;
        }
    }
}