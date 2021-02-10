namespace StoreReplenishment.Domain
{
    public record BatchSize
    {
        public string Code { get; init; }

        public int Size { get; init; }

        public BatchSize(string code, int size)
        {
            Code = code;
            Size = size;
        }
    }
}
