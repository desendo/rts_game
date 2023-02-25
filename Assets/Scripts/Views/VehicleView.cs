using System;
using Models;
using Models.Aspects;
using Services;
using UnityEngine;
using UnityEngine.AI;
using UniRx;

namespace Views
{
    public class VehicleView : UnitView
    {
        [SerializeField] private NavMeshAgent _agent;
        private AspectUnit _aspectUnit;

        public override void Awake()
        {
            base.Awake();
            if (_agent == null)
                _agent = GetComponent<NavMeshAgent>();
            _agent.enabled = false;
        }

        public override void OnDespawned()
        {
            base.OnSpawned();
            _agent.enabled = false;
            _aspectUnit = null;
        }

        public override void Bind(UnitCompositionBase model)
        {
            base.Bind(model);
            var index = model.AspectUnit.UnitIndex;
            _aspectUnit = index.Get<AspectUnit>();
            var aspectMove = index.Get<AspectMove>();
            _agent.enabled = true;
            if (aspectMove != null)
            {

                _agent.acceleration = aspectMove.Acceleration.Value;
                _agent.speed = aspectMove.Speed.Value;
                _agent.angularSpeed = aspectMove.RotationSpeed.Value;
                _agent.autoBraking = false;
            }

            Filter<AspectMoveTarget>.Instance.OnAdd.Subscribe(tuple =>
            {
                if(tuple.Item1 == index)
                    _agent.SetDestination(tuple.Item2.Target);

            }).AddTo(_sup);
        }

        private void Update()
        {
            if(_aspectUnit == null)
                return;

            _aspectUnit.Position.Value = transform.position;
            _aspectUnit.Rotation.Value = transform.rotation;
        }
    }
}