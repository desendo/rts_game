using UniRx;

namespace Models
{
    public class AspectProduction
    {
        public readonly string Id;
        public readonly ReactiveProperty<float> MaxTime = new ReactiveProperty<float>();
        public readonly ReactiveProperty<float> CurrentTime = new ReactiveProperty<float>();
        public readonly ReactiveProperty<bool> IsRunning = new ReactiveProperty<bool>();
        public readonly string ResultId;
        public readonly string ResultType;
        public readonly float ResultAmount;

        public AspectProduction(string id, float duration, float current, string resultId, string resultType, float resultAmount)
        {
            Id = id;
            MaxTime.Value = duration;
            CurrentTime.Value = current;
            ResultId = resultId;
            ResultType = resultType;
            ResultAmount = resultAmount;
        }
    }
}