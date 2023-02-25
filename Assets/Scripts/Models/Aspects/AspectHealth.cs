using Data;
using UniRx;

namespace Models.Aspects
{
    public class AspectHealth
    {
        public readonly ReactiveProperty<float> Max = new ReactiveProperty<float>();
        public readonly ReactiveProperty<float> Current = new ReactiveProperty<float>();

        public AspectHealth(SaveDataHealth data)
        {
            Max.Value = data.Max;
            Current.Value = data.Current;
        }
    }
}