using System.Collections.Generic;
using Services.StorageHandler;
using Unity.Mathematics;
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
        public List<UnitSaveData> UnitsSaveData = new List<UnitSaveData>();
    }

    [System.Serializable]
    public class UnitSaveData
    {
        public string UnitId;
        public Vector3 Position;
        public float Rotation;
        public string ConfigId;
        public int PlayerIndex;
        public string ViewId;
    }
}