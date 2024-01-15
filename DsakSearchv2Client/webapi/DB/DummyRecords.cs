namespace webapi.DB
{
    public record DummyRecord
    {
        public int CategoryId { get; init; }
        public string? Name { get; init; }
        public int Rank { get; init; }
        public string? Tooltip { get; init; }
        public int Deleted { get; init; }
        public DateTime? Registered { get; init; }
        public DateTime? Updated { get; init; }
    }
}
