using System;
using Locator;
using UnityEngine;

namespace Services.StorageHandler
{
    public class PlayerPrefsStorageHandler<T> : IStorage, IStorageReadable<T>, IStorageSavable<T>, IStorageGetVersion where T:class, new()
    {
        private readonly string _playerPrefsDataKey;
        private readonly string _playerPrefsVersionKey;
        private const int errorVersion = -2;
        private const string dataPostfix = "Data";
        private const string versionPostfix = "Version";

        public ConfigStorageType SourceType => ConfigStorageType.PlayerPrefs;


        private readonly IJsonService _jsonService;

        public PlayerPrefsStorageHandler()
        {
            _jsonService = Container.Get<IJsonService>();
            _playerPrefsDataKey = typeof(T).Name+dataPostfix;
            _playerPrefsVersionKey = typeof(T).Name+versionPostfix;
        }

        public void GetVersion(Action<int> onComplete = null, bool ignore = false)
        {
            if (ignore)
            {
                onComplete?.Invoke(-1);
                return;
            }

            if (!PlayerPrefs.HasKey(_playerPrefsVersionKey))
            {
                onComplete?.Invoke(errorVersion);
                return;
            }
            onComplete?.Invoke(PlayerPrefs.GetInt(_playerPrefsVersionKey));
        }

        public void SetVersion(int version)
        {
            PlayerPrefs.SetInt(_playerPrefsVersionKey, version);
        }
        public void GetData(Action<T> onGetData, T defaultData = default(T))
        {
            if (!HasData())
            {
                onGetData.Invoke(defaultData);
                return;
            }

            var obj = PlayerPrefs.GetString(_playerPrefsDataKey);
            if (string.IsNullOrEmpty(obj))
            {
                onGetData.Invoke(defaultData);
                return;
            }


            if (_jsonService.FromJson<T>(obj, out var data))
            {
                onGetData.Invoke(data);
            }
            else
            {
                onGetData.Invoke(defaultData);
            }


        }
        public void SaveData(T data, Action<bool> onComplete = null)
        {
            if (data == null)
            {
                onComplete?.Invoke(true);
                return;
            }

            var v = -1;
            if (data is IVersion version)
            {
                v = version.Version;
                SetVersion(version.Version);
            }

            var dataSerialized = _jsonService.ToJson(data);


            PlayerPrefs.SetString(_playerPrefsDataKey,dataSerialized);
            PlayerPrefs.Save();
            onComplete?.Invoke(true);

        }
        public bool HasData()
        {
            return PlayerPrefs.HasKey(_playerPrefsDataKey);
        }

        public void Clear()
        {
            PlayerPrefs.DeleteKey(_playerPrefsDataKey);
            PlayerPrefs.DeleteKey(_playerPrefsVersionKey);

        }
    }
}