namespace webapi.DTOs
{
    public struct Product
    {
        public int Id { get; }
        public string? ProductName { get; }
        public int Active { get; }

        public Product(int id, string? name, int active)
        {
            Id = id;
            ProductName = name;
            Active = active;
        }
    }
}

