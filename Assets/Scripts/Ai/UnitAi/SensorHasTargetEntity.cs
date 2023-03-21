using Leopotam.EcsLite;
using Models.Components;
using Services;
using UnityEngine;

namespace Ai.UnitAi
{
    public class SensorHasTargetEntity : SensorBoolBase
    {
        public static string SensorId = nameof(SensorHasTargetEntity);

        private static readonly int SensorIdHash = Animator.StringToHash("ai_has_target_entity");

        private void SetAnimatorParam(bool value)
        {
            animator.SetBool(SensorIdHash, value);
        }

        public override void Update()
        {
            if(!isSet)
                return;

            if (i.Has<ComponentAiMemory>(world) && i.Get<ComponentAiMemory>(world).Target.Unpack(world, out var target))
            {
                SetAnimatorParam(true);
            }
            else
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