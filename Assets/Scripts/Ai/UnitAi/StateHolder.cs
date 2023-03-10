using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

namespace Ai.UnitAi
{
    public class StateHolder : StateMachineBehaviour
    {
        public List<StateAction> ActionsUpdate;

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex,
            AnimatorControllerPlayable controller)
        {
            base.OnStateUpdate(animator, stateInfo, layerIndex, controller);
        }
    }

    [System.Serializable]
    public class StateAction
    {
        public string Id;
    }
}
