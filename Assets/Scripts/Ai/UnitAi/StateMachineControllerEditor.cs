using UnityEditor;
using UnityEngine;

namespace Ai.UnitAi
{
    [CustomEditor(typeof(StateMachineController))]
    public class StateMachineControllerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (!GUILayout.Button("Actualize animator parameters"))
                return;

            var stateMachineController = (StateMachineController) target;
            if (stateMachineController.StateMachineAnimator is not null)
            {
                stateMachineController.SensorIds.Clear();
                foreach (var parameter in stateMachineController.StateMachineAnimator.parameters)
                    stateMachineController.SensorIds.Add(parameter.name);
                EditorUtility.SetDirty(stateMachineController);
            }
        }
    }
}