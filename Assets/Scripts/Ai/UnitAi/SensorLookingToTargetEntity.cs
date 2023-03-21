using Leopotam.EcsLite;
using Models.Components;
using Services;
using UnityEngine;

namespace Ai.UnitAi
{
    public class SensorLookingToTargetEntity : SensorBoolBase
    {
        public static string SensorId = nameof(SensorLookingToTargetEntity);

        private static readonly int ParamHash = Animator.StringToHash("ai_looking_target_entity");
        private float _tolerance = 0.995f;
        private static readonly int LookTargetEntityValue = Animator.StringToHash("look_target_entity_value");

        private void SetAnimatorParam(bool value)
        {
            animator.SetBool(ParamHash, value);
        }

        public override void Update()
        {
            if(!isSet)
                return;

            var tr = i.Get<ComponentTransform>(world);
            if (!i.Has<ComponentAiMemory>(world))
            {
                SetAnimatorParam(false);
                animator.SetFloat(LookTargetEntityValue, 0);
            }
            else
            {
                var ai = i.Get<ComponentAiMemory>(world);
                if (ai.Target.Unpack(world, out var targetEntity) && targetEntity.Has<ComponentTransform>(world))
                {
                    var targetPos = targetEntity.Get<ComponentTransform>(world).Position;
                    var dot = Vector3.Dot(tr.Direction.normalized, (targetPos - tr.Position).normalized);
                    SetAnimatorParam(dot > _tolerance);
                    animator.SetFloat(LookTargetEntityValue, dot);
                }
                else
                {
                    SetAnimatorParam(false);
                    animator.SetFloat(LookTargetEntityValue, 0);

                }
            }

        }
        protected override void OnReset()
        {
            isSet = false;
        }
    }
}