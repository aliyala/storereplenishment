namespace StoreReplenishment.Domain
{
    public class Order
    {
        public string ProductCode { get; set; }

        public string BatchSizeCode { get; set; }

        public string ProductName { get; set; }

        public int BatchSize { get; set; }

        public int BatchQuantity { get; set; }

        public decimal Price { get; set; }

        public Order() { }

        public Order(string productCode, string batchSizeCode, string productName, int batchSize, int batchQuantity, decimal price)
        {
            ProductCode = productCode;
            BatchSizeCode = batchSizeCode;
            ProductName = productName;
            BatchSize = batchSize;
            BatchQuantity = batchQuantity;
            Price = price;
        }
    }
}
