using Models.Components;
using Services;
using UnityEngine;

namespace Ai.UnitAi
{
    public class SensorMoveTargetReached : SensorBoolBase
    {
        public static string SensorId = nameof(SensorMoveTargetReached);
        private static readonly int ParamHash = Animator.StringToHash("move_target_reached");


        private void SetAnimatorParam(bool value)
        {
            animator.SetBool(ParamHash, value);
        }

        public override void Update()
        {
            if(!isSet)
                return;

            if (!i.Has<ComponentMoveTarget>(world) && i.Has<ComponentMoveTargetReached>(world))
            {
                i.Del<ComponentMoveTargetReached>(world);
                SetAnimatorParam(true);
            }
            if (i.Has<ComponentMoveTarget>(world) && !i.Has<ComponentMoveTargetReached>(world))
            {
                SetAnimatorParam(false);
            }


        }
        protected override void OnReset()
        {
            isSet = false;
        }
    }
}