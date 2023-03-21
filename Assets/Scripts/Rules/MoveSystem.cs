using Leopotam.EcsLite;
using Locator;
using Models.Components;
using UnityEngine;
using UnityEngine.AI;

namespace Rules
{
    public class MoveSystem : IEcsRunSystem
    {
        private readonly EcsWorld _world;
        private readonly EcsFilter _moveTargetFilter;

        private readonly float _distanceTolerance = 0.1f;
        private readonly float _distanceSlowDown = 1f;
        private float _scalarTolerance = 0.9993f;
        private float _minimumProximityFactor = 0.3f;

        public MoveSystem()
        {
            _world = Container.Get<EcsWorld>();
            _moveTargetFilter = _world.Filter<ComponentTransform>().Inc<ComponentMove>().Inc<ComponentMoveTarget>().Inc<ComponentNavAgent>().End();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var i in _moveTargetFilter)
            {
                ref var c1 = ref _world.GetPool<ComponentMoveTarget>().Get(i);
                ref var c2 = ref _world.GetPool<ComponentTransform>().Get(i);
                ref var c3 = ref _world.GetPool<ComponentMove>().Get(i);
                var agent = _world.GetPool<ComponentNavAgent>().Get(i).Agent;


                var targetDirMag = (c1.Target - c2.Position).magnitude;
                var targetDir = (c1.Target - c2.Position) / targetDirMag;
                var scalar = Vector3.Dot(c2.Direction, targetDir);

                c2.OldPosition = c2.Position;
                c2.OldDirection = c2.Direction;
                if (targetDirMag < _distanceTolerance)
                {
                    _world.GetPool<ComponentMoveTarget>().Del(i);
                    _world.GetPool<ComponentMoveTargetReached>().Add(i);
                    c2.Delta = c2.Position - c2.OldPosition;
                    c2.EffectiveVelocity = c2.Delta / Time.deltaTime;
                    c2.EffectiveVelocityMag = c2.EffectiveVelocity.magnitude;
                    agent.ResetPath();
                    continue;
                }
                /*if (!agent.hasPath)
                {
                    c2.Position += c2.Direction * c3.MoveSpeedCurrent * Time.deltaTime;
                    agent.transform.forward = c2.Direction;
                    agent.transform.position = c2.Position;
                }*/
                if (scalar <= _scalarTolerance)
                {
                    if (agent.hasPath)
                    {
                        agent.ResetPath();
                    }

                    var proximitySlowFactor = 1f;
                    if (targetDirMag <_distanceSlowDown)
                        proximitySlowFactor = targetDirMag / _distanceSlowDown;

                    if (proximitySlowFactor < _minimumProximityFactor)
                        proximitySlowFactor = _minimumProximityFactor;

                    //c3.RotationSpeedCurrent = Mathf.Pow(Mathf.Clamp(1f - scalar, 0f, 1f), 0.2f) * c3.RotationSpeedMax;
                    c3.RotationSpeedCurrent = c3.RotationSpeedMax;
                    c2.Direction = Vector3.RotateTowards(c2.Direction, targetDir,
                        Time.deltaTime * c3.RotationSpeedCurrent, 0.0f);
                    c2.Direction.y = c2.Position.y;

                    agent.transform.forward = c2.Direction;
                    agent.transform.localRotation = Quaternion.Euler(0,agent.transform.eulerAngles.y, 0);
                    c2.Position += agent.transform.forward * c3.MoveSpeedCurrent * Time.deltaTime * proximitySlowFactor;

                    agent.transform.position = c2.Position;
                    if (scalar > _scalarTolerance)
                    {
                        agent.enabled = true;
                        c3.RotationSpeedCurrent = 0;
                    }
                }
                else
                {
                    agent.enabled = true;

                    if (!agent.hasPath)
                    {
                        Vector3[] corners = new Vector3[2];

                        agent.velocity = c2.EffectiveVelocity;
                        agent.ResetPath();
                        NavMeshPath path = new NavMeshPath();
                        agent.CalculatePath(c1.Target, path);

                        if (path.corners.Length == 0)
                        {
                            Debug.LogWarning("zero path reseting...");
                            agent.ResetPath();

                        }
                        else
                        {
                            agent.SetPath(path);
                        }
                    }

                }

                if (agent.hasPath)
                {
                    c2.Position = agent.transform.position;
                    c2.Direction = agent.transform.forward;
                }



                /*c3.RotationSpeedCurrent = Mathf.Pow(Mathf.Clamp(1f - scalar, 0f, 1f), 0.2f) * c3.RotationSpeedMax;
                c2.Direction = targetDir;
                c2.Position += c2.Direction * c3.MoveSpeedCurrent * Time.deltaTime;*/


                c2.Delta = c2.Position - c2.OldPosition;
                c2.EffectiveVelocity = c2.Delta / Time.deltaTime;
                c2.EffectiveVelocityMag = c2.EffectiveVelocity.magnitude;

            }
        }

    }
}