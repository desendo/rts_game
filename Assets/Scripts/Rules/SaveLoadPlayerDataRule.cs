using System.Collections.Generic;
using Data;
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

        public SaveLoadPlayerDataRule()
        {
            _playerProfileService = Container.Get<IPlayerProfileService>();
            _playerDataInitializers = Container.GetList<IDataHandler<PlayerData>>();
            _gameStateService = Container.Get<IGameStateService>();
            _messenger = Container.Get<GameMessenger>();

            _messenger.Subscribe<MainSignals.SaveGameRequest>(x => OnSaveDataRequest());
            _messenger.Subscribe<MainSignals.LoadGameRequest>(x => OnLoadDataRequest());
        }

        private void OnSaveDataRequest()
        {
            if (_gameStateService.CurrentState.Value == GameStateService.State.Loaded)
            {
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

                _playerProfileService.LoadProfile(data =>
                {
                    _playerDataInitializers
                        .ForEach(x => x.LoadData(data));

                    _gameStateService.SetState(GameStateService.State.Loaded);

                });
            }

        }
    }
}