IF '$(CREATE_EA_USER)' = 'True'
AND NOT EXISTS (
    SELECT *
      FROM dbo.AspNetUsers
      WHERE UserName = '$(EA_USER_NAME)')
BEGIN
    DECLARE @normalizedUserName AS nvarchar(256) = UPPER('$(EA_USER_NAME)');

    DECLARE @executiveAgencyRoleId AS nchar(5) = 'RO116';
    DECLARE @organisationId AS uniqueidentifier = (SELECT OrganisationId FROM dbo.Organisations WHERE PrimaryRoleId = @executiveAgencyRoleId);

    INSERT INTO dbo.AspNetUsers
    (
        Id, UserName, NormalizedUserName, PasswordHash, 
        FirstName, LastName, Email, NormalizedEmail, EmailConfirmed, PhoneNumber, PhoneNumberConfirmed,
        PrimaryOrganisationId, OrganisationFunction, CatalogueAgreementSigned, [Disabled],
        AccessFailedCount, ConcurrencyStamp, LockoutEnabled, SecurityStamp, TwoFactorEnabled
    )
	VALUES
	(CAST(NEWID() AS nchar(36)), '$(EA_USER_NAME)', @normalizedUserName, '$(EA_USER_PASSWORD_HASH)',
        '$(EA_USER_FIRST_NAME)', '$(EA_USER_LAST_NAME)', '$(EA_USER_NAME)', @normalizedUserName, 1, '$(EA_USER_PHONE)', 1,
        @organisationId, 'Authority', 1, 0,
        0, NEWID(), 1, NEWID(), 0);
END;
GO
