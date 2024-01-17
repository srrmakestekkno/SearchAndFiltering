namespace webapi.DTOs
{
    public readonly struct DipsVersion
    {
        public int? Id { get; }
        public string? Name { get; }

        public DipsVersion(int? id, string? name)
        {
            Id = id;
            Name = name;
        }
    }
}
