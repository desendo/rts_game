using System.Collections.Generic;
using Models.Components;
using Services;
using UnityEngine;
using UnityEngine.AI;

namespace Views
{
    public class WorkerView : UnitView
    {
        [SerializeField] private NavMeshAgent _agent;
        [SerializeField] private Animator _animator;
        private static readonly int IsMoving = Animator.StringToHash("is_moving");

        public override void Awake()
        {
            base.Awake();
        }


        private void LateUpdate()
        {
            var c1 = _world.GetPool<ComponentTransform>().Get(_entity);

            UpdateWheels(c1.Direction, c1.OldDirection, c1.EffectiveVelocity);
            _animator.SetBool(IsMoving, c1.EffectiveVelocityMag > 0.01f);
        }

        private void UpdateWheels(Vector3 c1Direction, Vector3 c1OldDirection, Vector3 c1EffectiveVelocity)
        {
        }


        public override void Bind(int entity)
        {
            base.Bind(entity);
            if (!_entity.Has<ComponentWorker>(_world))
                _entity.Add<ComponentWorker>(_world);
            if (!_entity.Has<ComponentAiMemory>(_world))
                _entity.Add<ComponentAiMemory>(_world);
            ref var move = ref _world.GetPool<ComponentMove>().Get(_entity);
            _agent.acceleration = move.MoveAcc;
            _agent.speed = move.MoveSpeedCurrent;
            _agent.angularSpeed = move.RotationSpeedMax;
            _agent.autoBraking = true;

            if (!_entity.Has<ComponentNavAgent>(_world))
            {
                ref var cAgent = ref _entity.Add<ComponentNavAgent>(_world);
                cAgent.Agent = _agent;
            }

            if (!_entity.Has<ComponentMove>(_world))
            {
                Debug.LogError("no component move");
                return;
            }


        }

    }
}