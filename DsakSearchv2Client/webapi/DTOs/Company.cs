namespace webapi.DTOs
{
    public readonly struct Company
    {
        public int Id { get; }
        public string? CompanyName { get; }
        public string? CompanyType { get; }
        public string? Manager { get; }
        public Company(int id, string? name, string? type, string? manager)
        {
            Id = id;
            CompanyName = name;
            CompanyType = type;
            Manager = manager;
        }
    }
}
