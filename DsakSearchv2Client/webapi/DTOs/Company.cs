namespace webapi.DTOs
{
    public struct Company
    {
        public int Id { get; }
        public string? CompanyName { get; }

        public Company(int id, string? name)
        {
            Id = id;
            CompanyName = name;
        }
    }
}