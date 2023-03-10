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

        private static readonly Dictionary<string, Func<SensorBoolBase>> _dictionary = new Dictionary<string, Func<SensorBoolBase>>()
        {
            {SensorIsMoving.SensorId, () => new SensorIsMoving()}//is_moving
        };
        private readonly List<SensorBase> _sensors = new List<SensorBase>();

        public Animator StateMachineAnimator => _stateMachineAnimator;
        public List<string> SensorIds => _sensorIds;

        private void Awake()
        {
            foreach (var id in _sensorIds)
            {
                if (_dictionary.TryGetValue(id, out var constructor))
                {
                    _sensors.Add(constructor.Invoke());
                }
                else
                {
                    Debug.LogError($"missing id {id} in sensors pool");
                }
            }
        }

        public void Bind(EcsWorld world, int entity)
        {
            _sensors.ForEach(x=>x.BindAnimator(_stateMachineAnimator));
            _sensors.ForEach(x=>x.BindEntity(world, entity));
        }

        private void Update()
        {
            foreach (var sensorBase in _sensors)
            {
                sensorBase.Update();
            }
        }
    }

    public static class SensorExtensions
    {
        public static SensorBase AddTo(this SensorBase sensorBase, List<SensorBase> list)
        {
            list.Add(sensorBase);
            return sensorBase;
        }
    }
}
