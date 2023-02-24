using Locator;
using Services;
using Services.PrefabPool;
using Signals;
using UniRx;
using UnityEditor.AI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Views
{
    public class UnitView : MonoPoolableObject, IPointerClickHandler, IModelView
    {
        [SerializeField] private Collider _collider;
        [SerializeField] private GameObject _selected;
        [SerializeField] private GameObject _hovered;
        //[SerializeField] private GameObject _actioned;
        private GameMessenger _messenger;
        private UnitCompositionBase _model;

        protected readonly CompositeDisposable _sup = new CompositeDisposable();
        public virtual UnitCompositionBase Model => _model;
        public virtual void Bind(UnitCompositionBase model)
        {
            _model = model;
            model.AspectSelection.Hovered.Subscribe(b => { _hovered.SetActive(b);}).AddTo(_sup);
            model.AspectSelection.Selected.Subscribe(b => { _selected.SetActive(b);}).AddTo(_sup);
            transform.position = model.AspectUnit.Position.Value;
            transform.rotation = model.AspectUnit.Rotation.Value;
            //model.AspectSelection.Actioned.Subscribe(b => { _actioned.SetActive(b);}).AddTo(_sup);
            NavMeshBuilder.BuildNavMesh();
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
                _messenger.Fire(new MainSignals.SelectRequest(_model));
            }
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                _messenger.Fire(new MainSignals.ContextActionRequest(_model));
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
    }

    public interface IModelView
    {
        UnitCompositionBase Model { get; }
        void Bind(UnitCompositionBase model);
    }
}
