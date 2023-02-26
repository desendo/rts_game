using Leopotam.EcsLite;
using Locator;
using Models.Aspects;
using Models.Components;
using Services;
using Signals;
using UnityEngine;
using Views;

namespace Rules
{

    public class ClickSystem : IEcsInitSystem, IEcsRunSystem
    {
        private readonly IPointerService _pointerService;
        private readonly IUnitsService _unitsService;
        private readonly EcsWorld _world;
        private EcsFilter _selectFilter;

        public ClickSystem()
        {
            _pointerService = Container.Get<IPointerService>();
            _unitsService = Container.Get<IUnitsService>();
            _world = Container.Get<EcsWorld>();
            _selectFilter = _world.Filter<ComponentSelection>().End();

            var messenger = Container.Get<GameMessenger>();
            messenger.Subscribe<MainSignals.EarthLeftClick>(x => ClearSelection());
            messenger.Subscribe<MainSignals.EarthRightClick>(x => OnEarthRightClick(x.Click));
            messenger.Subscribe<MainSignals.SelectRequest>(OnSelectRequest);
            messenger.Subscribe<MainSignals.ContextActionRequest>(OnActionRequest);
            messenger.Subscribe<MainSignals.HoverRequest>(OnHoverRequest);
        }

        public void Init(IEcsSystems systems)
        {

        }

        public void Run(IEcsSystems systems)
        {
            foreach (var i in _selectFilter)
            {
                var selection =  _world.GetPool<ComponentSelection>().Get(i);
                if (_unitsService.Units.TryGetValue(i, out var view))
                {
                    if (view is ISelect selectView)
                    {
                        selectView.SetSelected(selection.Selected);
                        selectView.SetHovered(selection.Hovered);
                    }
                }
            }
        }
        private void OnEarthRightClick(Vector3 objClick)
        {
            foreach (var i in _selectFilter)
            {
                var selection = _world.GetPool<ComponentSelection>().Get(i);
                if (selection.Selected)
                {
                    ref var  c1 = ref _world.GetPool<ComponentMoveTarget>().Add(i);
                    c1.Target = objClick;
                }
            }
        }

        private void ClearSelection()
        {
            foreach (var i in _selectFilter)
            {
                ref var selection = ref _world.GetPool<ComponentSelection>().Get(i);
                selection.Selected = false;
                selection.Hovered = false;
            }
        }

        private void OnHoverRequest(MainSignals.HoverRequest obj)
        {
            //obj.Model.AspectSelection.Hovered.Value = true;
        }

        private void OnActionRequest(MainSignals.ContextActionRequest obj)
        {
            //Debug.Log("action");
        }

        private void OnSelectRequest(MainSignals.SelectRequest obj)
        {
            
            foreach (var i in _selectFilter)
            {
                ref var selection = ref _world.GetPool<ComponentSelection>().Get(i);
                selection.Selected = false;
                selection.Hovered = false;
                if (i == obj.Entity)
                    selection.Selected = true;
            }
            /*if(_unitsService.CurrentUnitSelected.Value == obj.Model.AspectUnit)
                return;

            ClearSelection();
            obj.Model.AspectSelection.Selected.Value = true;
            _unitsService.SetSelected(obj.Model.AspectUnit);*/

        }

    }
}