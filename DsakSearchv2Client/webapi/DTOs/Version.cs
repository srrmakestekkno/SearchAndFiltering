namespace webapi.DTOs
{
    public readonly struct Version
    {
        public int? Id { get; }
        public string? Name { get; }

        public Version(int? id, string? name)
        {
            Id = id;
            Name = name;
        }
    }
}
