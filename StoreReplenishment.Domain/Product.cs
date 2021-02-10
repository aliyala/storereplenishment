namespace StoreReplenishment.Domain
{
    public record Product
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public decimal Price { get; set; }

        public Product(string code, string name, decimal price)
        {
            Code = code;
            Name = name;
            Price = price;
        }
    }
}
