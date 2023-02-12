using System;
using Data;
using Locator;
using Services.StorageHandler;

namespace Services
{


    public interface IDataHandler<in T>
    {
        void SaveData(T data);
        void LoadData(T data);
    }
    public interface IGameConfigService
    {
        void LoadData(Action<GameConfigData> callback);
    }
    public class GameConfigService : IGameConfigService
    {

        private readonly ScriptableObjectStorageHandler<GameConfigData> _scriptableObjectStorageHandler;

        public GameConfigService()
        {
            _scriptableObjectStorageHandler = Container.Get<ScriptableObjectStorageHandler<GameConfigData>>();

        }

        public void LoadData(Action<GameConfigData> callback)
        {
            _scriptableObjectStorageHandler.GetData(configData =>
            {
                callback?.Invoke(configData);

            });
        }
    }
}