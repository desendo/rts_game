using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

namespace Ai.UnitAi
{
    public class StateHolder : StateMachineBehaviour
    {
        public List<AiStateAction> Actions;

        private StateMachineController _stateMachineController;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (_stateMachineController == null)
                _stateMachineController = animator.gameObject.GetComponent<StateMachineController>();

            _stateMachineController.OnEnter(Actions);
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex,
            AnimatorControllerPlayable controller)
        {
            _stateMachineController.OnUpdate(Actions);
        }
    }


    public enum AiStateAction
    {
        FindMoveTarget,
        FindJobObject,
        RotateToEntityTarget,

    }
}
