using Models.Components;
using Services;
using UnityEngine;

namespace Ai.UnitAi
{
    public class SensorSetDestination : SensorBoolBase
    {
        public static string SensorId = nameof(SensorSetDestination);

        private static readonly int ParamHash = Animator.StringToHash("ai_has_new_target");

        private void SetAnimatorParam(bool value)
        {
            animator.SetBool(ParamHash, value);
        }

        public override void Update()
        {
            if(!isSet)
                return;

            if (!i.Has<ComponentAiMemory>(world))
            {
                SetAnimatorParam(false);
            }
            else
            {
                var targetSet = i.Get<ComponentAiMemory>(world).HasNewTarget;
                SetAnimatorParam(targetSet);
            }

        }
        protected override void OnReset()
        {
            isSet = false;
        }
    }
}