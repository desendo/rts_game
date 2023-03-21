using System.Collections.Generic;
using System.Linq;
using Models;
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

        public int CurrentPlayerIndex;
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
        public List<SaveDataMoveTarget> ComponentMoveRotateTargetsData = new List<SaveDataMoveTarget>();
        public List<SaveDataMoveTarget> ComponentMoveAgentTargetsData = new List<SaveDataMoveTarget>();
        public List<SaveDataMoveTarget> ComponentMoveSimpleTargetsData = new List<SaveDataMoveTarget>();
        public List<SaveDataSource> ComponentSourceSaveData = new List<SaveDataSource>();
        public CameraSaveData CameraData;
        public List<SaveDataAiMemory> ComponentAiMemorySaveData = new List<SaveDataAiMemory>();
        public List<SaveDataRequiredJob> ComponentRequiredJobSaveData = new List<SaveDataRequiredJob>();
    }

    [System.Serializable]
    public class CameraSaveData
    {
        public Vector3 Position;
        public float Height;
        public float AngleY;
        public float AngleX;
    }

    public class SaveDataEntityBase : ISaveDataEntity
    {
        public int Id { get; set; }
    }

    public interface ISaveDataEntity
    {
        public int Id { get; set; }
    }

    [System.Serializable]
    public class SaveDataAiMemory : SaveDataEntityBase
    {
        public Vector3 LastPosition;
        public Vector3 MoveTargetPosition;
        public bool HasNewTarget;
        public JobType TargetJobType;
        public float Timer;
        public int TargetEntity;
    }

    [System.Serializable]
    public class SaveDataUnit : SaveDataEntityBase
    {
        public string ConfigId;
        public int PlayerIndex;
        public Vector3 Position;
        public float Rotation;
        public bool Selected;
        public bool Selectable;
        public Vector3 Direction;
        public Vector3 EffectiveVelocity;

        public SaveDataUnit(int id, string configId, int playerIndex, Vector3 position, float rotation)
        {
            Id = id;
            ConfigId = configId;
            PlayerIndex = playerIndex;
            Position = position;
            Rotation = rotation;
        }


        public SaveDataUnit()
        {
        }
    }
    [System.Serializable]
    public class SaveDataMove  : SaveDataEntityBase
    {
        public float Speed;
        public float RotationSpeed;
        public float Acceleration;


        public SaveDataMove()
        {
        }

        public SaveDataMove(in int i, ComponentMove move)
        {
            Id = i;
            Speed = move.MoveSpeedCurrent;
            RotationSpeed = move.RotationSpeedMax;
            Acceleration = move.MoveAcc;

        }

        public SaveDataMove(MoveConfig moveConfig,  int move)
        {
            Id = move;
            Speed = moveConfig.Speed;
            RotationSpeed = moveConfig.RotationSpeed;
            Acceleration = moveConfig.Acceleration;

        }
    }
    [System.Serializable]
    public class SaveDataAttack : SaveDataEntityBase
    {
        public float Damage;
        public float Delay;
        public SaveDataAttack()
        {
        }
        public SaveDataAttack(AttackConfig config, int i)
        {
            Damage = config.Damage;
            Delay = config.Delay;
            Id = i;
        }

    }
    [System.Serializable]
    public class SaveDataRequiredJob : SaveDataEntityBase
    {
        public JobType Type;


    }
    [System.Serializable]
    public class SaveDataHealth : SaveDataEntityBase
    {
        public float Current;
        public float Max;

        public SaveDataHealth()
        {
        }

    }
    [System.Serializable]
    public class SaveDataSource : SaveDataEntityBase
    {
        public ResourceType Type;
        public float Amount;

        public SaveDataSource()
        {
        }

        public SaveDataSource(in int id, ResourceType type, float amount)
        {
            Id = id;
            Type = type;
            Amount = amount;
        }
    }
    [System.Serializable]
    public class SaveDataMoveTarget : SaveDataEntityBase
    {
        public Vector3 Target;

        public SaveDataMoveTarget()
        {
        }

        public SaveDataMoveTarget(in int id, Vector3 target)
        {
            Id = id;
            Target = target;
        }
    }

    [System.Serializable]
    public class SaveDataProductionQueue : SaveDataEntityBase
    {
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
    public class SaveDataProductionSchema : SaveDataEntityBase
    {
        public ProductionVariant[] ProductionVariants;
        public SaveDataProductionSchema()
        {
            ProductionVariants = new ProductionVariant[0];
        }

        public SaveDataProductionSchema(int id, ProductionVariantsConfig variantsConfig)
        {
            Id = id;
            ProductionVariants = new ProductionVariant[variantsConfig.ProductionVariantConfigs.Count];
            for (var index = 0; index < variantsConfig.ProductionVariantConfigs.Count; index++)
            {
                var variantConfig = variantsConfig.ProductionVariantConfigs[index];
                ProductionVariants[index] = new ProductionVariant()
                {
                    PriceAmount = variantConfig.Price.Select(x=>x.Amount).ToArray(),
                    PriceType = variantConfig.Price.Select(x=>x.Type).ToArray(),
                    Duration = variantConfig.Duration,
                    ResultId = variantConfig.ResultId
                };
            }
        }


        public SaveDataProductionSchema(in int id, ComponentProductionSchema c1)
        {
            Id = id;
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