using Leopotam.EcsLite;
using Models.Components;
using UnityEngine;

namespace Rules.StateSystems
{
    public struct ComponentSensorIsMoving
    {
        public bool Value;
    }

    internal sealed class StateIsMovingSystem : IEcsRunSystem, IEcsInitSystem
    {
        private EcsFilter _filter;

        private const float _moveTolerance = 0.1f;
        public void Init(IEcsSystems systems)
        {
            _filter = systems.GetWorld().Filter<ComponentSensorIsMoving>().Inc<ComponentTransform>().End();
        }
        public void Run(IEcsSystems systems)
        {
            foreach (var i in _filter)
            {
                 var c1 =  systems.GetWorld().GetPool<ComponentTransform>().Get(i);
                ref var c2 = ref systems.GetWorld().GetPool<ComponentSensorIsMoving>().Get(i);
                var delta = c1.Delta.sqrMagnitude / (Time.deltaTime * Time.deltaTime);
                c2.Value = delta > _moveTolerance;
            }
        }

    }
}