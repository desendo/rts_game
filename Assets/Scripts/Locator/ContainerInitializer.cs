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

            Bind();
            Container.SetBindComplete(true);
            Container.Init();
        }
        private void Bind()
        {
            AddDataContainers();
            AddServices();
            AddRules();
        }

        private void AddDataContainers()
        {
            Container.Add<DataContainer<VisualData>>(_visualData);
            Container.Add<DataContainer<GameConfigData>>(_gameConfigData);
            Container.Add<DataContainer<PlayerData>>(_playerData);
        }

        private static void AddRules()
        {
            Container.Add<GameLoadRule>();
            Container.Add<SaveLoadPlayerDataRule>();
            Container.Add<ClickRule>();
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
