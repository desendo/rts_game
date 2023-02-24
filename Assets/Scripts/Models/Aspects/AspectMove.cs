using UniRx;

namespace Models.Aspects
{
    public class AspectMove
    {
        public readonly ReactiveProperty<float> Speed = new ReactiveProperty<float>();
        public readonly ReactiveProperty<float> RotationSpeed = new ReactiveProperty<float>();
        public readonly ReactiveProperty<float> Acceleration = new ReactiveProperty<float>();
    }
}