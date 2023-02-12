using Data;
using UniRx;
using UnityEngine;

namespace Models
{
    public class AspectTransform
    {
        public readonly ReactiveProperty<Vector3> Position = new ReactiveProperty<Vector3>();
        public readonly ReactiveProperty<Quaternion> Rotation = new QuaternionReactiveProperty();
        public void InitFromSaveData(UnitSaveData saveData)
        {
            Position.Value = saveData.Position;
            Rotation.Value = Quaternion.AngleAxis(saveData.Rotation, Vector3.up);
        }
    }
}