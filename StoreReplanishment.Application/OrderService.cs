using System;
using System.Collections.Generic;
using System.Linq;
using StoreReplenishment.Domain;

namespace StoreReplanishment.Application
{
    public class OrderService : IOrderService
    {
        private const string GeneratedBatchSize = "BS_GENERATED_";

        public IList<Order> ProduceOrders(
            Product[] products,
            BatchSize[] batchSizes,
            ProductBatchSize[] productBatchSizes,
            BatchQuantity[] batchQuantities,
            bool batchSizeSelection)
        {
            var orders = new List<Order>();
            var productBatchSizesDict = productBatchSizes
                .GroupBy(pbs => pbs.ProductCode)
                .ToDictionary(gr => gr.Key, gr => gr.Select(pbs => pbs.BatchSizeCode).ToList());
            var batchQuantitiesDict = batchQuantities.ToDictionary(bq => bq.ProductCode, bq => bq.Quantity);
            var batchSizesDict = batchSizes.ToDictionary(bs => bs.Code, bs => bs);

            foreach(var product in products)
            {
                var productBatchSizesExist = productBatchSizesDict.TryGetValue(product.Code, out var availableProductBatchSizes);

                BatchSize batch;
                if (productBatchSizesExist)
                {
                    var bsizes = availableProductBatchSizes?.Select(pbs => batchSizesDict[pbs]).OrderBy(bs => bs.Size).ToList();
                    if(bsizes is null || !bsizes.Any())
                    {
                        throw new ArgumentException($"Batch sizes for product {product.Code} not found");
                    }
                    batch = batchSizeSelection ? bsizes.Last() : bsizes.First();
                }
                else
                {
                    batch = new BatchSize($"{GeneratedBatchSize}{product.Code}", 1);
                }

                var quantity = batchQuantitiesDict.ContainsKey(product.Code) ? batchQuantitiesDict[product.Code] : 1;
                orders.Add(new Order(product, batch, quantity));
            };

            return orders.ToList();
        }
    }
}
