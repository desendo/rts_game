using Data;
using Models.Components;

namespace Services
{
    public class ComponentMoveAdapter : UnitAdapter<ComponentMove, SaveDataMove, ComponentMoveAdapter>
    {
        protected override SaveDataMove Save(ComponentMove c, int i)
        {
            return new SaveDataMove(i, c);
        }

        protected override void Load(ref ComponentMove c1, SaveDataMove save)
        {
            c1.MoveAcc = save.Acceleration;
            c1.MoveSpeedCurrent = save.Speed;
            c1.RotationSpeedMax = save.RotationSpeed;
        }
    }
}