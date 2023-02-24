using Data;
using UniRx;

namespace Models.Aspects
{
    public class AspectAttack
    {
        public readonly ReactiveProperty<float> Delay = new ReactiveProperty<float>();
        public readonly ReactiveProperty<float> Damage = new ReactiveProperty<float>();

        public AspectAttack(AspectAttackSaveData data)
        {
            Delay.Value = data.Delay;
            Damage.Value = data.Damage;
        }
    }
}