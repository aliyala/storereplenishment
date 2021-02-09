using StoreReplenishment.Domain;

namespace StoreReplenishmentApi.Dto
{
    public record ProductData
    {
        public Product[] Products { get; init; }

        public BatchSize[] BatchSizes { get; init; }

        public ProductBatchSize[] ProductBatchSizes { get; init; }

        public BatchQuantity[] BatchQuantities { get; init; }

        public bool BatchSizeSelection { get; init; }

        public ProductData(Product[] products, BatchSize[] batchSizes, ProductBatchSize[] productBatchSizes, 
            BatchQuantity[] batchQuantities, bool batchSizeSelection)
        {
            Products = products;
            BatchSizes = batchSizes;
            ProductBatchSizes = productBatchSizes;
            BatchQuantities = batchQuantities;
            BatchSizeSelection = batchSizeSelection;
        }
    }
}
