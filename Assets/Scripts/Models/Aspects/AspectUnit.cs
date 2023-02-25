using System.Collections.Generic;
using Data;
using UniRx;
using UnityEngine;

namespace Models.Components
{
    public struct ComponentUnit
    {
        public int UnitIndex;
        public int PlayerIndex;
        public bool IsUser;
        public string ConfigId;
    }
    public struct ComponentSelection
    {
        public bool Selected;
        public bool Selectable;
        public bool Hovered;
    }
    public struct ComponentMove
    {
        public float MoveSpeed;
        public float RotationSpeed;
        public float MoveAcc;
    }
    public struct ComponentMoveTarget
    {
        public Vector3 Target;
    }
    public struct ComponentProductionSchema
    {
        public ProductionVariant[] Variants;
    }
    public struct ComponentProductionQueue
    {
        public List<string> Queue;
    }
    public struct ComponentTransform
    {
        public Vector3 Position;
        public Quaternion Rotation;

        public ComponentTransform(Vector3 position, Quaternion rotation)
        {
            Position = position;
            Rotation = rotation;
        }
    }
}

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

        public AspectUnit(SaveDataUnit save)
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