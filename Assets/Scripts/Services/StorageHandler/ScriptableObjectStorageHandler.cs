using System;
using Data;
using Locator;
using UnityEditor;

namespace Services.StorageHandler
{
    public class ScriptableObjectStorageHandler<T> : IStorage, IStorageReadable<T>, IStorageWritable<T> where T : class, IData
    {
        private readonly DataContainer<T> _dataScriptableObject;


        public ScriptableObjectStorageHandler()
        {
            _dataScriptableObject = Container.Get<DataContainer<T>>();
        }

        public ConfigStorageType SourceType => ConfigStorageType.ScriptableObject;

        public void GetData(Action<T> onGetData, T defaultData = default(T))
        {
            onGetData.Invoke(_dataScriptableObject.Data);
        }

        public void SetData(T data, Action<bool> onComplete = null)
        {
            _dataScriptableObject.Data = data;
#if UNITY_EDITOR
            EditorUtility.SetDirty(_dataScriptableObject);
#endif
        }
    }
}