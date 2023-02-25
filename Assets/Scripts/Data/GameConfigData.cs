using System;
using System.Collections.Generic;

namespace Data
{
    [Serializable]
    public class GameConfigData : IData
    {
        public GeneralConfig GeneralConfig;
        public AspectHealthConfig[] AspectHealthConfigs;
        public AspectAttackConfig[] AspectAttackConfigs;
        public AspectMoveConfig[] AspectMoveConfigs;
        public ProductionSchemaConfig[] AspectProductionConfigs;
        public ResultConfig[] ResultConfigs;
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
    public class AspectHealthConfig : ConfigElementBase
    {
        public float Max;
    }
    [Serializable]
    public class AspectAttackConfig : ConfigElementBase
    {
        public float Delay;
        public float Damage;
    }
    [Serializable]
    public class AspectMoveConfig : ConfigElementBase
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
    public class ProductionSchemaConfig : ConfigElementBase
    {
        public List<ProductionVariantConfig> ProductionVariantConfigs = new List<ProductionVariantConfig>();

    }
    [Serializable]
    public class ProductionVariantConfig
    {
        public List<PricePair> Price = new List<PricePair>();
        public float Duration;
        public string ResultId;
    }
}