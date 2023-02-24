using System.Collections.Generic;
using System.Linq;
using Models;
using Models.Aspects;
using Services.StorageHandler;
using UnityEngine;

namespace Data
{
    [System.Serializable]
    public class PlayerData : IData, IVersion
    {
        [SerializeField]
        private int _currentVersion = -1;
        public LevelSaveData CurrentLevelSaveData;

        public PlayerData Copy()
        {
            var serialized = JsonUtility.ToJson(this);
            return JsonUtility.FromJson<PlayerData>(serialized);
        }

        public int Version
        {
            get => _currentVersion ;
            set => _currentVersion = value;
        }
    }

    [System.Serializable]
    public class LevelSaveData
    {
        public bool IsValid;
        public List<AspectUnitSaveData> AspectsUnitSaveData = new List<AspectUnitSaveData>();
        public List<AspectMoveSaveData> AspectsMoveSaveData = new List<AspectMoveSaveData>();
        public List<AspectAttackSaveData> AspectsAttackSaveData = new List<AspectAttackSaveData>();
        public List<AspectHealthSaveData> AspectsHealthSaveData = new List<AspectHealthSaveData>();
        public List<AspectProductionSaveData> AspectsProductionSaveData = new List<AspectProductionSaveData>();
        public List<AspectQueueSaveData> AspectsQueueSaveData = new List<AspectQueueSaveData>();
        public List<AspectMoveTargetSaveData> AspectsMoveTargetSaveData = new List<AspectMoveTargetSaveData>();
        public CameraSaveData CameraData;
    }

    [System.Serializable]
    public class CameraSaveData
    {
        public Vector3 Position;
        public float Height;
        public float AngleY;
        public float AngleX;
    }

    [System.Serializable]
    public class AspectUnitSaveData
    {
        public int Id;
        public string ConfigId;
        public int PlayerIndex;
        public Vector3 Position;
        public float Rotation;

        public AspectUnitSaveData(int id, string configId, int playerIndex, Vector3 position, float rotation)
        {
            Id = id;
            ConfigId = configId;
            PlayerIndex = playerIndex;
            Position = position;
            Rotation = rotation;
        }

        public AspectUnitSaveData(int id, AspectUnit aspectUnit)
        {
            Id = id;
            ConfigId = aspectUnit.ConfigId;
            PlayerIndex = aspectUnit.PlayerIndex;
            Position = aspectUnit.Position.Value;
            Rotation = aspectUnit.Rotation.Value.eulerAngles.y;
        }

        public AspectUnitSaveData()
        {
        }
    }
    [System.Serializable]
    public class AspectMoveSaveData
    {
        public int Id;
        public float Speed;
        public float RotationSpeed;
        public float Acceleration;

        public AspectMoveSaveData(int i, AspectMoveConfig moveConfig)
        {
            Id = i;
            Speed = moveConfig.Speed;
            RotationSpeed = moveConfig.RotationSpeed;
            Acceleration = moveConfig.Acceleration;
        }
        public AspectMoveSaveData(int i, AspectMove move)
        {
            Id = i;
            Speed = move.Speed.Value;
            RotationSpeed = move.RotationSpeed.Value;
            Acceleration = move.Acceleration.Value;
        }
        public AspectMoveSaveData()
        {
        }
    }
    [System.Serializable]
    public class AspectAttackSaveData
    {
        public int Id;
        public float Damage;
        public float Delay;
        public AspectAttackSaveData()
        {
        }
        public AspectAttackSaveData(AspectAttackConfig config, int i)
        {
            Damage = config.Damage;
            Delay = config.Delay;
            Id = i;
        }
        public AspectAttackSaveData(AspectAttack aspectAttack, int i)
        {
            Damage = aspectAttack.Damage.Value;
            Delay = aspectAttack.Delay.Value;
            Id = i;
        }


    }
    [System.Serializable]
    public class AspectHealthSaveData
    {
        public int Id;
        public float Current;
        public float Max;

        public AspectHealthSaveData()
        {
        }

        public AspectHealthSaveData(int id, AspectHealthConfig config)
        {
            Id = id;
            Current = config.Max;
            Max = config.Max;
        }
        public AspectHealthSaveData(int id, AspectHealth health)
        {
            Id = id;
            Current = health.Current.Value;
            Max = health.Max.Value;
        }
    }

    [System.Serializable]
    public class AspectMoveTargetSaveData
    {
        public Vector3 Target;
        public int Id;
        public AspectMoveTargetSaveData(int id, AspectMoveTarget aspect)
        {
            Id = id;
            Target = aspect.Target;
        }

        public AspectMoveTargetSaveData()
        {
        }
    }

    [System.Serializable]
    public class AspectQueueSaveData
    {
        public int Id;
        public string[] List;

        public AspectQueueSaveData(int id, AspectQueue queue)
        {
            Id = id;
            List = queue.List.ToArray();
        }

        public AspectQueueSaveData()
        {
            List = new string[0];
        }
    }

    [System.Serializable]
    public class AspectProductionSaveData
    {
        public int Id;
        public ProductionVariantSaveData[] ProductionVariants;
        public AspectProductionSaveData()
        {
            ProductionVariants = new ProductionVariantSaveData[0];
        }

        public AspectProductionSaveData(int id, AspectProductionConfig config)
        {
            Id = id;
            ProductionVariants = new ProductionVariantSaveData[config.ProductionVariantConfigs.Count];
            for (var index = 0; index < config.ProductionVariantConfigs.Count; index++)
            {
                var variantConfig = config.ProductionVariantConfigs[index];
                ProductionVariants[index] = new ProductionVariantSaveData()
                {
                    PriceAmount = variantConfig.Price.Select(x=>x.Amount).ToArray(),
                    PriceType = variantConfig.Price.Select(x=>x.Type).ToArray(),
                    Duration = variantConfig.Duration,
                    ResultId = variantConfig.ResultId
                };
            }
        }
        public AspectProductionSaveData(int id, AspectProduction aspect)
        {
            Id = id;
            ProductionVariants = new ProductionVariantSaveData[aspect.ProductionVariants.Length];

            for (var i = 0; i < aspect.ProductionVariants.Length; i++)
            {
                var variant = aspect.ProductionVariants[i];
                var prices = new List<PricePair>();
                for (var i1 = 0; i1 < variant.PricesAmount.Length; i1++)
                {
                    prices.Add(new PricePair()
                    {
                        Amount = variant.PricesAmount[i1],
                        Type = variant.PricesTypes[i1],

                    });
                }

                ProductionVariants[i] = new ProductionVariantSaveData()
                {
                    Duration = variant.Duration,
                    PriceAmount = variant.PricesAmount,
                    PriceType = variant.PricesTypes,
                    ResultId = variant.ResultId

                };

            }
        }

        [System.Serializable]
        public class ProductionVariantSaveData
        {
            public int[] PriceAmount;
            public string[] PriceType;
            public float Duration;
            public string ResultId;
        }
    }
}