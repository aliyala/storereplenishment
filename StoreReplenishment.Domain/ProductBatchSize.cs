namespace StoreReplenishment.Domain
{
    public record ProductBatchSize
    {
        public string ProductCode { get; init; }

        public string BatchSizeCode { get; init; }

        public ProductBatchSize(string productCode, string batchSizeCode)
        {
            ProductCode = productCode;
            BatchSizeCode = batchSizeCode;
        }
    }
}
