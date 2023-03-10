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
        public List<SaveDataProductionVariants> ComponentProductionSchemaSaveData = new List<SaveDataProductionVariants>();
        public List<SaveDataProductionQueue> ComponentProductionQueueSaveData = new List<SaveDataProductionQueue>();
        public List<SaveDataMoveTarget> ComponentMoveTargetSaveData = new List<SaveDataMoveTarget>();
        public List<SaveDataMoveTarget> ComponentMoveRotateTargetsData = new List<SaveDataMoveTarget>();
        public List<SaveDataMoveTarget> ComponentMoveAgentTargetsData = new List<SaveDataMoveTarget>();
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
    public class SaveDataMove
    {
        public int Id;
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
    public class SaveDataAttack
    {
        public int Id;
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
    public class SaveDataHealth
    {
        public int Id;
        public float Current;
        public float Max;

        public SaveDataHealth()
        {
        }

    }

    [System.Serializable]
    public class SaveDataMoveTarget
    {
        public Vector3 Target;
        public int Id;


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
    public class SaveDataProductionVariants
    {
        public int Id;
        public ProductionVariant[] ProductionVariants;
        public SaveDataProductionVariants()
        {
            ProductionVariants = new ProductionVariant[0];
        }

        public SaveDataProductionVariants(int id, ProductionVariantsConfig variantsConfig)
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


        public SaveDataProductionVariants(in int id, ComponentProductionSchema c1)
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