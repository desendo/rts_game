using UniRx;

namespace Models.Aspects
{
    public class AspectSelection
    {
        public readonly ReactiveProperty<bool> Selected = new ReactiveProperty<bool>();
        public readonly ReactiveProperty<bool> Hovered = new ReactiveProperty<bool>();
        public readonly ReactiveProperty<bool> Actioned = new ReactiveProperty<bool>();

        public AspectSelection()
        {
        }
    }
}