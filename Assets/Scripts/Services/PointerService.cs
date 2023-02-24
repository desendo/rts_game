using Locator;
using UniRx;
using UnityEngine;


namespace Services
{
    public enum PointerState
    {
        Free,
        LeftButtonHold,
        MidButtonHold,
        RightButtonHold
    }

    public interface IPointerService
    {
        public void SetState(PointerState pointerState);
        public IReadOnlyReactiveProperty<PointerState> State { get; }
        public IReadOnlyReactiveProperty<Vector2> PointerPos { get; }
        public IReadOnlyReactiveProperty<Vector2> PrevPos { get; }
        public IReadOnlyReactiveProperty<Vector2> Delta { get; }
    }

    public class PointerService : ITick, IPointerService
    {
        public void SetState(PointerState pointerState)
        {
            _state.Value = pointerState;
        }

        public IReadOnlyReactiveProperty<PointerState> State => _state;
        public IReadOnlyReactiveProperty<Vector2> PointerPos => _pos;
        public IReadOnlyReactiveProperty<Vector2> PrevPos => _prevPos;
        public IReadOnlyReactiveProperty<Vector2> Delta => _delta;

        private readonly ReactiveProperty<PointerState> _state = new ReactiveProperty<PointerState>();
        private readonly ReactiveProperty<Vector2> _pos = new ReactiveProperty<Vector2>();
        private readonly ReactiveProperty<Vector2> _prevPos = new ReactiveProperty<Vector2>();
        private readonly ReactiveProperty<Vector2> _delta = new ReactiveProperty<Vector2>();

        public void Tick(float dt)
        {
            _pos.Value = Input.mousePosition;
            _delta.Value = _pos.Value - _prevPos.Value;
            if (Input.GetMouseButton(0))
            {
                if (_state.Value != Services.PointerState.LeftButtonHold)
                {
                    _prevPos.Value = _pos.Value;
                    _state.Value = Services.PointerState.LeftButtonHold;
                }

            }
            else if (Input.GetMouseButton(1))
            {
                if (_state.Value != Services.PointerState.RightButtonHold)
                {
                    _prevPos.Value = _pos.Value;
                    _state.Value = Services.PointerState.RightButtonHold;
                }

            }
            else if (Input.GetMouseButton(2))
            {
                if (_state.Value != Services.PointerState.MidButtonHold)
                {
                    _state.Value = Services.PointerState.MidButtonHold;
                }
                _prevPos.Value = _pos.Value;

            }
            else
            {
                _prevPos.Value = _pos.Value;
                _state.Value = Services.PointerState.Free;
            }
        }
    }
}
