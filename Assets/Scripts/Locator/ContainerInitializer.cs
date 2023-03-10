using Data;
using Leopotam.EcsLite;
using Rules;
using Rules.StateSystems;
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
        private EcsWorld _escWorld;
        private EcsSystems _systems;

        private void Awake()
        {
            if (_instance != null)
            {
                Destroy(gameObject);
                return;
            }
            DontDestroyOnLoad(this);
            _instance = this;

            InitEcs();

            AddDataContainers();
            AddServices();
            AddSystems();
            AddRules();

            Container.SetAddComplete(true);
            Container.Init();
        }

        private void InitEcs()
        {
            _escWorld = new EcsWorld();
            _systems = new EcsSystems(_escWorld);
            Container.AddExplicit(_escWorld);
            Container.AddExplicit(_systems);
        }

        private void AddSystems()
        {
            _systems
                .Add(new ClickSystem())
                .Add(new MoveSystem())
                .Add(new ProductionQueueSystem())
                .Add(new StartProductionSystem())
                .Add(new RunProductionSystem())
                .Add(new StartBuildSystem())
                //ai systems
                .Add(new StateIsMovingSystem())
#if UNITY_EDITOR
                .Add (new Leopotam.EcsLite.UnityEditor.EcsWorldDebugSystem ())
#endif
                .Init();

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
            _systems.Run();
            Container.UpdateLoop(Time.deltaTime);
        }

        private void FixedUpdate()
        {
            Container.FixedUpdateLoop(Time.deltaTime);
        }
    }
}
