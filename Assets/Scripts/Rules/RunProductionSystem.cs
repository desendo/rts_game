using System.Collections.Generic;
using System.Linq;
using Data;
using Leopotam.EcsLite;
using Locator;
using Models.Components;
using Services;
using UniRx;
using UnityEngine;

namespace Rules
{
    public class RunProductionSystem : IEcsInitSystem, IEcsRunSystem
    {
        private readonly GameMessenger _messenger;
        private readonly CompositeDisposable _sup = new CompositeDisposable();
        private GameConfigData _config;
        private IUnitsService _unitsService;
        private EcsWorld _world;
        private EcsFilter _filter1;

        public RunProductionSystem()
        {
            _config = Container.Get<GameConfigData>();
            _messenger = Container.Get<GameMessenger>();
            _unitsService = Container.Get<IUnitsService>();
        }
        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();
            _filter1 = _world.Filter<ComponentProductionRun>().End();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var i in _filter1)
            {
                ref var c1 = ref i.Get<ComponentProductionRun>(_world);
                c1.Current += Time.deltaTime;

                if (c1.Current > c1.Max)
                {
                    if (_world.GetPool<ComponentUnitProductionBuilding>().Has(i))
                    {
                        var path = _world.GetPool<ComponentUnitProductionBuilding>().Get(i);
                        var pos = path.Path[0];
                        var rot = Quaternion.LookRotation(path.Path[1] - path.Path[0]);
                        _unitsService.CreateUnit(c1.Result, pos, rot, out var unit);
                        ref var c4 = ref _world.GetPool<ComponentMoveTargetSimple>().Add(unit);
                        c4.Target = path.Path[1];
                        _world.GetPool<ComponentProductionRun>().Del(i);
                    }
                }
            }
        }
    }
}