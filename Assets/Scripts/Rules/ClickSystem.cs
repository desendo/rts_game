using Leopotam.EcsLite;
using Locator;
using Models.Components;
using Services;
using Signals;
using UnityEngine;
using Views;

namespace Rules
{
    public class ClickSystem : IEcsRunSystem
    {
        private readonly PointerService _pointerService;
        private readonly IUnitsService _unitsService;
        private readonly EcsWorld _world;
        private readonly EcsFilter _selectMovableFilter;

        private const string tree = "tree";
        public ClickSystem()
        {
            _pointerService = Container.Get<PointerService>();
            _unitsService = Container.Get<IUnitsService>();
            _world = Container.Get<EcsWorld>();
            _selectMovableFilter = _world.Filter<ComponentSelection>().Inc<ComponentTransform>().Inc<ComponentUnit>().End();
            var messenger = Container.Get<GameMessenger>();

            messenger.Subscribe<MainSignals.EarthLeftClick>(x => OnEarthLeftClick());
            messenger.Subscribe<MainSignals.EarthRightClick>(x => OnEarthRightClick(x.Click));
            messenger.Subscribe<MainSignals.SelectRequest>(OnSelectRequest);
            messenger.Subscribe<MainSignals.ContextActionRequest>(OnActionRequest);
            messenger.Subscribe<MainSignals.HoverRequest>(OnHoverRequest);
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var i in _selectMovableFilter)
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
            if (_pointerService.FunctionalState.Value == FunctionalState.Free && _pointerService.UnitState.Value == UnitState.Free)
            {
                var shouldSpawnWalkMarker = false;
                var id = string.Empty;
                foreach (var i in _selectMovableFilter)
                {
                    var selection = _world.GetPool<ComponentSelection>().Get(i);
                    var unit = _world.GetPool<ComponentUnit>().Get(i);

                    if (selection.Selected && unit.PlayerIndex == _unitsService.CurrentPlayerIndex.Value && i.Has<ComponentAiMemory>(_world))
                    {
                        ref var ai = ref _world.GetPool<ComponentAiMemory>().Get(i);
                        ai.HasNewTarget = true;
                        ai.TargetPosition = objClick;
                        ai.LastPosition = i.Get<ComponentTransform>(_world).Position;
                        ai.TargetJobType = JobType.None;
                        ai.Target = default;
                        shouldSpawnWalkMarker = true;
                        id = unit.ConfigId;
                    }
                }
                if(!string.IsNullOrEmpty(id))
                    Container.Get<SoundService>().PlayMove(id);

                if (shouldSpawnWalkMarker)
                    _unitsService.SpawnMoveMarker(objClick);
            }
        }

        private void OnEarthLeftClick()
        {
            if(_pointerService.UnitState.Value == UnitState.ChooseBuildSite)
                return;

            foreach (var i in _selectMovableFilter)
            {
                ref var selection = ref _world.GetPool<ComponentSelection>().Get(i);
                selection.Selected = false;
                selection.Hovered = false;
            }
            _unitsService.SetSelected(-1);
        }

        private void OnHoverRequest(MainSignals.HoverRequest obj)
        {
            foreach (var i in _selectMovableFilter)
            {
                ref var selection = ref _world.GetPool<ComponentSelection>().Get(i);
                if (i == obj.Entity)
                {
                    selection.Hovered = obj.Hovered;
                }
            }

            var anyHovered = false;
            foreach (var i in _selectMovableFilter)
            {
                var selection = _world.GetPool<ComponentSelection>().Get(i);
                if (selection.Hovered)
                    anyHovered = true;
            }
            _pointerService.SetHovered(anyHovered);
        }

        private void OnActionRequest(MainSignals.ContextActionRequest obj)
        {
            foreach (var selected in _selectMovableFilter)
            {
                var selection =  _world.GetPool<ComponentSelection>().Get(selected);

                if (!selection.Selected) continue;
                if (!_world.GetPool<ComponentWorker>().Has(selected)) continue;
                if (!_world.GetPool<ComponentSource>().Has(obj.Entity)) continue;

                if (obj.Entity.Has<ComponentSource>(_world))
                {
                    var source = obj.Entity.Get<ComponentSource>(_world);
                    if (source.Type == ResourceType.Wood)
                    {
                        ref var task = ref _world.GetPool<ComponentChopTreeJobRequest>().Add(selected);
                        task.ClickPosition = obj.Entity.Get<ComponentTransform>(_world).Position;
                        task.Tree = _world.PackEntity(obj.Entity);

                    }
                }
            }

        }

        private void OnSelectRequest(MainSignals.SelectRequest obj)
        {
            foreach (var i in _selectMovableFilter)
            {
                ref var selection = ref _world.GetPool<ComponentSelection>().Get(i);
                selection.Selected = false;
                selection.Hovered = false;
                if (i == obj.Entity)
                {
                    _unitsService.SetSelected(i);
                    selection.Selected = true;
                    var unit = _world.GetPool<ComponentUnit>().Get(i);
                    Container.Get<SoundService>().PlaySelect(unit.ConfigId);
                }
            }
        }

    }
}