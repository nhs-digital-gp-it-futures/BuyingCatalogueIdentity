using System;

namespace NHSD.BuyingCatalogue.Identity.Api.Models
{
    public sealed class Organisation
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string OdsCode { get; set; }

        public DateTime LastUpdated { get; set; }

        public Organisation(Guid id, string name, string odsCode)
        {
            Id = id;
            Name = name;
            OdsCode = odsCode;
        }
    }
}
