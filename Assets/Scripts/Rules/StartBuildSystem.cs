using System;
using System.Linq;
using Data;
using Helpers;
using Leopotam.EcsLite;
using Locator;
using Models.Components;
using Services;
using Signals;
using UniRx;
using UnityEngine;
using Views;


namespace Rules
{
    public class StartBuildSystem : IEcsInitSystem, IEcsRunSystem
    {
        private readonly GameMessenger _messenger;
        private readonly CompositeDisposable _sup = new CompositeDisposable();
        private GameConfigData _config;
        private EcsWorld _world;
        private EcsFilter _filter;
        private IPointerService _pointerService;
        private IUnitsService _unitsService;
        private ICameraService _cameraService;
        private UnitView _preview;
        private float _rotateBuilingTolerance = 0.5f;

        private UnitState _prevState;
        private IDisposable _timer;

        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();
            _filter = _world.Filter<ComponentProductionSchema>().Inc<ComponentBuilder>().End();

        }
        public StartBuildSystem()
        {
            _config = Container.Get<GameConfigData>();
            _cameraService = Container.Get<ICameraService>();
            _pointerService = Container.Get<IPointerService>();
            _unitsService = Container.Get<IUnitsService>();
            _messenger = Container.Get<GameMessenger>();
            _messenger.Subscribe<MainSignals.ProductionChooseBuildRequest>(ProductionChooseBuild);
            _pointerService.MouseState.Where(x=>x == PointerMouseState.LeftClick ).Subscribe(state =>
            {
                if (_pointerService.UnitState.Value == UnitState.ChooseBuildSite )
                {
                    if (_preview is not null)
                    {
                        _unitsService.CreateUnit(_preview.ConfigId, _preview.transform.position,
                            _preview.transform.rotation, _unitsService.CurrentPlayerIndex.Value, out var entity);
                        _unitsService.HidePreview();
                        _pointerService.SetUnitState(UnitState.Free);
                        _preview = null;
                    }
                }
            });
            _pointerService.MouseState.Where(x=> x == PointerMouseState.RightClick).Subscribe(state =>
            {
                if (_pointerService.UnitState.Value == UnitState.ChooseBuildSite &&
                    _pointerService.PrevMouseState.Value != PointerMouseState.RightButtonHold)
                {
                    if (_preview is not null)
                    {
                        _unitsService.HidePreview();
                        _preview = null;
                        _pointerService.SetUnitState(UnitState.Free);
                    }
                }
            });
        }

        private void ProductionChooseBuild(MainSignals.ProductionChooseBuildRequest obj)
        {
            _timer?.Dispose();
            _timer = Observable.TimerFrame(1).Subscribe(l =>
            {
                if (_pointerService.UnitState.Value == UnitState.Free)
                {
                    //var cfg = _config.BuildConfigs.FirstOrDefault(x => x.Id == obj.ResultId);

                    _preview = _unitsService.ShowPreview(obj.ResultId,
                        _cameraService.GetPlanePointAtCursor(Input.mousePosition));
                    if (_preview != null)
                    {
                        _preview.SetId(obj.ResultId);
                        _preview.transform.rotation = Const.DefaultAngle;
                        _pointerService.SetUnitState(UnitState.ChooseBuildSite);
                    }
                }
            });

        }


        public void Run(IEcsSystems systems)
        {
            if (_pointerService.UnitState.Value == UnitState.ChooseBuildSite && _preview != null)
            {
                if (_pointerService.MouseState.Value == PointerMouseState.LeftButtonHold &&
                    _pointerService.Sum > _rotateBuilingTolerance)
                {
                    _preview.transform.Rotate(Vector3.up, _pointerService.Delta.Value.x);
                }
                else
                {
                    _preview.transform.position = _cameraService.GetPlanePointAtCursor(Input.mousePosition);
                }

            }


            _prevState = _pointerService.UnitState.Value;

        }

    }
}