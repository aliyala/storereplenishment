namespace StoreReplenishment.Domain
{
    public record Product
    {
        public string Code { get; init; }

        public string Name { get; init; }

        public decimal Price { get; init; }

        public Product(string code, string name, decimal price)
        {
            Code = code;
            Name = name;
            Price = price;
        }
    }
}
