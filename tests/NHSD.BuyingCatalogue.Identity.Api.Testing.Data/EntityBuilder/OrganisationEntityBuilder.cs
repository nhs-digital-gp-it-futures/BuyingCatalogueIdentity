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
                AddressObject = new Address(),
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

        public OrganisationEntityBuilder WithAddressLine1(string line1)
        {
            _organisationEntity.AddressObject.Line1 = line1;
            return this;
        }

        public OrganisationEntityBuilder WithAddressLine2(string line2)
        {
            _organisationEntity.AddressObject.Line2 = line2;
            return this;
        }

        public OrganisationEntityBuilder WithAddressLine3(string line3)
        {
            _organisationEntity.AddressObject.Line3 = line3;
            return this;
        }

        public OrganisationEntityBuilder WithAddressLine4(string line4)
        {
            _organisationEntity.AddressObject.Line4 = line4;
            return this;
        }

        public OrganisationEntityBuilder WithAddressTown(string town)
        {
            _organisationEntity.AddressObject.Town = town;
            return this;
        }

        public OrganisationEntityBuilder WithAddressCounty(string county)
        {
            _organisationEntity.AddressObject.County = county;
            return this;
        }

        public OrganisationEntityBuilder WithAddressPostcode(string postcode)
        {
            _organisationEntity.AddressObject.Postcode = postcode;
            return this;
        }

        public OrganisationEntityBuilder WithAddressCountry(string country)
        {
            _organisationEntity.AddressObject.Country = country;
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
