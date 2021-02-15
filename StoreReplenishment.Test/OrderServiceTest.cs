using StoreReplanishment.Application;
using StoreReplenishment.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
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

        [Fact]
        public void ProduceOrderWithAmount_ShouldReturnCombination()
        {
            var orderService = new OrderService();

            var product = new Product("P1", "Name", 1.22m);
            var batchSizes = new BatchSize[] { new BatchSize("BS1", 20), new BatchSize("BS2", 30), new BatchSize("BS3", 40) };
            var productBatchSizes = new ProductBatchSize[] { 
                new ProductBatchSize("P1", "BS1"), 
                new ProductBatchSize("P1", "BS2"), 
                new ProductBatchSize("P1", "BS3") 
            };

            var result = orderService.ProduceOrders(product, batchSizes, productBatchSizes, 70).ToList();

            Assert.Equal(2, result.Count);
            Assert.NotNull(result.SingleOrDefault(r => r.BatchSize == 30));
            Assert.NotNull(result.SingleOrDefault(r => r.BatchSize == 40));
        }

        [Fact]
        public void ProduceOrderWithAmount_ShouldReturnDesiredBatchSize()
        {
            var orderService = new OrderService();

            var product = new Product("P1", "Name", 1.22m);
            var batchSizes = new BatchSize[] { new BatchSize("BS1", 70), new BatchSize("BS2", 30), new BatchSize("BS3", 40) };
            var productBatchSizes = new ProductBatchSize[] {
                new ProductBatchSize("P1", "BS1"),
                new ProductBatchSize("P1", "BS2"),
                new ProductBatchSize("P1", "BS3")
            };

            var result = orderService.ProduceOrders(product, batchSizes, productBatchSizes, 70).ToList();

            Assert.Single(result);
            Assert.NotNull(result.SingleOrDefault(r => r.BatchSize == 70));
        }

        [Fact]
        public void ProduceOrderWithAmount_ShouldReturnOneOfCombination()
        {
            var orderService = new OrderService();

            var product = new Product("P1", "Name", 1.22m);
            var batchSizes = new BatchSize[] { 
                new BatchSize("BS1", 20), 
                new BatchSize("BS2", 30), 
                new BatchSize("BS4", 50), 
                new BatchSize("BS3", 40) };
            var productBatchSizes = new ProductBatchSize[] {
                new ProductBatchSize("P1", "BS1"),
                new ProductBatchSize("P1", "BS2"),
                new ProductBatchSize("P1", "BS3"),
                new ProductBatchSize("P1", "BS4")
            };

            var result = orderService.ProduceOrders(product, batchSizes, productBatchSizes, 70).ToList();

            Assert.Equal(2, result.Count);
            Assert.NotNull(result.SingleOrDefault(r => r.BatchSize == 50));
            Assert.NotNull(result.SingleOrDefault(r => r.BatchSize == 20));
        }

        [Fact]
        public void ProduceOrderWithAmount_ShouldReturnClosestBatchSize()
        {
            var orderService = new OrderService();

            var product = new Product("P1", "Name", 1.22m);
            var batchSizes = new BatchSize[] {
                new BatchSize("BS1", 120),
                new BatchSize("BS2", 30),
                new BatchSize("BS3", 50),
                new BatchSize("BS4", 60),
                new BatchSize("BS5", 80),
                new BatchSize("BS6", 100) };
            var productBatchSizes = new ProductBatchSize[] {
                new ProductBatchSize("P1", "BS1"),
                new ProductBatchSize("P1", "BS2"),
                new ProductBatchSize("P1", "BS3"),
                new ProductBatchSize("P1", "BS4"),
                new ProductBatchSize("P1", "BS5"),
                new ProductBatchSize("P1", "BS6")
            };

            var result = orderService.ProduceOrders(product, batchSizes, productBatchSizes, 70).ToList();

            Assert.Single(result);
            Assert.NotNull(result.SingleOrDefault(r => r.BatchSize == 80));
        }

        [Fact]
        public void ProduceOrderWithAmount_ShouldReturnClosestBatchSizeFromBiggerBatches()
        {
            var orderService = new OrderService();

            var product = new Product("P1", "Name", 1.22m);
            var batchSizes = new BatchSize[] {
                new BatchSize("BS1", 120),
                new BatchSize("BS2", 300),
                new BatchSize("BS3", 500),
                new BatchSize("BS4", 600),
                new BatchSize("BS5", 80),
                new BatchSize("BS6", 100) };
            var productBatchSizes = new ProductBatchSize[] {
                new ProductBatchSize("P1", "BS1"),
                new ProductBatchSize("P1", "BS2"),
                new ProductBatchSize("P1", "BS3"),
                new ProductBatchSize("P1", "BS4"),
                new ProductBatchSize("P1", "BS5"),
                new ProductBatchSize("P1", "BS6")
            };

            var result = orderService.ProduceOrders(product, batchSizes, productBatchSizes, 70).ToList();

            Assert.Single(result);
            Assert.NotNull(result.SingleOrDefault(r => r.BatchSize == 80));
        }

        [Fact]
        public void ProduceOrderWithAmount_ShouldReturnCombinationFromThreeBatches()
        {
            var orderService = new OrderService();

            var product = new Product("P1", "Name", 1.22m);
            var batchSizes = new BatchSize[] {
                new BatchSize("BS1", 120),
                new BatchSize("BS2", 30),
                new BatchSize("BS3", 50),
                new BatchSize("BS4", 15),
                new BatchSize("BS5", 25),
                new BatchSize("BS6", 100) };
            var productBatchSizes = new ProductBatchSize[] {
                new ProductBatchSize("P1", "BS1"),
                new ProductBatchSize("P1", "BS2"),
                new ProductBatchSize("P1", "BS3"),
                new ProductBatchSize("P1", "BS4"),
                new ProductBatchSize("P1", "BS5"),
                new ProductBatchSize("P1", "BS6")
            };

            var result = orderService.ProduceOrders(product, batchSizes, productBatchSizes, 70).ToList();

            Assert.Equal(3, result.Count);
            Assert.NotNull(result.SingleOrDefault(r => r.BatchSize == 30));
            Assert.NotNull(result.SingleOrDefault(r => r.BatchSize == 15));
            Assert.NotNull(result.SingleOrDefault(r => r.BatchSize == 25));
        }

        [Fact]
        public void ProduceOrderWithAmount_ShouldReturnBatchWithSeveralQuantity()
        {
            var orderService = new OrderService();

            var product = new Product("P1", "Name", 1.22m);
            var batchSizes = new BatchSize[] {
                new BatchSize("BS1", 10),
                new BatchSize("BS2", 50),
                new BatchSize("BS3", 100) 
            };
            var productBatchSizes = new ProductBatchSize[] {
                new ProductBatchSize("P1", "BS1"),
                new ProductBatchSize("P1", "BS2"),
                new ProductBatchSize("P1", "BS3")
            };

            var result = orderService.ProduceOrders(product, batchSizes, productBatchSizes, 70).ToList();

            Assert.Single(result);
            Assert.NotNull(result.SingleOrDefault(r => r.BatchSize == 10));
            Assert.Equal(7, result.SingleOrDefault(r => r.BatchSize == 10)?.BatchQuantity);
        }

        [Fact]
        public void ProduceOrderWithAmount_ShouldReturnClosestCombination()
        {
            var orderService = new OrderService();

            var product = new Product("P1", "Name", 1.22m);
            var batchSizes = new BatchSize[] {
                new BatchSize("BS1", 20),
                new BatchSize("BS2", 100),
                new BatchSize("BS4", 55),
                new BatchSize("BS3", 110) };
            var productBatchSizes = new ProductBatchSize[] {
                new ProductBatchSize("P1", "BS1"),
                new ProductBatchSize("P1", "BS2"),
                new ProductBatchSize("P1", "BS3"),
                new ProductBatchSize("P1", "BS4")
            };

            var result = orderService.ProduceOrders(product, batchSizes, productBatchSizes, 70).ToList();

            Assert.Equal(2, result.Count);
            Assert.NotNull(result.SingleOrDefault(r => r.BatchSize == 55));
            Assert.NotNull(result.SingleOrDefault(r => r.BatchSize == 20));
        }

        [Fact]
        public void ProduceOrderWithAmount_ShouldReturnCombinationWithSeveralQuantity()
        {
            var orderService = new OrderService();

            var product = new Product("P1", "Name", 1.22m);
            var batchSizes = new BatchSize[] {
                new BatchSize("BS1", 60),
                new BatchSize("BS2", 100),
                new BatchSize("BS3", 120),
                new BatchSize("BS4", 30),
                new BatchSize("BS5", 20) };
            var productBatchSizes = new ProductBatchSize[] {
                new ProductBatchSize("P1", "BS1"),
                new ProductBatchSize("P1", "BS2"),
                new ProductBatchSize("P1", "BS3"),
                new ProductBatchSize("P1", "BS4"),
                new ProductBatchSize("P1", "BS5")
            };

            var result = orderService.ProduceOrders(product, batchSizes, productBatchSizes, 70).ToList();

            Assert.Equal(2, result.Count);
            Assert.NotNull(result.SingleOrDefault(r => r.BatchSize == 30));
            Assert.NotNull(result.SingleOrDefault(r => r.BatchSize == 20));
            Assert.Equal(2, result.SingleOrDefault(r => r.BatchSize == 20).BatchQuantity);
        }
    }
}
