using System;
using System.Collections.Generic;
using Leopotam.EcsLite;
using UnityEngine;

namespace Ai.UnitAi
{
	public class StateMachineController : MonoBehaviour
    {
        [SerializeField] private Animator _stateMachineAnimator;
        [SerializeField] private List<string> _sensorIds;
        private readonly List<SensorBase> _sensors = new List<SensorBase>();
        private EcsWorld _world;
        private int _i;
        public Animator StateMachineAnimator => _stateMachineAnimator;
        public List<string> SensorIds => _sensorIds;
        private static readonly Dictionary<string, Func<SensorBase>> AllSensors = new Dictionary<string, Func<SensorBase>>()
        {
            {SensorIsMoving.SensorId, () => new SensorIsMoving()},
            {SensorJobTypeSwitch.SensorId, () => new SensorJobTypeSwitch()},
            {SensorHasTargetEntity.SensorId, () => new SensorHasTargetEntity()},
            {SensorMoveTargetReached.SensorId, () => new SensorMoveTargetReached()},
            {SensorSetDestination.SensorId, () => new SensorSetDestination()},
            {SensorLookingToTargetEntity.SensorId, () => new SensorLookingToTargetEntity()},
            {SensorHasMoveTarget.SensorId, () => new SensorHasMoveTarget()},
        };
        private static readonly Dictionary<AiStateAction, Action<EcsWorld, int>>
            StateActionsEnter = new Dictionary<AiStateAction, Action<EcsWorld, int>>()
        {
            {StateActionFindMoveTarget.Type, StateActionFindMoveTarget.OnEnter},
            {StateActionFindJobObject.Type, StateActionFindJobObject.OnEnter},
        };
        private static readonly Dictionary<AiStateAction, Action<float, EcsWorld, int>>
            StateActionsUpdate = new Dictionary<AiStateAction, Action<float, EcsWorld, int>>()
        {
            {StateActionRotateToEntityTarget.Type, StateActionRotateToEntityTarget.OnUpdate},
        };



        private void Awake()
        {
            foreach (var id in _sensorIds)
            {
                if (AllSensors.TryGetValue(id, out var constructor))
                {
                    var sensor = constructor.Invoke();
                    sensor.SetAnimator(_stateMachineAnimator);
                    _sensors.Add(sensor);

                }
                else
                    Debug.LogError($"missing id {id} in sensors pool");
            }

        }

        public void Bind(EcsWorld world, int entity)
        {
            this._world = world;
            this._i = entity;
            _sensors.ForEach(x=>x.Bind(world, entity));
        }

        private void Update()
        {
            foreach (var sensorBase in _sensors)
            {
                sensorBase.Update();
            }
        }

        public void OnUpdate(List<AiStateAction> actions)
        {
            var dt = Time.deltaTime;
            foreach (var aiStateAction in actions)
            {
                if(StateActionsUpdate.ContainsKey(aiStateAction))
                    StateActionsUpdate[aiStateAction].Invoke(dt, _world,_i);
            }
        }

        public void OnEnter(List<AiStateAction> actions)
        {
            foreach (var aiStateAction in actions)
            {
                if(StateActionsEnter.ContainsKey(aiStateAction))
                    StateActionsEnter[aiStateAction].Invoke(_world,_i);
            }
        }

        public void Reset()
        {
            foreach (var sensorBase in _sensors)
            {
                sensorBase.Reset();
            }
        }
    }
}
