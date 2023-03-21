using System.Collections.Generic;
using Models.Components;
using Services;
using UnityEngine;

namespace Ai.UnitAi
{
    public class SensorJobTypeSwitch : SensorEnumSwitchBase
    {
        public static string SensorId = nameof(SensorJobTypeSwitch);
        public static readonly int _anyJob = Animator.StringToHash("ai_any_job");

        public static Dictionary<JobType, int> _params = new Dictionary<JobType, int>()
        {
            {JobType.Chop, Animator.StringToHash("ai_chop_job")},
            {JobType.Build, Animator.StringToHash("ai_build_job")},
            {JobType.Repair, Animator.StringToHash("ai_repair_job")},
            {JobType.Haul, Animator.StringToHash("ai_haul_job")},
            {JobType.Dig, Animator.StringToHash("ai_dig_job")},
        };

        private void SetAnimatorParam(int key, bool value)
        {
            animator.SetBool(key, value);
        }

        public override void Update()
        {
            if(!isSet)
                return;


            if (i.Has<ComponentAiMemory>(world))
            {
                var type = i.Get<ComponentAiMemory>(world).TargetJobType;

                foreach (var keyValuePair in _params)
                {
                    SetAnimatorParam(keyValuePair.Value, keyValuePair.Key == type);
                }
                SetAnimatorParam(_anyJob, type != JobType.None);
            }
            else
            {
                foreach (var keyValuePair in _params)
                {
                    SetAnimatorParam(keyValuePair.Value, false);
                }
                SetAnimatorParam(_anyJob, false);
            }
        }
        protected override void OnReset()
        {
            isSet = false;
        }
    }
}