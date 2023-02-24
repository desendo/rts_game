using Data;

namespace Models.Aspects
{
    public struct ProductionVariant
    {
        public int[] PricesAmount;
        public string[] PricesTypes;
        public float Duration;
        public string ResultId;
    }
    public class AspectProduction
    {
        public ProductionVariant[] ProductionVariants;

        public AspectProduction()
        {
        }

        public AspectProduction(AspectProductionSaveData save)
        {
            ProductionVariants = new ProductionVariant[save.ProductionVariants.Length];
            for (var i = 0; i < save.ProductionVariants.Length; i++)
            {
                var variant = save.ProductionVariants[i];
                ProductionVariants[i] = new ProductionVariant()
                {
                    Duration = variant.Duration,
                    PricesAmount = variant.PriceAmount,
                    PricesTypes = variant.PriceType,
                    ResultId = variant.ResultId
                };
            }
        }
    }
}