using System.Collections.Generic;
using System.Linq;
using StoreReplenishment.Domain;

namespace StoreReplanishment.Application
{
    public class OrderService : IOrderService
    {
        public IList<Order> ProduceOrder(
            List<Product> products,
            List<BatchSize> batchSizes,
            List<ProductBatchSize> productBatchSizes,
            List<BatchQuantity> batchQuantities,
            bool batchSizeSelection)
        {
            var orders = new List<Order>();
            var productBatchSizesDict = productBatchSizes
                .GroupBy(pbs => pbs.ProductCode)
                .ToDictionary(gr => gr.Key, gr => gr.Select(pbs => pbs.BatchSizeCode).ToList());
            var batchQuantitiesDict = batchQuantities.ToDictionary(bq => bq.ProductCode, bq => bq.Quantity);
            var batchSizesDict = batchSizes.ToDictionary(bs => bs.Code, bs => bs);

            foreach (var product in products)
            {
                var productBatchSizesExist = productBatchSizesDict.TryGetValue(product.Code, out var availableProductBatchSizes);

                BatchSize batch;
                if (productBatchSizesExist)
                {
                    var bsizes = availableProductBatchSizes.Select(pbs => batchSizesDict[pbs]).OrderBy(bs => bs.Size).ToList();
                    // TODO if bsizes not found throw exception
                    batch = batchSizeSelection ? bsizes.Last() : bsizes.First();
                }
                else
                {
                    batch = new BatchSize { Code = $"BS_GENERATED_{product.Code}", Size = 1 };
                }

                var quantity = batchQuantitiesDict.ContainsKey(product.Code) ? batchQuantitiesDict[product.Code] : 1;
                orders.Add(new Order(product.Code, batch.Code, product.Name, batch.Size, quantity, product.Price));
            }

            return orders;
        }
    }
}
