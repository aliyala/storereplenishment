using StoreReplanishment.Application;
using StoreReplenishment.Domain;
using System;
using System.Collections.Generic;
using Xunit;

namespace StoreReplenishment.Test
{
    public class OrderServiceTest
    {
        private readonly Product[] products = new Product[]
        {
            new Product("P1", "Milk", 1.99m),
            new Product("P2", "Soure Milk", 2.05m),
            new Product("P3", "Cream", 3.59m),
            new Product("P4", "Yoghurt", 4.99m),
            new Product("P5", "Buttermilk", 3.1m )
        };

        private readonly BatchSize[] batchSizes = new BatchSize[]
        {
            new BatchSize("BS1", 20),
            new BatchSize("BS2", 30),
            new BatchSize("BS3", 40),
            new BatchSize("BS4", 50),
            new BatchSize("BS5", 100),
            new BatchSize("BS6", 20),
            new BatchSize("BS7", 50)
        };

        private readonly ProductBatchSize[] productBatchSizes = new ProductBatchSize[]
        {
            new ProductBatchSize("P1", "BS6"),
            new ProductBatchSize("P2", "BS1"),
            new ProductBatchSize("P2", "BS2"),
            new ProductBatchSize("P2", "BS3"),
            new ProductBatchSize("P3", "BS4"),
            new ProductBatchSize("P3", "BS5"),
            new ProductBatchSize("P5", "BS7"),
        };

        private readonly BatchQuantity[] batchQuantities = new BatchQuantity[]        {
            new BatchQuantity("P1", 20),
            new BatchQuantity("P2", 500),
            new BatchQuantity("P3", 40),
            new BatchQuantity("P4", 234)
        };

        [Fact]
        public void ProduceOrderWithMaxBatchShouldReturnCorrectData()
        {
            var expectedOrders = new List<Order>
            {
                new Order(new Product("P1", "Milk", 1.99m), new BatchSize("BS6", 20), 20),
                new Order(new Product("P2", "Soure Milk", 2.05m), new BatchSize("BS3", 40), 500),
                new Order(new Product("P3", "Cream", 3.59m), new BatchSize("BS5", 100), 40),
                new Order(new Product("P4", "Yoghurt", 4.99m), new BatchSize("BS_GENERATED_P4", 1), 234),
                new Order(new Product("P5", "Buttermilk", 3.1m), new BatchSize("BS7", 50), 1),
            };

            var orderService = new OrderService();
            var result = orderService.ProduceOrders(products, batchSizes, productBatchSizes, batchQuantities, true);

            Assert.Equal(expectedOrders.Count, result.Count);

            for (var i = 0; i < expectedOrders.Count; i++)
            {
                Assert.Equal(expectedOrders[i].ProductCode, result[i].ProductCode);
                Assert.Equal(expectedOrders[i].ProductName, result[i].ProductName);
                Assert.Equal(expectedOrders[i].BatchSizeCode, result[i].BatchSizeCode);
                Assert.Equal(expectedOrders[i].BatchSize, result[i].BatchSize);
                Assert.Equal(expectedOrders[i].BatchQuantity, result[i].BatchQuantity);
                Assert.Equal(expectedOrders[i].Price, result[i].Price);
            };
        }

        [Fact]
        public void ProduceOrderWithMinBatchShouldReturnCorrectData()
        {
            var expectedOrders = new List<Order>
            {
                new Order(new Product("P1", "Milk", 1.99m), new BatchSize("BS6", 20), 20),
                new Order(new Product("P2", "Soure Milk", 2.05m), new BatchSize("BS1", 20), 500),
                new Order(new Product("P3", "Cream", 3.59m), new BatchSize("BS4", 50), 40),
                new Order(new Product("P4", "Yoghurt", 4.99m), new BatchSize("BS_GENERATED_P4", 1), 234),
                new Order(new Product("P5", "Buttermilk", 3.1m), new BatchSize("BS7", 50), 1),
            };

            var orderService = new OrderService();
            var result = orderService.ProduceOrders(products, batchSizes, productBatchSizes, batchQuantities, false);

            Assert.Equal(expectedOrders.Count, result.Count);

            for (var i = 0; i < expectedOrders.Count; i++)
            {
                Assert.Equal(expectedOrders[i].ProductCode, result[i].ProductCode);
                Assert.Equal(expectedOrders[i].ProductName, result[i].ProductName);
                Assert.Equal(expectedOrders[i].BatchSizeCode, result[i].BatchSizeCode);
                Assert.Equal(expectedOrders[i].BatchSize, result[i].BatchSize);
                Assert.Equal(expectedOrders[i].BatchQuantity, result[i].BatchQuantity);
                Assert.Equal(expectedOrders[i].Price, result[i].Price);
            };
        }

        [Theory()]
        [InlineData(0, "P123", 45)]
        [InlineData(-10, "P123", 45)]
        [InlineData(10, "", 45)]
        [InlineData(10, "P123", -70)]
        public void ProduceOrderWithIncorrectDataShouldThrowException(int quantity, string productCode, int batchSize)
        {
            var batchQuantities = new BatchQuantity[]
            {
                new BatchQuantity(productCode, quantity)
            };

            var products = new Product[] { new Product(productCode, "name", 5.33m) };

            var batchSizes = new BatchSize[] { new BatchSize("BS123", batchSize) };

            var productBatchSizes = new ProductBatchSize[] { new ProductBatchSize(productCode, "BS123") };

            var orderService = new OrderService();
            Assert.Throws<ArgumentException>(() => 
            orderService.ProduceOrders(products, batchSizes, productBatchSizes, batchQuantities, false));
        }
    }
}
