DECLARE @name AS varchar(255) = 'NHS Digital';

IF NOT EXISTS(
  SELECT *
  FROM dbo.Organisations
  WHERE [Name] = @name)

BEGIN
    INSERT INTO dbo.Organisations (OrganisationId, [Name], OdsCode, LastUpdated)
	VALUES
	('FFE7CB2F-9494-4CC7-A348-420D502956D9', @name, 'ODS 1', GETDATE());
END;