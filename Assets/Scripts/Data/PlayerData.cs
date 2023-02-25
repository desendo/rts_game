using System.Collections.Generic;
using System.Linq;
using Models;
using Models.Aspects;
using Models.Components;
using Services.StorageHandler;
using UnityEngine;
using UnityEngine.Serialization;

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
        public List<SaveDataUnit> ComponentUnitSaveData = new List<SaveDataUnit>();
        public List<SaveDataMove> ComponentMoveSaveData = new List<SaveDataMove>();
        public List<SaveDataAttack> ComponentAttackSaveData = new List<SaveDataAttack>();
        public List<SaveDataHealth> ComponentHealthSaveData = new List<SaveDataHealth>();
        public List<SaveDataProductionSchema> ComponentProductionSchemaSaveData = new List<SaveDataProductionSchema>();
        public List<SaveDataProductionQueue> ComponentProductionQueueSaveData = new List<SaveDataProductionQueue>();
        public List<SaveDataMoveTarget> ComponentMoveTargetSaveData = new List<SaveDataMoveTarget>();
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
    public class SaveDataUnit
    {
        public int Id;
        public string ConfigId;
        public int PlayerIndex;
        public Vector3 Position;
        public float Rotation;
        public bool Selected;
        public bool Selectable;

        public SaveDataUnit(int id, string configId, int playerIndex, Vector3 position, float rotation)
        {
            Id = id;
            ConfigId = configId;
            PlayerIndex = playerIndex;
            Position = position;
            Rotation = rotation;
        }

        public SaveDataUnit(int id, AspectUnit aspectUnit)
        {
            Id = id;
            ConfigId = aspectUnit.ConfigId;
            PlayerIndex = aspectUnit.PlayerIndex;
            Position = aspectUnit.Position.Value;
            Rotation = aspectUnit.Rotation.Value.eulerAngles.y;
        }

        public SaveDataUnit()
        {
        }
    }
    [System.Serializable]
    public class SaveDataMove
    {
        public int Id;
        public float Speed;
        public float RotationSpeed;
        public float Acceleration;

        public SaveDataMove(int i, AspectMoveConfig moveConfig)
        {
            Id = i;
            Speed = moveConfig.Speed;
            RotationSpeed = moveConfig.RotationSpeed;
            Acceleration = moveConfig.Acceleration;
        }

        public SaveDataMove()
        {
        }

        public SaveDataMove(in int i, ComponentMove move)
        {
            Id = i;
            Speed = move.MoveSpeed;
            RotationSpeed = move.RotationSpeed;
            Acceleration = move.MoveAcc;
            
        }
    }
    [System.Serializable]
    public class SaveDataAttack
    {
        public int Id;
        public float Damage;
        public float Delay;
        public SaveDataAttack()
        {
        }
        public SaveDataAttack(AspectAttackConfig config, int i)
        {
            Damage = config.Damage;
            Delay = config.Delay;
            Id = i;
        }
        public SaveDataAttack(AspectAttack aspectAttack, int i)
        {
            Damage = aspectAttack.Damage.Value;
            Delay = aspectAttack.Delay.Value;
            Id = i;
        }


    }
    [System.Serializable]
    public class SaveDataHealth
    {
        public int Id;
        public float Current;
        public float Max;

        public SaveDataHealth()
        {
        }

        public SaveDataHealth(int id, AspectHealthConfig config)
        {
            Id = id;
            Current = config.Max;
            Max = config.Max;
        }
        public SaveDataHealth(int id, AspectHealth health)
        {
            Id = id;
            Current = health.Current.Value;
            Max = health.Max.Value;
        }
    }

    [System.Serializable]
    public class SaveDataMoveTarget
    {
        public Vector3 Target;
        public int Id;
        public SaveDataMoveTarget(int id, AspectMoveTarget aspect)
        {
            Id = id;
            Target = aspect.Target;
        }

        public SaveDataMoveTarget()
        {
        }

        public SaveDataMoveTarget(in int id, ComponentMoveTarget component)
        {
            Id = id;
            Target = component.Target;
        }
    }

    [System.Serializable]
    public class SaveDataProductionQueue
    {
        public int Id;
        public string[] List;

        public SaveDataProductionQueue(int id, ComponentProductionQueue queue)
        {
            Id = id;
            List = queue.Queue.ToArray();
        }

        public SaveDataProductionQueue()
        {
            List = new string[0];
        }
    }

    [System.Serializable]
    public class SaveDataProductionSchema
    {
        public int Id;
        public ProductionVariant[] ProductionVariants;
        public SaveDataProductionSchema()
        {
            ProductionVariants = new ProductionVariant[0];
        }

        public SaveDataProductionSchema(int id, ProductionSchemaConfig schemaConfig)
        {
            Id = id;
            ProductionVariants = new ProductionVariant[schemaConfig.ProductionVariantConfigs.Count];
            for (var index = 0; index < schemaConfig.ProductionVariantConfigs.Count; index++)
            {
                var variantConfig = schemaConfig.ProductionVariantConfigs[index];
                ProductionVariants[index] = new ProductionVariant()
                {
                    PriceAmount = variantConfig.Price.Select(x=>x.Amount).ToArray(),
                    PriceType = variantConfig.Price.Select(x=>x.Type).ToArray(),
                    Duration = variantConfig.Duration,
                    ResultId = variantConfig.ResultId
                };
            }
        }
        public SaveDataProductionSchema(int id, AspectProduction aspect)
        {
            Id = id;
            ProductionVariants = new ProductionVariant[aspect.ProductionVariants.Length];

            for (var i = 0; i < aspect.ProductionVariants.Length; i++)
            {
                var variant = aspect.ProductionVariants[i];
                ProductionVariants[i] = new ProductionVariant()
                {
                    Duration = variant.Duration,
                    PriceAmount = variant.PricesAmount,
                    PriceType = variant.PricesTypes,
                    ResultId = variant.ResultId

                };

            }
        }

        public SaveDataProductionSchema(in int id, ComponentProductionSchema c1)
        {
            ProductionVariants = new ProductionVariant[c1.Variants.Length];

            for (var i = 0; i < c1.Variants.Length; i++)
            {
                var variant = c1.Variants[i];
                ProductionVariants[i] = new ProductionVariant()
                {
                    Duration = variant.Duration,
                    PriceAmount = variant.PriceAmount,
                    PriceType = variant.PriceType,
                    ResultId = variant.ResultId
                };

            }

        }
    }
    [System.Serializable]
    public class ProductionVariant
    {
        public int[] PriceAmount;
        public string[] PriceType;
        public float Duration;
        public string ResultId;
    }
}