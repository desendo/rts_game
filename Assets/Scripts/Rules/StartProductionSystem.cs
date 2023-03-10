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
    public class StartProductionSystem : IEcsInitSystem, IEcsRunSystem
    {
        private readonly GameMessenger _messenger;
        private readonly CompositeDisposable _sup = new CompositeDisposable();
        private GameConfigData _config;
        private EcsWorld _world;
        private EcsFilter _filter;

        public StartProductionSystem()
        {
            _config = Container.Get<GameConfigData>();
            _messenger = Container.Get<GameMessenger>();

        }
        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();
            _filter = _world.Filter<ComponentProductionQueue>().Exc<ComponentProductionRun>().Inc<ComponentProductionSchema>().Exc<ComponentBuilder>().End();
        }
        public void Run(IEcsSystems systems)
        {
            foreach (var i in _filter)
            {
                ref var c1 = ref i.Get<ComponentProductionQueue>(_world);
                ref var c3 = ref i.Get<ComponentProductionSchema>(_world);
                if (c1.Queue.Count > 0)
                {
                    var target = c1.Queue[0];
                    var variant = c3.Variants.FirstOrDefault(x => x.ResultId == target);
                    if (variant != null)
                    {
                        ref var c2 = ref _world.GetPool<ComponentProductionRun>().Add(i);
                        c1.Queue.RemoveAt(0);
                        c2.Current = 0;
                        c2.Max = variant.Duration;
                        c2.Result = target;
                        if(!_world.GetPool<ComponentProductionProgressStarted>().Has(i))
                            _world.GetPool<ComponentProductionProgressStarted>().Add(i);
                        if(!_world.GetPool<ComponentProductionQueueUpdated>().Has(i))
                            _world.GetPool<ComponentProductionQueueUpdated>().Add(i);
                    }
                    else
                    {
                        Debug.Log("zero production start variant");
                    }
                }
            }
        }

    }
}