using System;
using System.Collections.Generic;
using Data;
using Leopotam.EcsLite;
using UnityEngine;
using UnityEngine.AI;

namespace Models.Components
{
    public struct ComponentUnit
    {
        public int EntityIndex;
        public int PlayerIndex;
        public string ConfigId;
    }
    public struct ComponentInfo
    {
        public string Name;
        public string Title;
        public string Description;
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
    
    public struct ComponentChopTreeJobRequest
    {
        public Vector3 ClickPosition;
        public EcsPackedEntity Tree;
    }
    public struct ComponentSetDestinationRequest
    {
        public Vector3 Destination;
    }
    public struct ComponentAiMemory
    {
        public Vector3 LastPosition;
        public Vector3 TargetPosition;
        public bool HasNewTarget;
        public JobType TargetJobType;
        public float Timer;
        public EcsPackedEntity Target;
    }

    public enum JobType
    {
        None,
        Chop,
        Build,
        Repair,
        Haul,
        Dig,
        
        
    }

    public struct ComponentMoveTarget
    {
        public Vector3 Target;
    }
    public struct ComponentMoveTargetReached
    {
    }
    public struct ComponentNavAgent
    {
        public NavMeshAgent Agent;
    }

    public struct ComponentProductionSchema
    {
        public ProductionVariant[] Variants;
    }
    public struct ComponentSource
    {
        public ResourceType Type;
        public float Amount;
    }
    public struct ComponentRequiredJob
    {
        public JobType Type;
    }
    public enum ResourceType
    {
        None,
        Wood
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
    public struct ComponentWorker
    {
    }

    [Serializable]
    public class UnitTask
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
        public float EffectiveVelocityMag;
        public Vector3 OldDirection;
    }
}