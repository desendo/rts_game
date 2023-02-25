using System.Collections.Generic;
using Data;
using Leopotam.EcsLite;
using Locator;
using Services;
using Signals;
using UnityEngine.SceneManagement;

namespace Rules
{
    public class GameLoadRule : IInit
    {
        private static readonly string LoaderSceneName = "LoaderScene";
        private static readonly string UISceneName = "UIScene";
        private static readonly string GameSceneName = "GameScene";
        private readonly GameConfigData _gameConfigData;
        private readonly List<IDataHandler<GameConfigData>> _gameConfigInitializers;
        private readonly IGameConfigService _gameConfigService;
        private readonly IGameStateService _gameStateService;

        private readonly GameMessenger _messenger;

        private readonly List<IDataHandler<PlayerData>> _playerDataInitializers;
        private readonly List<IDataHandler<LevelSaveData>> _levelSaveDataInitializers;
        private readonly List<ISpawn> _spawns;
        private readonly IPlayerProfileService _playerProfileService;

        private readonly ISceneLoadService _sceneLoadService;
        private readonly ILevelService _levelService;

        private EcsWorld _world;
        public GameLoadRule()
        {
            _sceneLoadService = Container.Get<ISceneLoadService>();
            _levelService = Container.Get<ILevelService>();
            _gameConfigService = Container.Get<IGameConfigService>();
            _gameConfigInitializers = Container.GetList<IDataHandler<GameConfigData>>();
            _playerProfileService = Container.Get<IPlayerProfileService>();
            _spawns = Container.GetList<ISpawn>();
            _gameStateService = Container.Get<IGameStateService>();
            _playerDataInitializers = Container.GetList<IDataHandler<PlayerData>>();
            _levelSaveDataInitializers = Container.GetList<IDataHandler<LevelSaveData>>();

            _world = Container.Get<EcsWorld>();

            _messenger = Container.Get<GameMessenger>();
            _messenger.Subscribe<MainSignals.ResetGameRequest>(x => Load(true));
        }

        public void Init()
        {
            _gameConfigService.LoadData(data =>
            {
                _gameConfigInitializers.ForEach(x => x.LoadData(data));
                Load();
            });
        }

        private void Load(bool isReset = false)
        {

            _gameStateService.SetState(GameStateService.State.Loading);
            _spawns.ForEach(x=>x.DeSpawnViews());
            _spawns.ForEach(x=>x.SetSpawned(false));

            _playerProfileService.LoadProfile(data =>
            {
                _playerDataInitializers.ForEach(x => x.LoadData(data));

                _sceneLoadService.LoadScene(GameSceneName, LoadSceneMode.Additive, () =>
                {
                    _sceneLoadService.LoadScene(UISceneName, LoadSceneMode.Additive, () =>
                    {
                        _sceneLoadService.UnloadScene(LoaderSceneName, () =>
                        {
                            _levelService.InitLevelSaveData();
                            var currentLevelData = _levelService.CurrentLevelSaveData.Value;
                            _levelSaveDataInitializers.ForEach(x => x.LoadData(currentLevelData));
                            _spawns.ForEach(x=>x.SpawnViews());
                            _spawns.ForEach(x=>x.SetSpawned(true));
                            _gameStateService.SetState(GameStateService.State.Loaded);
                            _messenger.Fire(new MainSignals.SaveGameRequest());
                        });

                    });
                });
            }, isReset);
        }
    }
}