using Data;
using UnityEngine;

namespace Models.Aspects
{
    public class AspectMoveTarget
    {
        public Vector3 Target;

        public AspectMoveTarget(AspectMoveTargetSaveData save)
        {
            Target = save.Target;
        }

        public AspectMoveTarget(Vector3 target)
        {
            Target = target;
        }
    }
}