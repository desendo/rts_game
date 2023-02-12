using System;
using UniRx;

namespace Services
{

    public interface IGameStateService
    {
        void SetState(GameStateService.State state);
        IReadOnlyReactiveProperty<GameStateService.State> CurrentState { get; }
    }
    public class GameStateService : IGameStateService
    {
        public enum  State
        {
            None = 0,
            Loading = 1,
            Loaded = 2
        }

        public IReadOnlyReactiveProperty<State> CurrentState => _state;
        private readonly ReactiveProperty<State> _state = new ReactiveProperty<State>();
        public void SetState(State state) => _state.Value = state;

    }
}