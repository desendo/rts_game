using System;
using System.Collections.Generic;
using Models.Components;
using UnityEngine;
using UnityEngine.Serialization;

namespace Data
{
    [Serializable]
    public class GameConfigData : IData
    {
        public GeneralConfig GeneralConfig;
        public HealthConfig[] HealthConfigs;
        public AttackConfig[] AttackConfigs;
        public MoveConfig[] MoveConfigs;
        public ProductionVariantsConfig[] ProductionVariantsConfigs;
        public ResourceConfig[] ResourceConfigs;
        public ResultConfig[] ResultConfigs;
        public BuildConfig[] BuildConfigs;
    }

    [Serializable]
    public abstract class ConfigElementBase : IConfigElement
    {
        public string Id;
        string IConfigElement.Id => Id;
    }
    public interface IConfigElement
    {
        string Id { get; }
    }
    [Serializable]
    public class GeneralConfig
    {
        public float BorderScrollSpeed;
        public float CameraRotateSpeed;
        public float DragScrollSpeed;
    }

    [Serializable]
    public class HealthConfig : ConfigElementBase
    {
        public float Max;
    }
    [Serializable]
    public class AttackConfig : ConfigElementBase
    {
        public float Delay;
        public float Damage;
    }
    [Serializable]
    public class MoveConfig : ConfigElementBase
    {
        public float Speed;
        public float Acceleration;
        public float RotationSpeed;
    }
    [Serializable]
    public class ResultConfig : ConfigElementBase
    {
        public ResultType ResultType;
        public int ResultAmount;
    }
    [Serializable]
    public class BuildConfig : ConfigElementBase
    {
        public List<PricePair> Price = new List<PricePair>();
    }

    public enum ResultType
    {
        None,
        Unit,
        Resource,
        Upgrade
    }

    [Serializable]
    public class PricePair
    {
        public string Type;
        public int Amount;
    }

    [Serializable]
    public class ProductionVariantsConfig : ConfigElementBase
    {
        public List<ProductionVariantConfig> ProductionVariantConfigs = new List<ProductionVariantConfig>();
    }
    [Serializable]
    public class ResourceConfig : ConfigElementBase
    {
        public float Amount;
        public ResourceType Type;
    }
    [Serializable]
    public class ProductionVariantConfig
    {
        public List<PricePair> Price = new List<PricePair>();
        public float Duration;
        public string ResultId;
    }
}