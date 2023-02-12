using System;
using Locator;
using Models;
using Services;
using Services.PrefabPool;
using Signals;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Views
{
    public class UnitView : MonoPoolableObject, IPointerClickHandler, IModelView
    {
        [SerializeField] private Collider _collider;
        private GameMessenger _messenger;
        private IModel _model;

        protected readonly CompositeDisposable _sup = new CompositeDisposable();
        public virtual IModel Model => _model;
        public virtual void Bind(IModel model)
        {
        }

        protected override void Awake()
        {
            base.Awake();
            _messenger = Container.Get<GameMessenger>();
        }

        public virtual void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                _messenger.Fire(new MainSignals.SelectModelView(_model));
            }
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                _messenger.Fire(new MainSignals.ContextActionModelView(_model));
            }
        }

        public override void OnDespawned()
        {
            _sup.Clear();
            base.OnDespawned();
        }

        private void OnDestroy()
        {
            _sup.Clear();
        }
    }

    public interface IModelView
    {
        IModel Model { get; }
        void Bind(IModel model);
    }
}
