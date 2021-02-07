using StoreReplenishment.Domain;
using System.Collections.Generic;

namespace StoreReplanishment.Application
{
    public interface IOrderService
    {
        IEnumerable<Order> ProduceOrder(
            List<Product> products,
            List<BatchSize> batchSizes,
            List<ProductBatchSize> productBatchSizes,
            List<BatchQuantity> batchQuantities,
            bool batchSizeSelection);
    }
}
