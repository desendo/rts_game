using Data;
using Models.Components;

namespace Services
{
    public class ComponentMoveTargetAdapter : UnitAdapter<ComponentMoveTarget, SaveDataMoveTarget, ComponentMoveTargetAdapter>
    {

        protected override SaveDataMoveTarget Save(ComponentMoveTarget c, int i)
        {
            return new SaveDataMoveTarget(i, c.Target);
        }

        protected override void Load(ref ComponentMoveTarget c1, SaveDataMoveTarget save)
        {
            c1.Target = save.Target;
        }
    }
}