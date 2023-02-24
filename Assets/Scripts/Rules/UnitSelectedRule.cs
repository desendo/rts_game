using Models.Aspects;
using Services;
using UniRx;

namespace Rules
{
    public class UnitSelectedRule
    {
        public UnitSelectedRule()
        {
            Filter<AspectSelection>.Instance.OnAdd.Subscribe(OnAdd);
            Filter<AspectSelection>.Instance.OnRemove.Subscribe(OnRemove);
        }

        private void OnRemove((int, AspectSelection) obj)
        {
        }

        private void OnAdd((int, AspectSelection) obj)
        {
        }
    }
}