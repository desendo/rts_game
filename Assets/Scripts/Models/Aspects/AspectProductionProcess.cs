using UniRx;

namespace Models.Aspects
{
    public class AspectProductionProcess
    {
        public readonly ReactiveProperty<float> MaxTime = new ReactiveProperty<float>();
        public readonly ReactiveProperty<float> CurrentTime = new ReactiveProperty<float>();
        public readonly ReactiveProperty<bool> Ready = new ReactiveProperty<bool>();
        public string ResultId;

        public AspectProductionProcess()
        {
        }
    }
}