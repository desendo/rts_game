using Locator;
using UniRx;
using UnityEngine;


namespace Services
{
    public enum PointerMouseState
    {
        Free,
        LeftButtonHold,
        MidButtonHold,
        RightButtonHold,
        LeftClick,
        RightClick,
        MiddleCLick
    }
    public enum FunctionalState
    {
        Free,
        DragPanning,
        FrameSelecting
    }
    public enum UnitState
    {
        Free,
        ChooseBuildSite
    }

    public interface IReadOnlyPointerService
    {
        public IReadOnlyReactiveProperty<PointerMouseState> MouseState { get; }
        public IReadOnlyReactiveProperty<PointerMouseState> PrevMouseState { get; }
        public IReadOnlyReactiveProperty<FunctionalState> FunctionalState { get; }
        public IReadOnlyReactiveProperty<UnitState> UnitState { get; }
        public IReadOnlyReactiveProperty<Vector2> PointerPos { get; }
        public IReadOnlyReactiveProperty<Vector2> PrevPos { get; }
        public IReadOnlyReactiveProperty<Vector2> HoldDelta { get; }
        public IReadOnlyReactiveProperty<Vector2> Delta  { get; }
        float Sum { get;  }
        public IReadOnlyReactiveProperty<bool> IsHovered { get; }
    }

    public interface IPointerService : IReadOnlyPointerService
    {
        public void SetState(PointerMouseState pointerMouseState);
        public void SetUnitState(UnitState state);
        public void SetHovered(bool hovered);

        void SetFunctionalState(FunctionalState state);
    }

    public class PointerService : ITick, IPointerService
    {
        public void SetState(PointerMouseState pointerMouseState)
        {
            _mouseState.Value = pointerMouseState;
        }

        public void SetHovered(bool hovered)
        {
            _isHovered.Value = hovered;
        }

        public void SetFunctionalState(FunctionalState state)
        {
            _functionalState.Value = state;
        }
        public void SetUnitState(UnitState state)
        {
            _unitState.Value = state;
        }

        public IReadOnlyReactiveProperty<PointerMouseState> MouseState => _mouseState;
        public IReadOnlyReactiveProperty<PointerMouseState> PrevMouseState => _mouseStatePrev;
        public IReadOnlyReactiveProperty<FunctionalState> FunctionalState => _functionalState;
        public IReadOnlyReactiveProperty<UnitState> UnitState => _unitState;
        public IReadOnlyReactiveProperty<Vector2> PointerPos => _pos;
        public IReadOnlyReactiveProperty<Vector2> PrevPos => _prevPos;
        public IReadOnlyReactiveProperty<Vector2> HoldDelta => _sumDelta;
        public IReadOnlyReactiveProperty<Vector2> Delta => _delta;
        public float Sum => _dragSum;
        public IReadOnlyReactiveProperty<bool> IsHovered => _isHovered;

        private readonly ReactiveProperty<PointerMouseState> _mouseState = new ReactiveProperty<PointerMouseState>();
        private readonly ReactiveProperty<PointerMouseState> _mouseStatePrev = new ReactiveProperty<PointerMouseState>();
        private readonly ReactiveProperty<FunctionalState> _functionalState = new ReactiveProperty<FunctionalState>();
        private readonly ReactiveProperty<UnitState> _unitState = new ReactiveProperty<UnitState>();
        private readonly ReactiveProperty<Vector2> _pos = new ReactiveProperty<Vector2>();
        private readonly ReactiveProperty<Vector2> _prevPos = new ReactiveProperty<Vector2>();
        private readonly ReactiveProperty<Vector2> _sumDelta = new ReactiveProperty<Vector2>();
        private readonly ReactiveProperty<Vector2> _delta = new ReactiveProperty<Vector2>();
        private readonly ReactiveProperty<bool> _isHovered = new ReactiveProperty<bool>();

        private float _dragSum;
        private float _dragTolerance = 5f;

        public void Tick(float dt)
        {
            _delta.Value = _pos.Value - (Vector2)Input.mousePosition;
            _pos.Value = Input.mousePosition;
            _sumDelta.Value = _pos.Value - _prevPos.Value;

            if (!Input.GetMouseButton(0) && !Input.GetMouseButton(1) && !Input.GetMouseButton(2))
            {
                if (_mouseState.Value != Services.PointerMouseState.Free)
                {
                    _dragSum = 0;
                    _prevPos.Value = _pos.Value;
                    _sumDelta.Value = Vector2.zero;
                    _mouseStatePrev.Value = _mouseState.Value;
                    _mouseState.Value = Services.PointerMouseState.Free;
                    _functionalState.Value = Services.FunctionalState.Free;
                }

            }
            if (Input.GetMouseButton(0))
            {
                if (_mouseState.Value != Services.PointerMouseState.LeftButtonHold)
                {
                    _prevPos.Value = _pos.Value;
                    _mouseStatePrev.Value = _mouseState.Value;
                    _mouseState.Value = Services.PointerMouseState.LeftButtonHold;
                    _sumDelta.Value = Vector2.zero;
                    _dragSum = 0;
                    if (_functionalState.Value == Services.FunctionalState.Free && _unitState.Value == Services.UnitState.Free)
                    {
                        _functionalState.Value = Services.FunctionalState.FrameSelecting;
                    }
                }
                _dragSum = _sumDelta.Value.magnitude;
            }
            else if (Input.GetMouseButton(1))
            {
                if (_mouseState.Value != Services.PointerMouseState.RightButtonHold
                    && _dragTolerance < _sumDelta.Value.magnitude)
                {
                    _prevPos.Value = _pos.Value;
                    _mouseStatePrev.Value = _mouseState.Value;
                    _mouseState.Value = Services.PointerMouseState.RightButtonHold;
                    _sumDelta.Value = Vector2.zero;
                    _dragSum = 0;
                }


            }
            else if (Input.GetMouseButton(2))
            {
                if (_mouseState.Value != Services.PointerMouseState.MidButtonHold)
                {
                    _sumDelta.Value = Vector2.zero;
                    _mouseStatePrev.Value = _mouseState.Value;
                    _mouseState.Value = Services.PointerMouseState.MidButtonHold;
                    _dragSum = 0;
                }
                _prevPos.Value = _pos.Value;

                _dragSum = _sumDelta.Value.magnitude;
            }


            if (Input.GetMouseButtonUp(0))
            {
                _mouseStatePrev.Value = _mouseState.Value;
                _mouseState.Value = Services.PointerMouseState.LeftClick;
            }
            else if (Input.GetMouseButtonUp(1))
            {
                _mouseStatePrev.Value = _mouseState.Value;
                _mouseState.Value = Services.PointerMouseState.RightClick;
            }
            else if (Input.GetMouseButtonUp(2))
            {
                _mouseStatePrev.Value = _mouseState.Value;
                _mouseState.Value = Services.PointerMouseState.MiddleCLick;
            }
        }
    }
}
