namespace StoreReplenishment.Domain
{
    public class BatchSize
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
