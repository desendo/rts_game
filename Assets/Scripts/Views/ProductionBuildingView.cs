using Ai.UnitAi;
using Leopotam.EcsLite;
using Leopotam.EcsLite.UnityEditor;
using Locator;
using Models.Components;
using Services;
using Services.PrefabPool;
using Signals;
using UniRx;
using UnityEditor.AI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Views
{
    public class UnitView : MonoPoolableObject, IPointerClickHandler, IModelView, ISelect, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Collider _collider;
        [SerializeField] private GameObject _selected;
        [SerializeField] private GameObject _hovered;
        [SerializeField] private StateMachineController _stateMachineController;
        private GameMessenger _messenger;

        public string ConfigId => _configId;
        protected readonly CompositeDisposable _sup = new CompositeDisposable();
        protected int _entity = -1;
        protected EcsWorld _world;
        private ISelect _selectImplementation;
        private string _configId;
        public int Entity => _entity;

        public virtual void Bind(int entity)
        {
            _world = Container.Get<EcsWorld>();
            _entity = entity;
            if(_stateMachineController != null)
                _stateMachineController.Bind(_world, _entity);
            var id = _world.GetPool<ComponentUnit>().Get(_entity).ConfigId;
            SetId(id);
        }

        public void SetId(string id)
        {
            _configId = id;
        }

        public override void Awake()
        {
            base.Awake();
            _messenger = Container.Get<GameMessenger>();
        }

        public virtual void OnPointerClick(PointerEventData eventData)
        {
            if(_entity == -1)
                return;

            if (eventData.button == PointerEventData.InputButton.Left)
            {
                _messenger.Fire(new MainSignals.SelectRequest(_entity));
            }
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                _messenger.Fire(new MainSignals.ContextActionRequest(_entity));
            }
        }

        public override void OnDespawned()
        {
            _sup.Clear();
            base.OnDespawned();
            if(_entity > -1)
                _world?.DelEntity(_entity);
            _entity = -1;
        }

        public override void Dispose()
        {
            PrefabPool.InstanceGlobal.Despawn(this);

        }

        private void OnDestroy()
        {
            _sup.Clear();
        }


        public void SetSelected(bool selected)=> _selected.SetActive(selected);
        public void SetHovered(bool hovered) => _hovered.SetActive(hovered);

        public void OnPointerEnter(PointerEventData eventData)
        {
            _messenger.Fire(new MainSignals.HoverRequest(_entity, true));

        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _messenger.Fire(new MainSignals.HoverRequest(_entity, false));
        }
    }

    public interface ISelect
    {
        void SetSelected(bool selected);
        void SetHovered(bool hovered);
    }

    public interface IModelView
    {
        int Entity { get; }

    }
}
