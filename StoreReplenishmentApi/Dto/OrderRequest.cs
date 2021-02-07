using StoreReplenishment.Domain;
using System.Collections.Generic;

namespace StoreReplenishmentApi.Dto
{
    public class OrderRequest
    {
        public List<Product> Products { get; set; }

        public List<BatchSize> BatchSizes { get; set; }

        public List<ProductBatchSize> ProductBatchSizes { get; set; }

        public List<BatchQuantity> BatchQuantities { get; set; }
            
        public bool BatchSizeSelection { get; set; }
    }
}
