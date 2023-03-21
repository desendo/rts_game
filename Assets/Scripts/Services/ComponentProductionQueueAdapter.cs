using System.Linq;
using Data;
using Models.Components;

namespace Services
{
    public class ComponentProductionQueueAdapter : UnitAdapter<ComponentProductionQueue, SaveDataProductionQueue, ComponentProductionQueueAdapter>
    {
        protected override SaveDataProductionQueue Save(ComponentProductionQueue c, int i)
        {
            return new SaveDataProductionQueue(i, c);
        }

        protected override void Load(ref ComponentProductionQueue c1, SaveDataProductionQueue save)
        {
            c1.Queue = save.List.ToList();
        }
    }
}