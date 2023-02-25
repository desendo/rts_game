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

    }
}