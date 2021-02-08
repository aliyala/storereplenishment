using StoreReplanishment.Application;
using StoreReplenishment.Domain;
using System.Collections.Generic;
using Xunit;

namespace StoreReplenishment.Test
{
    public class OrderServiceTest
    {
        private readonly List<Product> products = new List<Product>
        {
            new Product { Code = "P1", Name = "Milk", Price = 1.99m },
            new Product { Code = "P2", Name = "Soure Milk", Price = 2.05m },
            new Product { Code = "P3", Name = "Cream", Price = 3.59m },
            new Product { Code = "P4", Name = "Yoghurt", Price = 4.99m },
            new Product { Code = "P5", Name = "Buttermilk", Price = 3.1m }
        };

        private readonly List<BatchSize> batchSizes = new List<BatchSize>
        {
            new BatchSize { Code = "BS1", Size = 20 },
            new BatchSize { Code = "BS2", Size = 30 },
            new BatchSize { Code = "BS3", Size = 40 },
            new BatchSize { Code = "BS4", Size = 50 },
            new BatchSize { Code = "BS5", Size = 100 },
            new BatchSize { Code = "BS6", Size = 20 },
            new BatchSize { Code = "BS7", Size = 50 }
        };

        private readonly List<ProductBatchSize> productBatchSizes = new List<ProductBatchSize>
        {
            new ProductBatchSize { ProductCode = "P1", BatchSizeCode = "BS6" },
            new ProductBatchSize { ProductCode = "P2", BatchSizeCode = "BS1" },
            new ProductBatchSize { ProductCode = "P2", BatchSizeCode = "BS2" },
            new ProductBatchSize { ProductCode = "P2", BatchSizeCode = "BS3" },
            new ProductBatchSize { ProductCode = "P3", BatchSizeCode = "BS4" },
            new ProductBatchSize { ProductCode = "P3", BatchSizeCode = "BS5" },
            new ProductBatchSize { ProductCode = "P5", BatchSizeCode = "BS7" },
        };

        private readonly List<BatchQuantity> batchQuantities = new List<BatchQuantity>
        {
            new BatchQuantity { ProductCode = "P1", Quantity = 20 },
            new BatchQuantity { ProductCode = "P2", Quantity = 500 },
            new BatchQuantity { ProductCode = "P3", Quantity = 40 },
            new BatchQuantity { ProductCode = "P4", Quantity = 234 }
        };

        [Fact]
        public void ProduceOrderWithMaxBatchShouldReturnCorrectData()
        {
            var expectedOrders = new List<Order>
            {
                new Order("P1", "BS6", "Milk", 20, 20, 1.99m),
                new Order("P2", "BS3", "Soure Milk", 40, 500, 2.05m),
                new Order("P3", "BS5", "Cream", 100, 40, 3.59m),
                new Order("P4", "BS_GENERATED_P4", "Yoghurt", 1, 234, 4.99m),
                new Order("P5", "BS7", "Buttermilk", 50, 1, 3.1m)
            };

            var orderService = new OrderService();
            var result = orderService.ProduceOrder(products, batchSizes, productBatchSizes, batchQuantities, true);

            Assert.Equal(expectedOrders.Count, result.Count);

            for(var i = 0; i < expectedOrders.Count; i++)
            {
                Assert.Equal(expectedOrders[i].ProductCode, result[i].ProductCode);
                Assert.Equal(expectedOrders[i].ProductName, result[i].ProductName);
                Assert.Equal(expectedOrders[i].BatchSizeCode, result[i].BatchSizeCode);
                Assert.Equal(expectedOrders[i].BatchSize, result[i].BatchSize);
                Assert.Equal(expectedOrders[i].BatchQuantity, result[i].BatchQuantity);
                Assert.Equal(expectedOrders[i].Price, result[i].Price);
            };
        }
    }
}
