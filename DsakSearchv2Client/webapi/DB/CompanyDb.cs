namespace webapi.DB
{
    public record CompanyDb
    {
        public int Id { get; }
        public string? CompanyName { get; }
        public string? CompanyType { get; }
        public string? Manager { get; }
    }
}
