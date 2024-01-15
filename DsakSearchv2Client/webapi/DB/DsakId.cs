namespace webapi.DB
{
    public struct DsakId : IEquatable<DsakId>
    {
        public DsakId(int id)
        {
            Id = id;
        }

        public int Id { get; }

        public bool Equals(DsakId other) => Id == other.Id;
        public override bool Equals(object obj) => obj is DsakId && Equals((DsakId)obj);
        public override int GetHashCode() => Id.GetHashCode();
        public override string ToString() => Id.ToString();
    }
}
