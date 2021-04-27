using System;
using NHSD.BuyingCatalogue.Identity.Api.Testing.Data.Entities;
using NHSD.BuyingCatalogue.Identity.Api.Testing.Data.Models;

namespace NHSD.BuyingCatalogue.Identity.Api.Testing.Data.EntityBuilder
{
    public sealed class OrganisationEntityBuilder
    {
        private readonly OrganisationEntity organisationEntity;

        public OrganisationEntityBuilder()
        {
            organisationEntity = new OrganisationEntity
            {
                OrganisationId = Guid.NewGuid(),
                Name = "Organisation Name",
                OdsCode = "Ods Code",
                PrimaryRoleId = "ID 0",
                CatalogueAgreementSigned = false,
                AddressObject = new Address(),
                LastUpdated = DateTime.Now,
            };
        }

        public static OrganisationEntityBuilder Create()
        {
            return new();
        }

        public OrganisationEntityBuilder WithId(Guid id)
        {
            organisationEntity.OrganisationId = id;
            return this;
        }

        public OrganisationEntityBuilder WithName(string name)
        {
            organisationEntity.Name = name;
            return this;
        }

        public OrganisationEntityBuilder WithOdsCode(string odsCode)
        {
            organisationEntity.OdsCode = odsCode;
            return this;
        }

        public OrganisationEntityBuilder WithPrimaryRoleId(string primaryRoleId)
        {
            organisationEntity.PrimaryRoleId = primaryRoleId;
            return this;
        }

        public OrganisationEntityBuilder WithCatalogueAgreementSigned(bool isSigned)
        {
            organisationEntity.CatalogueAgreementSigned = isSigned;
            return this;
        }

        public OrganisationEntityBuilder WithAddressLine1(string line1)
        {
            organisationEntity.AddressObject.Line1 = line1;
            return this;
        }

        public OrganisationEntityBuilder WithAddressLine2(string line2)
        {
            organisationEntity.AddressObject.Line2 = line2;
            return this;
        }

        public OrganisationEntityBuilder WithAddressLine3(string line3)
        {
            organisationEntity.AddressObject.Line3 = line3;
            return this;
        }
        public OrganisationEntityBuilder WithAddressLine4(string line4)
        {
            organisationEntity.AddressObject.Line4 = line4;
            return this;
        }

        public OrganisationEntityBuilder WithAddressTown(string town)
        {
            organisationEntity.AddressObject.Town = town;
            return this;
        }

        public OrganisationEntityBuilder WithAddressCounty(string county)
        {
            organisationEntity.AddressObject.County = county;
            return this;
        }

        public OrganisationEntityBuilder WithAddressPostcode(string postcode)
        {
            organisationEntity.AddressObject.Postcode = postcode;
            return this;
        }

        public OrganisationEntityBuilder WithAddressCountry(string country)
        {
            organisationEntity.AddressObject.Country = country;
            return this;
        }

        public OrganisationEntity Build()
        {
            return organisationEntity;
        }
    }
}
