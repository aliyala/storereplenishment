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

        
        public IList<Order> ProduceOrders(Product product, BatchSize[] batchSizes, ProductBatchSize[] productBatchSizes, int amount)
        {
            var availableProductBatchSizes = productBatchSizes.Where(pbs => pbs.ProductCode == product.Code)
                .Select(pbs => pbs.BatchSizeCode).ToList();
            var availableBatchSizes = batchSizes.Where(bs => availableProductBatchSizes.Contains(bs.Code))
                .OrderByDescending(bs => bs.Size).ToList();

            if (!availableBatchSizes.Any())
            {
                return new List<Order>();
            }

            var potentialOrdersWithDesiredAmount = new List<Order>();
            var potentialOrdersWithClosestAmount = new List<Order>();

            var bsArr = availableBatchSizes.ToArray();
            var allCombinations = new List<BatchSize[]>();
            for (var i = 0; i < bsArr.Length; i++)
            {
                var batchSize = bsArr[i];
                if (batchSize.Size == amount)
                {
                    var order = new List<Order> { new Order(product, batchSize, 1) };

                    return order;
                }
                if (batchSize.Size > amount)
                {
                    var order = new List<Order> { new Order(product, batchSize, 1) };
                    if (IsCloserOrder(potentialOrdersWithClosestAmount, order))
                    {
                        potentialOrdersWithClosestAmount = order;
                    }
                }
                else
                {
                    if (!allCombinations.Any())
                    {
                        var lessBatchSizes = bsArr.Skip(i).ToArray();
                        allCombinations = ProcessCombinations(lessBatchSizes, amount, product, ref potentialOrdersWithDesiredAmount,
                            ref potentialOrdersWithClosestAmount);
                    }

                    ProcessBatchSizesWithSeveralQuantity(batchSize, amount, allCombinations, product,
                        ref potentialOrdersWithDesiredAmount, ref potentialOrdersWithClosestAmount);
                }

            }          

            return potentialOrdersWithDesiredAmount.Any() ? potentialOrdersWithDesiredAmount :
                potentialOrdersWithClosestAmount.Any() ? potentialOrdersWithClosestAmount : new List<Order>();
        }

        private List<BatchSize[]> ProcessCombinations(BatchSize[] batchSizes, int amount, Product product, 
            ref List<Order> potentialOrdersWithDesiredAmount, ref List<Order> potentialOrdersWithClosestAmount)
        {
            var allCombinations = new List<BatchSize[]>();
            var closestBatchSizes = Array.Empty<BatchSize>();
            for (var combLength = 2; combLength <= batchSizes.Length; combLength++)
            {
                Combinations(batchSizes, combLength, 0, new BatchSize[combLength], amount, out var desiredBatchSizes, 
                    ref closestBatchSizes, ref allCombinations);
                if (desiredBatchSizes != null)
                {
                    FindPotentialOrders(product, desiredBatchSizes, ref potentialOrdersWithDesiredAmount, IsBetterOrder);
                }
            }

            if (closestBatchSizes != null && closestBatchSizes.Any())
            {
                FindPotentialOrders(product, closestBatchSizes, ref potentialOrdersWithClosestAmount, IsCloserOrder);
            }

            return allCombinations;
        }

        private static void FindPotentialOrders(Product product, BatchSize[] batchSizes, ref List<Order> potentialOrders,
            Func<List<Order>, List<Order>, bool> filter)
        {
            var orderList = batchSizes.Select(bs => new Order(product, bs, 1)).ToList();
            
            if (filter(potentialOrders, orderList))
            {
                potentialOrders = orderList;
            }
        }

        private void ProcessBatchSizesWithSeveralQuantity(BatchSize batchSize, int amount, List<BatchSize[]> allCombinations, 
            Product product, ref List<Order> potentialOrdersWithDesiredAmount, ref List<Order> potentialOrdersWithClosestAmount)
        {
            // more quantity
            var sizeWithSeveralQuantity = batchSize.Size;
            var quantity = 1;
            while (sizeWithSeveralQuantity < amount)
            {
                quantity++;
                sizeWithSeveralQuantity += batchSize.Size;

                if (sizeWithSeveralQuantity < amount)
                {
                    // check with combinations
                    CheckBatchSizeWithCombination(batchSize, allCombinations, amount, out var desiredBatchSizes);
                    if (desiredBatchSizes != null)
                    {
                        var desiredOrder = new List<Order>();
                        foreach (var batchGroup in desiredBatchSizes.GroupBy(b => b))
                        {
                            desiredOrder.Add(new Order(product, batchGroup.Key, batchGroup.Count()));
                        }
                        if (IsBetterOrder(potentialOrdersWithDesiredAmount, desiredOrder))
                        {
                            potentialOrdersWithDesiredAmount = desiredOrder;
                        }
                    }
                }
            }
            if (sizeWithSeveralQuantity == amount)
            {
                var desiredOrder = new List<Order> { new Order(product, batchSize, quantity) };
                if (IsBetterOrder(potentialOrdersWithDesiredAmount, desiredOrder))
                {
                    potentialOrdersWithDesiredAmount = desiredOrder;
                }
            }
            var order = new List<Order> { new Order(product, batchSize, quantity) };
            if (IsCloserOrder(potentialOrdersWithClosestAmount, order))
            {
                potentialOrdersWithClosestAmount = order;
            }
        }

        private void Combinations(BatchSize[] arr, int len, int startPosition, BatchSize[] result, int amount, 
            out BatchSize[]? desiredBatchSizes, ref BatchSize[]? closestBatchSizes, ref List<BatchSize[]> allCombinations)
        {
            if (len == 0)
            {
                desiredBatchSizes = result.Sum(s => s.Size) == amount ? result : null;
                
                var currentSum = result.Sum(r => r.Size);
                if (currentSum > amount &&
                    (closestBatchSizes == null || !closestBatchSizes.Any() || currentSum < closestBatchSizes.Sum(c => c.Size)))
                {
                    closestBatchSizes = result;
                }
                allCombinations.Add(result);
                return;
            }
            for (var i = startPosition; i <= arr.Length - len; i++)
            {
                result[^len] = arr[i];
                Combinations(arr, len - 1, i + 1, result, amount, out desiredBatchSizes, ref closestBatchSizes, ref allCombinations);
                if (desiredBatchSizes != null)
                {
                    return;
                }
            }
            desiredBatchSizes = null;
        }

        private void CheckBatchSizeWithCombination(BatchSize batchSize, List<BatchSize[]> allCombinations, int amount,
            out BatchSize[]? desiredBatchSizes)
        {
            foreach (var comb in allCombinations)
            {
                var newComb = comb.Append(batchSize);
                if (newComb.Sum(n => n.Size) == amount)
                {
                    desiredBatchSizes = newComb.ToArray();
                    return;
                }
            }
            desiredBatchSizes = null;
        }

        private static bool IsBetterOrder(List<Order> prev, List<Order> next) =>        
             prev is null || !prev.Any() || 
             next.Count < prev.Count ||
             next.Count == prev.Count && next.Sum(o => o.BatchQuantity) < prev.Sum(o => o.BatchQuantity);
        

        private static bool IsCloserOrder(List<Order> prev, List<Order> next) =>        
            prev is null || !prev.Any() ||
            next.Sum(o => o.BatchSize * o.BatchQuantity) < prev.Sum(o => o.BatchSize * o.BatchQuantity) ||
            next.Sum(o => o.BatchSize * o.BatchQuantity) == prev.Sum(o => o.BatchSize * o.BatchQuantity) && IsBetterOrder(prev, next);
    }
}
