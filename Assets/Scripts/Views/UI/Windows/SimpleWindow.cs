using System;
using DG.Tweening;
using UnityEngine;

namespace Views.UI.Windows
{
    [RequireComponent(typeof(AnimatedPanel))]
    public abstract class SimpleWindow : Window
    {
        private AnimatedPanel _animatedPanel;

        public override void Awake()
        {
            _animatedPanel = GetComponent<AnimatedPanel>();
            OnAwake();
        }

        protected virtual void OnAwake()
        {
        }

        protected override void ShowWindow(Action callback)
        {
            _animatedPanel
                .ShowPanel().OnComplete(() => callback?.Invoke());
        }

        protected override void HideWindow(Action callback)
        {
            _animatedPanel
                .HidePanel().OnComplete(() => callback?.Invoke());
        }
    }
}