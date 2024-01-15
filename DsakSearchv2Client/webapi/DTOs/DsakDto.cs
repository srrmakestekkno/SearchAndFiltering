namespace webapi.DTOs
{
    public class DsakDto
    {
        public int CategoryId { get; set; }
        public string? Name { get; set; }
        public int Rank { get; set; }
        public string? Tooltip { get; set; }
        public bool Deleted { get; set; }
        public DateTime? Registered { get; set; }
        public DateTime? Updated { get; set; }
    }
}
