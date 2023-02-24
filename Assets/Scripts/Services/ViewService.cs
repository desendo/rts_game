using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using Locator;
using UnityEngine;
using Views;

namespace Services
{
    public interface IInitializeViewRootService
    {
        void RegisterViewRoot(Transform windowsParent, string id);
    }

    public interface IViewService
    {
        bool BlockGoBack { get; set; }
        bool IsViewOpened { get; }

        TWindow ShowAndPush<TWindow>(Transform parent = null, Action onShowed = null)
            where TWindow : Window;

        TWindow ShowAndPush<TWindow, TData>(TData data, Transform parent = null, Action onShowed = null)
            where TWindow : Window<TData>
            where TData : struct, IEquatable<TData>;

        TWindow Show<TWindow>(Transform parent = null, Action onShowed = null)
            where TWindow : Window;

        TWindow Show<TWindow, TData>(TData data, Transform parent = null, Action onShowed = null)
            where TWindow : Window<TData>
            where TData : struct;

        void ClearHistory();
        void Hide(BaseWindow window, Action onHidden = null);
        void Hide<T>(Action onHidden = null) where T : BaseWindow;
        void GoBack();
    }

    public class ViewService : IViewService, IInitializeViewRootService
    {
        private readonly Dictionary<string, Transform> _viewRoots = new Dictionary<string, Transform>();
        private readonly Dictionary<Type, BaseWindow> _windowPrefabs;
        private readonly List<BaseWindow> _windowsStack;

        public ViewService()
        {
            _windowsStack = new List<BaseWindow>();
            var visualData = Container.Get<DataContainer<VisualData>>().Data;
            _windowPrefabs = new Dictionary<Type, BaseWindow>();
            foreach (var windowPrefab in visualData.WindowPrefabs)
                if (windowPrefab != null)
                    _windowPrefabs.Add(windowPrefab.GetType(), windowPrefab);
        }

        public void RegisterViewRoot(Transform windowsRoot, string id = "")
        {
            if (!_viewRoots.ContainsKey(id))
                _viewRoots.Add(id, windowsRoot);
            else
                _viewRoots[id] = windowsRoot;
        }

        public bool BlockGoBack { get; set; }
        public bool IsViewOpened => _windowsStack.Count > 0;

        public TWindow ShowAndPush<TWindow>(Transform parent = null, Action onShowed = null)
            where TWindow : Window
        {
            var previousWindow = _windowsStack.LastOrDefault();
            var type = typeof(TWindow);
            if (previousWindow is not null && previousWindow && previousWindow.GetType() == type)
            {
                onShowed?.Invoke();
                return (TWindow) previousWindow;
            }

            var window = (TWindow) GetWindow(type, parent);
            window
                .Show(() => onShowed?.Invoke());
            _windowsStack.Add(window);

            return window;
        }

        public TWindow ShowAndPush<TWindow, TData>(TData data, Transform parent = null, Action onShowed = null)
            where TWindow : Window<TData>
            where TData : struct, IEquatable<TData>
        {
            var previousWindow = _windowsStack.LastOrDefault() as TWindow;
            var type = typeof(TWindow);

            if (previousWindow is not null && previousWindow && previousWindow.CurrentData.Equals(data))
            {
                onShowed?.Invoke();

                return previousWindow;
            }

            var window = (TWindow) GetWindow(type, parent);
            window.SetData(data);
            window
                .Show(() => onShowed?.Invoke());
            _windowsStack.Add(window);


            return window;
        }

        public TWindow Show<TWindow>(Transform parent = null, Action onShowed = null)
            where TWindow : Window
        {
            var type = typeof(TWindow);
            var window = (TWindow) GetWindow(type, parent);
            window
                .Show(() => onShowed?.Invoke());


            return window;
        }

        public TWindow Show<TWindow, TData>(TData data, Transform parent = null, Action onShowed = null)
            where TWindow : Window<TData>
            where TData : struct
        {
            var type = typeof(TWindow);
            var window = (TWindow) GetWindow(type, parent);
            window.SetData(data);
            window
                .Show(() => onShowed?.Invoke());


            return window;
        }

        public void ClearHistory()
        {
            foreach (var window in _windowsStack)
                window
                    .Hide(() => RemoveWindow(window));

            _windowsStack.Clear();
        }

        public void Hide(BaseWindow window, Action onHidden = null)
        {
            for (var i = _windowsStack.Count - 1; i >= 0; i--)
            {
                var historyWindow = _windowsStack[i];
                if (historyWindow == window)
                {
                    _windowsStack.Remove(historyWindow);
                    break;
                }
            }

            window
                .Hide(() =>
                {
                    onHidden?.Invoke();
                    RemoveWindow(window);
                });
        }

        public void Hide<T>(Action onHidden = null) where T : BaseWindow
        {
            for (var i = _windowsStack.Count - 1; i >= 0; i--)
            {
                var historyWindow = _windowsStack[i];
                if (!(historyWindow is T))
                    continue;

                _windowsStack.Remove(historyWindow);
                historyWindow
                    .Hide(() =>
                    {
                        onHidden?.Invoke();
                        RemoveWindow(historyWindow);
                    });
                break;
            }
        }

        public void GoBack()
        {
            if (_windowsStack.Count == 0 || BlockGoBack)
                return;

            var window = _windowsStack.Last();
            _windowsStack.RemoveAt(_windowsStack.Count - 1);
            window
                .Hide(() => { RemoveWindow(window); });
        }


        private BaseWindow GetWindow(Type windowType, Transform parent = null)
        {
            if (parent == null)
                parent = _viewRoots[""];

            var prefab = _windowPrefabs[windowType];
            var instance = PrefabPool.PrefabPool.InstanceGlobal.Spawn(prefab, parent);
            instance.transform.SetAsLastSibling();

            return instance;
        }

        private void RemoveWindow(BaseWindow window)
        {
            window.Dispose();
            //PrefabPool.PrefabPool.InstanceGlobal.Despawn(window);
        }
    }
}