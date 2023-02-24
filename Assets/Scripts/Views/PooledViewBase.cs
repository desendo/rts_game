using System;
using Services.PrefabPool;
using UniRx;

namespace Views
{
    public abstract class Window : BaseWindow
    {
    }

    public abstract class Window<TData> : BaseWindow where TData : struct
    {
        public TData CurrentData { get; private set; }

        public void SetData(TData data)
        {
            CurrentData = data;
            OnSetData(data);
        }

        protected abstract void OnSetData(TData data);
    }

    public abstract class BaseWindow : MonoPoolableObject
    {
        public enum WindowState
        {
            Hidden,
            IsHiding,
            Showed,
            IsShowing
        }

        private ReactiveProperty<WindowState> _state = new ReactiveProperty<WindowState>(WindowState.Hidden);
        public IReadOnlyReactiveProperty<WindowState> State => _state;

        public void Show(Action callback)
        {
            _state.Value = WindowState.IsShowing;
            ShowWindow(() =>
            {
                callback?.Invoke();
                _state.Value = WindowState.Showed;
            });
        }


        public void Hide(Action callback)
        {
            _state.Value = WindowState.IsHiding;
            HideWindow(() =>
            {
                callback?.Invoke();
                _state.Value = WindowState.Hidden;
                _state.Dispose();
                _state = new ReactiveProperty<WindowState>(WindowState.Hidden);
            });
        }

#pragma warning disable 1998
        protected virtual void ShowWindow(Action callback)
        {
        }

        protected virtual void HideWindow(Action callback)
        {
        }
#pragma warning restore 1998
    }
}