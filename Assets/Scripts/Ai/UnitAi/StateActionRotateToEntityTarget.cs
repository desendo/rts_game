using Leopotam.EcsLite;
using Models.Components;
using Services;
using UnityEngine;
using UnityEngine.AI;

namespace Ai.UnitAi
{
    public static class StateActionRotateToEntityTarget
    {
        public static AiStateAction Type = AiStateAction.RotateToEntityTarget;

        public static void OnUpdate(float dt, EcsWorld world, int i)
        {
            ref var cAi = ref world.GetPool<ComponentAiMemory>().Get(i);
            var c3 = world.GetPool<ComponentMove>().Get(i);
            ref var c2 = ref world.GetPool<ComponentTransform>().Get(i);

            if(cAi.Target.Unpack(world, out var target))
            {
                var targetTransform =  world.GetPool<ComponentTransform>().Get(target);

                var targetDir = (targetTransform.Position - c2.Position).normalized;
                var scalar = Vector3.Dot(c2.Direction.normalized, targetDir);
                if (scalar < 0.9993)
                {
                    c2.Direction = Vector3.RotateTowards(c2.Direction, targetTransform.Position, 0.1f, 0f);
                    if (i.Has<ComponentNavAgent>(world))
                    {
                        var agent = i.Get<ComponentNavAgent>(world).Agent;
                        agent.transform.forward = c2.Direction;
                        agent.transform.localRotation = Quaternion.Euler(0,agent.transform.eulerAngles.y, 0);
                    }

                }

            }
        }

    }
}