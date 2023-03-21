using Leopotam.EcsLite;
using Locator;
using Models.Components;
using Services;
using UnityEngine;


namespace Rules
{
    public class TaskSystem : IEcsRunSystem
    {
        private readonly EcsFilter _taskReceiver;
        private readonly EcsWorld _world;


        public TaskSystem()
        {
            _world = Container.Get<EcsWorld>();
            _taskReceiver = _world.Filter<ComponentTransform>().Inc<ComponentAiMemory>().End();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var i in _taskReceiver)
            {
                ref var cTransform = ref _world.GetPool<ComponentTransform>().Get(i);

                ref var cAi = ref _world.GetPool<ComponentAiMemory>().Get(i);
                if (i.Has<ComponentChopTreeJobRequest>(_world))
                {
                    var clickPosition = i.Get<ComponentChopTreeJobRequest>(_world).ClickPosition;
                    var tree = i.Get<ComponentChopTreeJobRequest>(_world).Tree;
                    cAi.TargetJobType = JobType.Chop;
                    cAi.LastPosition = cTransform.Position;
                    cAi.TargetPosition = clickPosition;
                    cAi.HasNewTarget = true;
                    cAi.Target = tree;

                    i.Del<ComponentChopTreeJobRequest>(_world);

                    /*
                    cAi.TargetJobType = WorkerJobType.Chop;
                    cAi.LastPosition = cTransform.Position;
                    var agent = i.Get<ComponentNavAgent>(_world);

                    //find  navemesh near clickPosition
                    var distance = cTransform.Position - clickPosition;
                    var samplingResolution = 10;
                    var step = distance / samplingResolution;
                    //NavMesh.SamplePosition()

                    var movePos = cTransform.Position;
                    for (var j = 0; j < samplingResolution + 1; j++)
                    {
                        var probe = clickPosition + step * j;

                        if (NavMesh.SamplePosition(probe, out var hit, 0.1f, agent.Agent.areaMask))
                        {
                            movePos = hit.position;
                            break;
                        }
                    }

                    ref var c = ref i.Add<ComponentMoveTarget>(_world);*/

                    //add job move pos
                    //add find tree
                    //
                    //add move to tree
                    //add start chop
                }
            }
        }

    }
}