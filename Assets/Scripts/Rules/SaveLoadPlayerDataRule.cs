using System.Collections.Generic;
using Data;
using Leopotam.EcsLite;
using Locator;
using Services;
using Signals;

namespace Rules
{
    public class SaveLoadPlayerDataRule
    {
        private readonly IPlayerProfileService _playerProfileService;
        private readonly List<IDataHandler<PlayerData>> _playerDataInitializers;
        private readonly IGameStateService _gameStateService;
        private readonly GameMessenger _messenger;
        private readonly List<IDataHandler<LevelSaveData>> _levelSaveDataInitializers;
        private List<IDataHandler<GameConfigData>> _gameConfigInitializers;
        private readonly ISceneLoadService _sceneLoadService;
        private readonly ILevelService _levelService;
        private IGameConfigService _gameConfigService;
        private readonly List<ISpawn> _spawns;
        private EcsWorld _world;

        public SaveLoadPlayerDataRule()
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
            _messenger.Subscribe<MainSignals.SaveGameRequest>(x => OnSaveDataRequest());
            _messenger.Subscribe<MainSignals.LoadGameRequest>(x => OnLoadDataRequest());
        }

        private void OnSaveDataRequest()
        {
            if (_gameStateService.CurrentState.Value == GameStateService.State.Loaded)
            {

                foreach (var service in _levelSaveDataInitializers)
                    service.SaveData(_levelService.CurrentLevelSaveData.Value);

                foreach (var service in _playerDataInitializers)
                    service.SaveData(_playerProfileService.CurrentPlayerData);

                _playerProfileService.SaveProfile();

            }
        }

        private void OnLoadDataRequest()
        {
            if (_gameStateService.CurrentState.Value == GameStateService.State.Loaded)
            {
                _gameStateService.SetState(GameStateService.State.Loading);

                if (_world.IsAlive())
                {
                    int[] e = new int[_world.GetEntitiesCount()];
                    _world.GetAllEntities(ref e);
                    foreach (var i in e)
                    {
                        _world.DelEntity(i);
                    }
                }
                else
                {
                    _world = new EcsWorld();
                }

                _spawns.ForEach(x=>x.DeSpawnViews());
                _spawns.ForEach(x=>x.SetSpawned(false));
                _playerProfileService.LoadProfile(data =>
                {
                    _playerDataInitializers.ForEach(x => x.LoadData(data));
                    _levelService.InitLevelSaveData();
                    var currentLevelData = _levelService.CurrentLevelSaveData.Value;
                    _levelSaveDataInitializers.ForEach(x => x.LoadData(currentLevelData));
                    _spawns.ForEach(x=>x.SpawnViews());
                    _spawns.ForEach(x=>x.SetSpawned(true));
                    _gameStateService.SetState(GameStateService.State.Loaded);

                });
            }
        }
    }
}