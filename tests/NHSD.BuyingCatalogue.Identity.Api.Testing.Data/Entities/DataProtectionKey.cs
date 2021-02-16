using System;
using System.Xml.Linq;

namespace NHSD.BuyingCatalogue.Identity.Api.Testing.Data.Entities
{
    public sealed class DataProtectionKey : IEquatable<DataProtectionKey>
    {
        public DataProtectionKey(string xml)
            : this(XElement.Parse(xml))
        {
        }

        public DataProtectionKey(XElement key)
        {
            Element = key ?? throw new ArgumentNullException(nameof(key));
            Id = key.Attribute("id")?.Value;
            FriendlyName = $"key-{Id}";
            Xml = key.ToString(SaveOptions.DisableFormatting);
        }

        public XElement Element { get; }

        public string FriendlyName { get; }

        public string Id { get; }

        public string Xml { get; }

        public bool Equals(DataProtectionKey other)
        {
            if (other is null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as DataProtectionKey);
        }

        public override int GetHashCode() => HashCode.Combine(Id);
    }
}
