using Data;
using Models.Components;

namespace Services
{
    public class ComponentSourceAdapter : UnitAdapter<ComponentSource, SaveDataSource, ComponentSourceAdapter>
    {
        protected override SaveDataSource Save(ComponentSource c, int i)
        {
            return new SaveDataSource(i, c.Type, c.Amount);
        }

        protected override void Load(ref ComponentSource c1, SaveDataSource save)
        {
            c1.Amount = save.Amount;
            c1.Type = save.Type;
        }
    }
}