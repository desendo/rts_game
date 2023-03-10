using Leopotam.EcsLite;
using Rules.StateSystems;
using UnityEngine;

namespace Ai.UnitAi
{
    public class SensorIsMoving : SensorBoolBase
    {
        public static string SensorId => "is_moving";
        private static readonly int SensorIdHash = Animator.StringToHash(SensorId);
        private EcsWorld _world;
        private int _i;
        private bool _isSet;
        public SensorIsMoving()
        {
        }

        public override void BindEntity(EcsWorld world, int i)
        {
            _i = i;
            _world = world;
            world.GetPool<ComponentSensorIsMoving>().Add(i);
            _isSet = true;
        }

        private void SetAnimatorParam(bool value)
        {
            animator.SetBool(SensorIdHash, value);
        }

        public override void Update()
        {
            if(!_isSet)
                return;

            SetAnimatorParam(_world.GetPool<ComponentSensorIsMoving>().Get(_i).Value);
        }
        protected override void OnReset()
        {
            _isSet = false;
        }
    }

}