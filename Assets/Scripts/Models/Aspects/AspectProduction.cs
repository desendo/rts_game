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
        public int Id;

        public AspectProduction()
        {
        }

        public AspectProduction(SaveDataProductionSchema save)
        {
            ProductionVariants = new ProductionVariant[save.ProductionVariants.Length];
            for (var i = 0; i < save.ProductionVariants.Length; i++)
            {
                Id = i;
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