using System.Collections.Generic;
using Data;
using UnityEngine;
using UnityEngine.AI;

namespace Models.Components
{
    public struct ComponentUnit
    {
        public int EntityIndex;
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
        public float MoveSpeedCurrent;
        public float MoveSpeedMax;
        public float RotationSpeedMax;
        public float RotationSpeedCurrent;
        public float MoveAcc;
    }
    public struct ComponentMoveTarget
    {
        public Vector3 Target;
    }
    public struct ComponentMoveTargetAgent
    {
        public Vector3 Target;
    }
    public struct ComponentMoveTargetSimple
    {
        public Vector3 Target;
    }
    public struct ComponentMoveRotateToTarget
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
    public struct ComponentProductionQueueUpdated
    {
    }
    public struct ComponentProductionProgressStarted
    {
    }
    public struct ComponentUnitProductionBuilding
    {
        public List<Vector3> Path;
    }
    public struct ComponentBuilder
    {
    }
    public struct ComponentBuildInProgress
    {
        public float ProgressCurrent;
        public float ProgressMax;
        public float ProgressNormalized;
    }
    public struct ComponentProductionRun
    {
        public float Current;
        public float Max;
        public string Result;
    }

    public struct ComponentTransform
    {
        public Vector3 Position;
        public Vector3 OldPosition;
        public Vector3 Direction;
        public Quaternion Rotation;
        public Vector3 Delta;
        public Vector3 EffectiveVelocity;
        public Vector3 OldDirection;
    }
}