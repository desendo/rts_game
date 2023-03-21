using Data;
using Models.Components;

namespace Services
{
    public class ComponentProductionSchemaAdapter : UnitAdapter<ComponentProductionSchema, SaveDataProductionSchema, ComponentProductionSchemaAdapter>
    {
        protected override SaveDataProductionSchema Save(ComponentProductionSchema c, int i)
        {
            return new SaveDataProductionSchema(i, c);
        }

        protected override void Load(ref ComponentProductionSchema c1, SaveDataProductionSchema save)
        {
            c1.Variants = save.ProductionVariants;

        }
    }
}