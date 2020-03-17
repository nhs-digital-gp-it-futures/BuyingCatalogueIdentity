CREATE TABLE [dbo].[Organisations]
(
	OrganisationId UNIQUEIDENTIFIER NOT NULL,
    [Name] NVARCHAR(255) NOT NULL,    
    [Address] NVARCHAR(MAX) NULL,
    OdsCode NVARCHAR(8) NULL,
    PrimaryRoleId NVARCHAR(8) NULL,
    CatalogueAgreementSigned BIT NOT NULL,
    LastUpdated DATETIME2(7) NOT NULL,
    CONSTRAINT PK_Organisations PRIMARY KEY NONCLUSTERED (OrganisationId),
    INDEX IX_OrganisationName CLUSTERED ([Name])
);
