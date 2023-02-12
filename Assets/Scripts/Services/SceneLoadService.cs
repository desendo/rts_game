using System;
using System.Collections;
using UniRx;
using UnityEngine.SceneManagement;

namespace Services
{
	public interface ISceneLoadServiceReadOnly
    {
        IReadOnlyReactiveProperty<string> SceneLoaded { get; }
        IReadOnlyReactiveProperty<string> SceneUnLoaded { get; }
        IObservable<string> OnSceneStartLoading { get; }
    }

    public interface ISceneLoadService : ISceneLoadServiceReadOnly
    {
        void UnloadScene(string sceneName, Action onUnLoad = null);
        void LoadScene(string sceneName, LoadSceneMode mode = LoadSceneMode.Single, Action onLoad = null);
    }

    public class SceneLoadService : ISceneLoadService
    {
        public IReadOnlyReactiveProperty<string> SceneLoaded => _sceneLoaded;
        public IReadOnlyReactiveProperty<string> SceneUnLoaded => _sceneLoaded;
        public IObservable<string> OnSceneStartLoading => _onSceneStartLoading;

        private readonly Subject<string> _onSceneStartLoading = new Subject<string>();
        private readonly float _onloadDelay = 0.1f;
        private IDisposable _loadStream;
        private IDisposable _unLoadStream;
        private IDisposable _onLoadDelayStream;

        private readonly ReactiveProperty<string> _sceneLoaded = new ReactiveProperty<string>();
        private readonly ReactiveProperty<string> _sceneUnLoaded = new ReactiveProperty<string>();
        public SceneLoadService()
        {
            SceneManager.sceneLoaded += delegate(Scene scene, LoadSceneMode mode)
                {
                    _sceneLoaded.Value = scene.name;
                };
            SceneManager.sceneUnloaded += delegate(Scene scene)
            {
                _sceneUnLoaded.Value = scene.name;
            };
        }

        public void UnloadScene(string sceneName, Action onUnLoad = null)
        {
            _unLoadStream?.Dispose();
            var sceneLoaded = false;
            for (var i = 0; i < SceneManager.sceneCount; i++)
            {
                if (SceneManager.GetSceneAt(i).name == sceneName)
                {
                    sceneLoaded = true;
                }
            }

            if (!sceneLoaded)
            {
                onUnLoad?.Invoke();
                return;
            }

            _unLoadStream = Observable.FromCoroutine(() => AsyncSceneUnLoad(sceneName))
                .Subscribe(x =>
                {
                    _onLoadDelayStream?.Dispose();
                    onUnLoad?.Invoke();

                });
        }

        public void LoadScene(string sceneName, LoadSceneMode mode = LoadSceneMode.Single, Action onLoad = null)
        {
            for (var i = 0; i < SceneManager.sceneCount; i++)
            {
                if (SceneManager.GetSceneAt(i).name == sceneName)
                {
                    onLoad?.Invoke();
                    return;
                }
            }
            _onSceneStartLoading.OnNext(sceneName);
            _loadStream?.Dispose();
            _onLoadDelayStream?.Dispose();

            _loadStream = Observable.FromCoroutine(() => AsyncSceneLoad(sceneName, mode))
                .Subscribe(x =>
                {
                    _onLoadDelayStream?.Dispose();
                    _onLoadDelayStream = Observable.Timer(TimeSpan.FromSeconds(_onloadDelay)).Subscribe(l =>
                    {
                        onLoad?.Invoke();
                        _onLoadDelayStream?.Dispose();
                    });
                });
        }

        private IEnumerator AsyncSceneLoad(string sceneName, LoadSceneMode mode)
        {
            var asyncLoad = SceneManager.LoadSceneAsync(sceneName, mode);

            while (!asyncLoad.isDone) yield return null;
        }
        private IEnumerator AsyncSceneUnLoad(string sceneName)
        {
            var asyncLoad = SceneManager.UnloadSceneAsync(sceneName);

            while (!asyncLoad.isDone) yield return null;
        }
    }

}