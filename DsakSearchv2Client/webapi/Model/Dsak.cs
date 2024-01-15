namespace webapi.Model
{
    public class Dsak
    {
        public int Id { get; }
        public string? Title { get; }
        public DateTime Created_At { get; }
        public int? Company_Id { get; }
        public string? Company { get; }
        public int? Manager_Id { get; }
        public string? Manager { get; }
        public string? Pbi { get; }
        public int? Product_Id { get; }
        public string? Product { get; }
        public int? Version_Id { get; }
        public int Status { get; }
        public int? Front { get; }
    }
}
