using System;
using NHSD.BuyingCatalogue.Identity.Api.Testing.Data.Entities;

namespace NHSD.BuyingCatalogue.Identity.Api.Testing.Data.EntityBuilder
{
    public sealed class OrganisationEntityBuilder
    {
        private readonly OrganisationEntity _organisationEntity;

        public static OrganisationEntityBuilder Create()
        {
            return new OrganisationEntityBuilder();
        }

        public OrganisationEntityBuilder()
        {
            _organisationEntity = new OrganisationEntity()
            {
                Id = Guid.NewGuid(),
                Name = "Organisation Name",
                OdsCode = "Ods Code",
                LastUpdated = DateTime.Now
            };
        }

        public OrganisationEntityBuilder WithId(Guid id)
        {
            _organisationEntity.Id = id;
            return this;
        }

        public OrganisationEntityBuilder WithName(string name)
        {
            _organisationEntity.Name = name;
            return this;
        }

        public OrganisationEntityBuilder WithOdsCode(string odsCode)
        {
            _organisationEntity.OdsCode = odsCode;
            return this;
        }

        public OrganisationEntityBuilder WithLastUpdated(DateTime lastUpdated)
        {
            _organisationEntity.LastUpdated = lastUpdated;
            return this;
        }

        public OrganisationEntity Build()
        {
            return _organisationEntity;
        }
    }
}
