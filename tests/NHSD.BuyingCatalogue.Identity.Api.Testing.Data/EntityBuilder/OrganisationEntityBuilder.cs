using System;
using NHSD.BuyingCatalogue.Identity.Api.Testing.Data.Entities;
using NHSD.BuyingCatalogue.Identity.Api.Testing.Data.Models;

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
                PrimaryRoleId = "ID 0",
                CatalogueAgreementSigned = false,
                LocationObject = new Location(),
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

        public OrganisationEntityBuilder WithPrimaryRoleId(string primaryRoleId)
        {
            _organisationEntity.PrimaryRoleId = primaryRoleId;
            return this;
        }

        public OrganisationEntityBuilder WithCatalogueAgreementSigned(bool isSigned)
        {
            _organisationEntity.CatalogueAgreementSigned = isSigned;
            return this;
        }

        public OrganisationEntityBuilder WithLocationLine1(string line1)
        {
            _organisationEntity.LocationObject.Line1 = line1;
            return this;
        }

        public OrganisationEntityBuilder WithLocationLine2(string line2)
        {
            _organisationEntity.LocationObject.Line2 = line2;
            return this;
        }

        public OrganisationEntityBuilder WithLocationLine3(string line3)
        {
            _organisationEntity.LocationObject.Line3 = line3;
            return this;
        }

        public OrganisationEntityBuilder WithLocationLine4(string line4)
        {
            _organisationEntity.LocationObject.Line4 = line4;
            return this;
        }

        public OrganisationEntityBuilder WithLocationTown(string town)
        {
            _organisationEntity.LocationObject.Town = town;
            return this;
        }

        public OrganisationEntityBuilder WithLocationCounty(string county)
        {
            _organisationEntity.LocationObject.County = county;
            return this;
        }

        public OrganisationEntityBuilder WithLocationPostcode(string postcode)
        {
            _organisationEntity.LocationObject.Postcode = postcode;
            return this;
        }

        public OrganisationEntityBuilder WithLocationCountry(string country)
        {
            _organisationEntity.LocationObject.Country = country;
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
