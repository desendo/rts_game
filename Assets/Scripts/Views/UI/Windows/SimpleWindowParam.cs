using System;
using DG.Tweening;
using UnityEngine;

namespace Views.UI.Windows
{
    [RequireComponent(typeof(AnimatedPanel))]
    public abstract class SimpleWindowParam<TData> : Window<TData> where TData : struct
    {
        [SerializeField]
        protected AnimatedPanel _animatedPanel;

        private Tween _tween;

        public override void Awake()
        {
            _animatedPanel = GetComponent<AnimatedPanel>();
            OnAwake();
        }

        protected virtual void OnAwake() {}

        protected override void ShowWindow(Action callback)
        {
            _tween?.Kill();
            _tween = _animatedPanel
                .ShowPanel().OnComplete(() => callback?.Invoke());

        }

        protected override void HideWindow(Action callback)
        {
            _tween?.Kill();
            _tween = _animatedPanel
                .HidePanel().OnComplete(() => callback?.Invoke());
        }
    }
}