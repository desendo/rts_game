using Data;
using UniRx;
using UnityEngine;

namespace Models.Aspects
{
    public class AspectUnit
    {
        public int UnitIndex;
        public int PlayerIndex;
        public bool IsUser;
        public string ConfigId;
        public readonly ReactiveProperty<Vector3> Position = new ReactiveProperty<Vector3>();
        public readonly ReactiveProperty<Quaternion> Rotation = new QuaternionReactiveProperty();

        public AspectUnit(AspectUnitSaveData save)
        {
            UnitIndex = save.Id;
            PlayerIndex = save.PlayerIndex;
            ConfigId = save.ConfigId;
            Position.Value = save.Position;
            Rotation.Value = Quaternion.Euler(0,save.Rotation,0);
        }

        public AspectUnit()
        {
        }
    }
}