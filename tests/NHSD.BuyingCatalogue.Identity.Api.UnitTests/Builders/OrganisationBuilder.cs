using System;
using NHSD.BuyingCatalogue.Identity.Api.Models;

namespace NHSD.BuyingCatalogue.Identity.Api.UnitTests.Builders
{
    internal sealed class OrganisationBuilder
    {
        private readonly Guid organisationId;
        private readonly string name;

        private OrganisationBuilder()
        {
            organisationId = Guid.NewGuid();
            name = "Primary HealthTrust";
        }

        public Organisation Build()
        {
            return new() { Name = name, OrganisationId = organisationId };
        }

        internal static OrganisationBuilder Create() => new();
    }
}
