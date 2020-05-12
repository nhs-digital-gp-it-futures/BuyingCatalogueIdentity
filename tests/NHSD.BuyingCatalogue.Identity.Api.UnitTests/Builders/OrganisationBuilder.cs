using System;
using System.Collections.Generic;
using System.Text;
using NHSD.BuyingCatalogue.Identity.Api.Models;

namespace NHSD.BuyingCatalogue.Identity.Api.UnitTests.Builders
{
    internal sealed class OrganisationBuilder
    {
        internal static OrganisationBuilder Create() => new OrganisationBuilder();

        private Guid _organisationId;
        private string _name;

        private OrganisationBuilder()
        {
            _organisationId =  Guid.NewGuid();
            _name = "Primary HealthTrust";
        }

        public OrganisationBuilder WithName(string name)
        {
            _name = name;
            return this;
        }

        public OrganisationBuilder WithOrganisationId(Guid organisationId)
        {
            _organisationId = organisationId;
            return this;
        }

        public Organisation Build()
        {
            return new Organisation {Name = _name, OrganisationId = _organisationId};
        }
    }
}
