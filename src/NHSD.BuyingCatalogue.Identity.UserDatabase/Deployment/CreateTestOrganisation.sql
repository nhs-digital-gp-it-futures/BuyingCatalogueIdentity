DECLARE @name AS varchar(255) = 'NHS Digital';

IF NOT EXISTS(
  SELECT *
  FROM dbo.Organisations
  WHERE [Name] = @name)

BEGIN
    INSERT INTO dbo.Organisations (Id, [Name], OdsCode, LastUpdated)
	VALUES
	(NEWID(), @name, 'ODS 1', GETDATE());
END;