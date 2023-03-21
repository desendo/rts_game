using Leopotam.EcsLite;
using Rules.StateSystems;
using UnityEngine;

namespace Ai.UnitAi
{
    public class SensorIsMoving : SensorBoolBase
    {
        public static string SensorId = nameof(SensorIsMoving);
        private static readonly int ParamHash = Animator.StringToHash("is_moving");


        public override void Bind(EcsWorld w, int e)
        {
            base.Bind(w, e);
            world.GetPool<ComponentSensorIsMoving>().Add(e);
        }

        private void SetAnimatorParam(bool value)
        {
            animator.SetBool(ParamHash, value);
        }

        public override void Update()
        {
            if(!isSet)
                return;

            SetAnimatorParam(world.GetPool<ComponentSensorIsMoving>().Get(i).Value);
        }
        protected override void OnReset()
        {
            isSet = false;
        }
    }
}