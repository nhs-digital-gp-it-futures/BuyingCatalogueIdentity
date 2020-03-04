﻿CREATE TABLE [dbo].[Organisations]
(
	[Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY, 
    [Name] VARCHAR(255) NOT NULL, 
    [OdsCode] VARCHAR(8) NULL, 
    [LastUpdated] DATETIME2 NOT NULL
)