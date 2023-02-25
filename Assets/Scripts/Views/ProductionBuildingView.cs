using Leopotam.EcsLite;
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
    public class UnitView : MonoPoolableObject, IPointerClickHandler, IModelView,ISelect
    {
        [SerializeField] private Collider _collider;
        [SerializeField] private GameObject _selected;
        [SerializeField] private GameObject _hovered;
        //[SerializeField] private GameObject _actioned;
        private GameMessenger _messenger;

        protected readonly CompositeDisposable _sup = new CompositeDisposable();
        private int _entity;
        private EcsWorld _world;
        private ISelect _selectImplementation;
        private bool _selectEnabled;
        public int Entity => _entity;

        public void Bind(int entity)
        {
            _world = Container.Get<EcsWorld>();
            _entity = entity;
            var unit = _world.GetPool<ComponentUnit>().Get(_entity);
            ref var tr = ref _world.GetPool<ComponentTransform>().Get(_entity);
            ref var sel = ref _world.GetPool<ComponentSelection>().Get(_entity);

        }
        public override void Awake()
        {
            base.Awake();
            _messenger = Container.Get<GameMessenger>();
        }

        public virtual void OnPointerClick(PointerEventData eventData)
        {
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
            NavMeshBuilder.BuildNavMesh();

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
