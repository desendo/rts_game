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
    public class LocalizationConfig : ConfigElementBase
    {
        public string Rus;
        public string En;
    }

}