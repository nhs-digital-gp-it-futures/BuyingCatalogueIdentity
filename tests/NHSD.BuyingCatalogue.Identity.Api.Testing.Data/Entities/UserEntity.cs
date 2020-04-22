using System;

namespace NHSD.BuyingCatalogue.Identity.Api.Testing.Data.Entities
{
    public sealed class UserEntity : EntityBase
    {
        public string PasswordHash { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public bool Disabled { get; set; }

        public Guid OrganisationId { get; set; }

        public string OrganisationFunction { get; set; }

        public string SecurityStamp { get; set; }

        public string Id { get; set; }

        public bool CatalogueAgreementSigned { get; set; }

        protected override string InsertSql => @"
            INSERT INTO dbo.AspNetUsers
                (Id, UserName, NormalizedUserName, Email, NormalizedEmail, EmailConfirmed,
	            PasswordHash, SecurityStamp, ConcurrencyStamp, PhoneNumber, PhoneNumberConfirmed,
	            TwoFactorEnabled, LockoutEnabled, AccessFailedCount,
	            PrimaryOrganisationId, OrganisationFunction, [Disabled], CatalogueAgreementSigned,
	            FirstName, LastName)
            VALUES
                (@id, @email, UPPER(@email), @email, UPPER(@email), 1,
	            @passwordHash, @securityStamp, NEWID(), @phoneNumber, 1,
	            0, 0, 0,
	            @organisationId, @organisationFunction, @disabled, @catalogueAgreementSigned,
	            @firstName, @lastName);";

        protected override string GetSql => @"
              SELECT FirstName, LastName, PhoneNumber, Email AS EmailAddress, Disabled, dbo.Organisations.Name AS OrganisationName
              FROM dbo.AspNetUsers
              INNER JOIN dbo.Organisations
              ON dbo.AspNetUsers.PrimaryOrganisationId = dbo.Organisations.OrganisationId
              WHERE Id = @id";
    }
}
