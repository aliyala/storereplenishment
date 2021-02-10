using System;

namespace StoreReplenishment.Domain
{
    public record Order
    {
        public string ProductCode { get; private set; }

        public string BatchSizeCode { get; private set; }

        public string ProductName { get; private set; }

        public int BatchSize { get; private set; }

        public int BatchQuantity { get; private set;  }

        public decimal Price { get; private set; }

        public Order(Product product, BatchSize batch, int quantity)
        {
            if (quantity <= 0) throw new ArgumentException("Quantity should be more than 0", nameof(quantity));
            if (product is null) throw new ArgumentNullException(nameof(product));
            if (batch is null) throw new ArgumentNullException(nameof(batch));

            if (batch.Size <= 0 || string.IsNullOrEmpty(batch.Code)) 
                throw new ArgumentException("Batch size should be more than 0 and code should have value", nameof(batch));
            if (product.Price < 0 || string.IsNullOrEmpty(product.Code) || string.IsNullOrEmpty(product.Name)) 
                throw new ArgumentException("Product price should be more than 0 and code should have value", nameof(product));

            BatchQuantity = quantity;
            BatchSize = batch.Size;
            BatchSizeCode = batch.Code;

            ProductCode = product.Code;
            ProductName = product.Name;
            Price = product.Price;
        }
    }
}
