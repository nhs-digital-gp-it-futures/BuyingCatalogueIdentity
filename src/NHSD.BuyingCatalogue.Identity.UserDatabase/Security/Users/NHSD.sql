﻿CREATE USER NHSD
	FOR LOGIN NHSD
	WITH DEFAULT_SCHEMA = dbo;
GO

GRANT CONNECT TO NHSD;
GO

ALTER ROLE db_owner
ADD MEMBER NHSD;
