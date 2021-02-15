using StoreReplenishment.Domain;
using System.Collections.Generic;

namespace StoreReplanishment.Application
{
    public interface IOrderService
    {
        IList<Order> ProduceOrders(
            Product[] products,
            BatchSize[] batchSizes,
            ProductBatchSize[] productBatchSizes,
            BatchQuantity[] batchQuantities,
            bool batchSizeSelection);

        IList<Order> ProduceOrders(
            Product product,
            BatchSize[] batchSizes,
            ProductBatchSize[] productBatchSizes,
            int amount);
    }
}
