using Leopotam.EcsLite;
using Models.Components;
using Services;
using UnityEngine;
using UnityEngine.AI;

namespace Ai.UnitAi
{
    public static class StateActionFindMoveTarget
    {
        public static AiStateAction Type = AiStateAction.FindMoveTarget;

        public static void OnEnter (EcsWorld world, int i)
        {
            ref var cAi = ref world.GetPool<ComponentAiMemory>().Get(i);
            ref var cTransform = ref world.GetPool<ComponentTransform>().Get(i);
            var agent = i.Get<ComponentNavAgent>(world);

            if (cAi.Target.Unpack(world, out var targetEntity))
            {
                if (targetEntity.Has<ComponentTransform>(world))
                {
                    var pos = targetEntity.Get<ComponentTransform>(world).Position;
                    if (NavMesh.SamplePosition(pos, out var hit, 1f, agent.Agent.areaMask))
                    {
                        var targetMove = hit.position;
                        ref var c1 = ref i.Add<ComponentMoveTarget>(world);
                        c1.Target = targetMove;
                        cAi.HasNewTarget = false;
                        return;
                    }
                }
            }

            var distance = cTransform.Position - cAi.TargetPosition;
            var samplingResolution = 10;
            var step = distance / samplingResolution;

            var movePos = cTransform.Position;
            for (var j = 0; j < samplingResolution + 1; j++)
            {
                var probe = cAi.TargetPosition + step * j;

                if (NavMesh.SamplePosition(probe, out var hit, 0.1f, agent.Agent.areaMask))
                {
                    movePos = hit.position;
                    break;
                }
            }

            ref var c = ref i.Add<ComponentMoveTarget>(world);
            c.Target = movePos;
            cAi.HasNewTarget = false;
        }


    }
}
