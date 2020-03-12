﻿CREATE TABLE [dbo].[Organisations]
(
	Id UNIQUEIDENTIFIER NOT NULL,
    [Name] NVARCHAR(255) NOT NULL,    
    [Address] NVARCHAR(255) NULL,
    OdsCode NVARCHAR(8) NULL,
    LastUpdated DATETIME2(7) NOT NULL,
    CONSTRAINT PK_Organisations PRIMARY KEY NONCLUSTERED (Id),
    INDEX IX_OrganisationName CLUSTERED ([Name])
);
