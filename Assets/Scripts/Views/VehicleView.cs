using Models.Components;
using UnityEngine;
using UnityEngine.AI;

namespace Views
{
    public class VehicleView : UnitView
    {
        [SerializeField] private NavMeshAgent _agent;
        [SerializeField] private Animator _animator;
        [SerializeField] private Transform[] _wheels;
        [SerializeField] private Transform _moveDir;


        private static readonly int IsMoving = Animator.StringToHash("is_moving");
        private Transform _tr;
        [SerializeField] private float _wheelFactor;

        public override void Awake()
        {
            _tr = transform;
            base.Awake();
        }

        protected virtual void Update()
        {
            if (_world == null)
                return;

            if (!_world.GetPool<ComponentMove>().Has(_entity))
                return;
            if(!_world.GetPool<ComponentMoveTarget>().Has(_entity))
                return;

            ref var c1 = ref _world.GetPool<ComponentTransform>().Get(_entity);
            ref var c2 = ref _world.GetPool<ComponentMoveTarget>().Get(_entity);

            /*
            if (c2.Type == MoveTargetType.MoveTarget)
            {

                _agent.enabled = true;
                _agent.velocity = c1.EffectiveVelocity;
                _agent.ResetPath();

                _agent.SetDestination(c2.Target);
                c2.Type = MoveTargetType.NavAgent;

            }

            if (c2.Type != MoveTargetType.NavAgent)
            {
                if (_agent.enabled)
                    _agent.enabled = false;

                _tr.position = c1.Position;
                _tr.rotation = c1.Rotation;
                _tr.forward = c1.Direction;
                UpdateWheels(c1.Direction, c1.OldDirection, c1.EffectiveVelocity);

            }
            else
            {
                c1.Position = _tr.position;
                c1.Rotation = _tr.rotation;
                c1.Direction = _tr.forward;

                /*
                if (c1.Delta.sqrMagnitude > 0.0001f)
                {
                    _tr.forward = Vector3.Lerp(_tr.forward, new Vector3(c1.Delta.x,_tr.forward.y,c1.Delta.z), 0.5f);
                }
                #1#

                UpdateWheels(c1.Direction, c1.OldDirection, c1.EffectiveVelocity);

            }*/
            _animator.SetBool(IsMoving, c1.EffectiveVelocity.magnitude > 0.1f);

        }

        private void UpdateWheels(Vector3 dir, Vector3 forward, Vector3 vel)
        {
            var velMag = vel.magnitude;
            var angle = Vector3.SignedAngle(forward, dir, Vector3.up) * velMag / (_wheelFactor  * Time.deltaTime);

            foreach (var wheel in _wheels)
            {
                wheel.localRotation = Quaternion.Euler(0, angle, 0);
            }
        }

        public override void Bind(int entity)
        {
            base.Bind(entity);
            ref var move = ref _world.GetPool<ComponentMove>().Get(_entity);
            _agent.acceleration = move.MoveAcc;
            _agent.speed = move.MoveSpeedCurrent;
            _agent.angularSpeed = move.RotationSpeedMax;
            _agent.autoBraking = true;
            _agent.enabled = false;
            ref var c2 = ref _world.GetPool<ComponentTransform>().Get(entity);
            c2.Direction = _tr.forward;
        }

        public override void OnDespawned()
        {
            _agent.enabled = false;
            base.OnDespawned();
        }

        public override void OnSpawned()
        {
            _agent.enabled = false;
            base.OnSpawned();
        }
    }
}