using IdolTower.Data;
using UnityEngine;

namespace Data
{
    [System.Serializable]
    public class LocalizationData : IData
    {
        [SerializeField]
        public LocalizationConfig[] LocalizationConfigs;

    }

    [System.Serializable]
    public class LocalizationConfig
    {
    }
}