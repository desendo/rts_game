using Data;
using Models.Components;

namespace Services
{
    public class ComponentRequiredJobAdapter : UnitAdapter<ComponentRequiredJob, SaveDataRequiredJob, ComponentRequiredJobAdapter>
    {
        protected override SaveDataRequiredJob Save(ComponentRequiredJob c, int i)
        {
            return new SaveDataRequiredJob()
            {
                Id = i, Type = c.Type
            };
        }

        protected override void Load(ref ComponentRequiredJob c1, SaveDataRequiredJob save)
        {
            c1.Type = save.Type;
        }
    }
}