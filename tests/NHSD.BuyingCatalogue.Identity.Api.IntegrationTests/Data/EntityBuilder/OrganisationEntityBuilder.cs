using System;
using Microsoft.EntityFrameworkCore;
using NHSD.BuyingCatalogue.Identity.Api.Data;
using NHSD.BuyingCatalogue.Identity.Api.IntegrationTests.Data.Entities;
using NHSD.BuyingCatalogue.Identity.Api.Models;

namespace NHSD.BuyingCatalogue.Identity.Api.IntegrationTests.Data.EntityBuilder
{
    public sealed class OrganisationEntityBuilder
    {
        private readonly ApplicationDbContext _context;
        private readonly OrganisationEntity _organisationEntity;
        private readonly DbContextOptions<ApplicationDbContext> _options;

        public static OrganisationEntityBuilder Create()
        {
            return new OrganisationEntityBuilder();
        }

        public OrganisationEntityBuilder()
        {
            _options = new DbContextOptions<ApplicationDbContext>();
            _context = new ApplicationDbContext(_options);

            _organisationEntity = new OrganisationEntity()
            {
                Id = new Guid(),
                Name = "Organisation Name",
                OdsCode = "Ods Code",
                LastUpdated = DateTime.Now
            };
        }

        public void Insert()
        {
            var organisation = new Organisation(Guid.NewGuid(), _organisationEntity.Name, _organisationEntity.OdsCode);

            _context.Organisations.Add(organisation);
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
