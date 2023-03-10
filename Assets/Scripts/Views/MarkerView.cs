using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Services.PrefabPool;
using UniRx;
using UnityEngine;

namespace Views
{
    public class MarkerView : MonoPoolableObject
    {
        private IDisposable _sup;
        private Vector3 _initialScale;
        private Tween _tween;

        public override void Awake()
        {
            base.Awake();
            _initialScale = transform.localScale;
        }

        public override void OnSpawned()
        {
            base.OnSpawned();
            transform.localScale = _initialScale;
            _sup?.Dispose();

            _tween = transform.DOScale(0.01f, 0.3f);
            _sup = Observable.Timer(TimeSpan.FromSeconds(0.3f)).Subscribe(x => Dispose());
        }
        public override void Dispose()
        {
            _tween?.Kill();
            base.Dispose(this);
            
        }
    }
}
