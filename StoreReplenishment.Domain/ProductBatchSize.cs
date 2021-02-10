namespace StoreReplenishment.Domain
{
    public record ProductBatchSize
    {
        public string ProductCode { get; set; }

        public string BatchSizeCode { get; set; }

        public ProductBatchSize(string productCode, string batchSizeCode)
        {
            ProductCode = productCode;
            BatchSizeCode = batchSizeCode;
        }
    }
}
