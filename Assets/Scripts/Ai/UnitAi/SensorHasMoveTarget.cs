using Leopotam.EcsLite;
using Models.Components;
using Services;
using UnityEngine;

namespace Ai.UnitAi
{
    public class SensorHasMoveTarget : SensorBoolBase
    {
        public static string SensorId = nameof(SensorHasMoveTarget);

        private static readonly int SensorIdHash = Animator.StringToHash("has_move_target");

        private void SetAnimatorParam(bool value)
        {
            animator.SetBool(SensorIdHash, value);
        }

        public override void Update()
        {
            if(!isSet)
                return;

            SetAnimatorParam(i.Has<ComponentMoveTarget>(world));
        }
        protected override void OnReset()
        {
            isSet = false;
        }
    }
}