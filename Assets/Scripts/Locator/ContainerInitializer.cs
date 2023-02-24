using Data;
using Rules;
using Services;
using Services.StorageHandler;
using UnityEngine;

namespace Locator
{

    public class ContainerInitializer : MonoBehaviour
    {
        [SerializeField] private DataContainer<PlayerData> _playerData;
        [SerializeField] private DataContainer<GameConfigData> _gameConfigData;
        [SerializeField] private DataContainer<VisualData> _visualData;

        private static ContainerInitializer _instance;
        private void Awake()
        {
            if (_instance != null)
            {
                Destroy(gameObject);
                return;
            }
            DontDestroyOnLoad(this);
            _instance = this;

            AddDataContainers();
            AddServices();
            AddRules();

            Container.SetAddComplete(true);
            Container.Init();
        }

        private void AddDataContainers()
        {
            Container.Add<VisualData>(_visualData.Data);
            Container.Add<DataContainer<VisualData>>(_visualData);
            Container.Add<DataContainer<GameConfigData>>(_gameConfigData);
            Container.Add<GameConfigData>(_gameConfigData.Data);
            Container.Add<DataContainer<PlayerData>>(_playerData);
        }

        private static void AddRules()
        {
            Container.Add<GameLoadRule>();
            Container.Add<SaveLoadPlayerDataRule>();
            Container.Add<ClickRule>();
            Container.Add<UnitSelectedRule>();
            Container.Add<ProductionQueueRule>();
            Container.Add<StartProductionRule>();
            Container.Add<RunProductionRule>();
            Container.Add<CameraMoveRule>();
            Container.Add<DebugKeysRule>();
        }

        private static void AddServices()
        {
            Container.Add<GameMessenger>();
            Container.Add<JsonService>();

            Container.Add<PlayerPrefsStorageHandler<PlayerData>>();
            Container.Add<ScriptableObjectStorageHandler<PlayerData>>();
            Container.Add<ScriptableObjectStorageHandler<GameConfigData>>();

            Container.Add<PointerService>();
            Container.Add<CameraService>();
            Container.Add<GameConfigService>();
            Container.Add<GameStateService>();
            Container.Add<LevelService>();
            Container.Add<SceneLoadService>();
            Container.Add<PlayerProfileService>();
            Container.Add<UnitsService>();
        }



        private void Update()
        {
            Container.UpdateLoop(Time.deltaTime);
        }

        private void FixedUpdate()
        {
            Container.FixedUpdateLoop(Time.deltaTime);
        }
    }
}
