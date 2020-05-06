using System;

namespace NHSD.BuyingCatalogue.Identity.Common.Models
{
    public sealed class ErrorDetails : IEquatable<ErrorDetails>
    {
        public string Id { get; }

        public string? Field { get; }

        public ErrorDetails(string id, string? field = null)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            Field = field;
        }

        public bool Equals(ErrorDetails other)
        {
            if (other is null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return string.Equals(Id, other.Id, StringComparison.Ordinal)
                && string.Equals(Field, other.Field, StringComparison.Ordinal);
        }


        public override bool Equals(object? obj)
        {
#pragma warning disable CS8604 // Possible null reference argument.
            return Equals(obj as ErrorDetails);
#pragma warning restore CS8604 // Possible null reference argument.
        }

        public override int GetHashCode() => HashCode.Combine(Id, Field);
        
    }
}
