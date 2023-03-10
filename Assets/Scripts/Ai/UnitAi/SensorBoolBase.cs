
using UnityEngine;

namespace Ai.UnitAi
{
    public abstract class SensorBoolBase : SensorBase
    {
        protected Animator animator;

        public override void BindAnimator(Animator stateMachineAnimator)
        {
            animator = stateMachineAnimator;
        }

    }
}