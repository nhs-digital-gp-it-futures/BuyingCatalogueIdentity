using System;

namespace NHSD.BuyingCatalogue.Organisations.Api.Models
{
    public sealed class Error
    {
        public string Id { get; }

        public string Field { get; }

        public Error(string id) : this(id, null)
        {
        }

        public Error(string id, string field)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            Field = field;
        }

        private bool Equals(Error other)
        {
            return string.Equals(Id, other.Id, StringComparison.Ordinal)
                && string.Equals(Field, other.Field, StringComparison.Ordinal);
        }

        public override bool Equals(object obj)
        {
            return ReferenceEquals(this, obj) || obj is Error other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Field);
        }
    }
}
