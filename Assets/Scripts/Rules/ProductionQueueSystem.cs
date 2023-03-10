using System.Collections.Generic;
using Leopotam.EcsLite;
using Locator;
using Models.Components;
using Services;
using Signals;
using UnityEngine;

namespace Rules
{
    public class ProductionQueueSystem : IEcsInitSystem
    {
        private readonly GameMessenger _messenger;
        private EcsFilter _filter;
        private EcsWorld _world;

        public ProductionQueueSystem()
        {
            _messenger = Container.Get<GameMessenger>();
            _messenger.Subscribe<MainSignals.ProductionEnqueueRequest>(ProductionEnqueueRequest);
            _messenger.Subscribe<MainSignals.ProductionDequeueRequest>(ProductionDequeueRequest);
        }

        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();
            _filter = _world.Filter<ComponentProductionSchema>().Exc<ComponentProductionQueueUpdated>().Exc<ComponentBuilder>().End();
        }

        private void ProductionEnqueueRequest(MainSignals.ProductionEnqueueRequest obj)
        {

            var pool = _world.GetPool<ComponentProductionQueue>();
            foreach (var i in _filter)
            {
                if (i != obj.UnitIndex) continue;
                if (!pool.Has(i))
                {
                    ref var c1 = ref pool.Add(i);
                    c1.Queue ??= new List<string>();
                    c1.Queue.Add(obj.ResultId);
                }
                else
                {
                    ref var c1 = ref pool.Get(i);
                    c1.Queue ??= new List<string>();
                    c1.Queue.Add(obj.ResultId);
                }

                _world.GetPool<ComponentProductionQueueUpdated>().Add(i);
            }
        }

        private void ProductionDequeueRequest(MainSignals.ProductionDequeueRequest obj)
        {
            var pool = _world.GetPool<ComponentProductionQueue>();
            foreach (var i in _filter)
            {
                if (i != obj.UnitIndex) continue;


                if (!pool.Has(i))
                {
                    ref var c1 = ref pool.Add(i);
                    c1.Queue ??= new List<string>();
                    c1.Queue.Remove(obj.ResultId);
                }
                else
                {
                    ref var c1 = ref pool.Get(i);
                    c1.Queue ??= new List<string>();
                    c1.Queue.Remove(obj.ResultId);
                }

                _world.GetPool<ComponentProductionQueueUpdated>().Add(i);
            }
        }
    }
}