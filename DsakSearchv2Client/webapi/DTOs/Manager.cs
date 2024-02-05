using System.Diagnostics.CodeAnalysis;

namespace webapi.DTOs
{
    public readonly struct Manager
    {
        public int Id { get; }
        public string? ManagerName { get; }
        public Manager(int id, string? manager)
        {
            Id = id;
            ManagerName = manager;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            return Id == ((Manager)obj).Id;
        }
    }
}
