namespace StoreReplenishment.Domain
{
    public record BatchQuantity
    {
        public string ProductCode { get; init; }

        public int Quantity { get; init; }

        public BatchQuantity(string productCode, int quantity)
        {
            ProductCode = productCode;
            Quantity = quantity;
        }
    }
}
