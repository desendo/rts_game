using System;

namespace Data
{
    [Serializable]
    public class GameConfigData : IData
    {
        public GeneralConfig GeneralConfig;
        public UnitConfig[] UnitConfigs;
        public AspectHealthConfig[] AspectHealthConfigs;
        public AspectAttackConfig[] AspectAttackConfigs;
        public AspectMoveConfig[] AspectMoveConfigs;
        public AspectProductionConfig[] AspectProductionConfigs;
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
        public float DailyRewardLoopSize;
        public float MediumPerHard;
        public float SoftPerHard;
    }
    [Serializable]
    public class UnitConfig : ConfigElementBase
    {
        public string ViewId;
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
        public float RotationSpeed;
    }
    [Serializable]
    public class AspectProductionConfig : ConfigElementBase
    {
        public float Duration;
        public string ResultId;
        public string ResultType;
        public float ResultAmount;
    }
}