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
        public override void Awake()
        {
            base.Awake();
            if (_agent == null)
                _agent = GetComponent<NavMeshAgent>();
        }

        public override void Bind(UnitCompositionBase model)
        {
            base.Bind(model);
            var index = model.AspectUnit.UnitIndex;
            var aspectMove = index.Get<AspectMove>();
            if (aspectMove != null)
            {
                _agent.acceleration = aspectMove.Acceleration.Value;
                _agent.speed = aspectMove.Speed.Value;
                _agent.angularSpeed = aspectMove.RotationSpeed.Value;
            }

            Filter<AspectMoveTarget>.Instance.OnAdd.Subscribe(tuple =>
            {
                if(tuple.Item1 == index)
                    _agent.SetDestination(tuple.Item2.Target);

            }).AddTo(_sup);
        }
    }
}