using System;

namespace Services.StorageHandler
{

        public interface IStorage
        {
            ConfigStorageType SourceType { get; }
        }
        public interface IStorageReadable<T> where T:class
        {
            void GetData(Action<T> onGetData, T defaultData = null);
        }
        public interface IStorageSavable<in T> where T:class
        {
            void SaveData(T data, Action<bool> onComplete);
        }
        public interface IStorageWritable<in T>
        {
            void SetData(T data, Action<bool> onComplete = null);
        }
        public interface IStorageGetVersion
        {
            void GetVersion(Action<int> onComplete = null, bool ignore = false);
        }
        public enum ConfigStorageType
        {
            ScriptableObject,
            GoogleTables,
            BackendJson,
            PlayerPrefs,
            FileSystem
        }
        public interface IVersion
        {
            int Version { get; set; }
        }
}