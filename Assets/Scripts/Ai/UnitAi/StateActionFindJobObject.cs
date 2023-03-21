using Leopotam.EcsLite;
using Models.Components;
using Services;
using UnityEngine.AI;

namespace Ai.UnitAi
{
    public static class StateActionFindJobObject
    {
        public static AiStateAction Type = AiStateAction.FindJobObject;

        public static void OnEnter (EcsWorld world, int i)
        {
            var filter = world.Filter<ComponentSource>().Inc<ComponentTransform>().End();
            ref var cAi = ref world.GetPool<ComponentAiMemory>().Get(i);
            ref var cTransform = ref world.GetPool<ComponentTransform>().Get(i);

            float dist = 25f;
            foreach (var i1 in filter)
            {
                var c1 = i1.Get<ComponentSource>(world);
                var c2 = i1.Get<ComponentTransform>(world);

                var deltaDist = (cTransform.Position - c2.Position).sqrMagnitude;
                if (deltaDist < dist)
                {
                    
                    dist = deltaDist;
                }

            }

        }


    }
}