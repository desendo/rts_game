using Locator;
using UniRx;
using UnityEngine;


namespace Services
{
    public enum PointerState
    {
        Free,
        SelectDraw,
        RotateCamera,
        DragCamera
    }

    public interface IPointerService
    {
        public Vector2 GetPosition();
        public IReadOnlyReactiveProperty<PointerState> PointerState { get; }
    }

    public class PointerService : ITick, IPointerService
    {
        public IReadOnlyReactiveProperty<PointerState> PointerState => _state;

        private readonly ReactiveProperty<PointerState> _state = new ReactiveProperty<PointerState>();

        public void Tick(float dt)
        {
        }

        public Vector2 GetPosition()
        {
            return Input.mousePosition;
        }

    }
}
