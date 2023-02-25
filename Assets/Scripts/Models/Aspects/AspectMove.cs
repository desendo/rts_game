using Data;
using UniRx;

namespace Models.Aspects
{
    public class AspectMove
    {
        public readonly ReactiveProperty<float> Speed = new ReactiveProperty<float>();
        public readonly ReactiveProperty<float> RotationSpeed = new ReactiveProperty<float>();
        public readonly ReactiveProperty<float> Acceleration = new ReactiveProperty<float>();
        private int Id;

        public AspectMove(AspectMoveSaveData save)
        {
            Id = save.Id;
            Speed.Value = save.Speed;
            RotationSpeed.Value = save.RotationSpeed;
            Acceleration.Value = save.Acceleration;
        }
    }
}