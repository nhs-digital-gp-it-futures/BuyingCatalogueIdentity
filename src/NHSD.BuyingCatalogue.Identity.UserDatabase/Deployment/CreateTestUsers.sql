IF NOT EXISTS(
  SELECT *
  FROM dbo.AspNetUsers
  WHERE UserName IN ('alice', 'bob'))
BEGIN
	DECLARE @address AS nchar(108) = N'{ "street_address": "One Hacker Way", "locality": "Heidelberg", "postal_code": 69118, "country": "Germany" }';

	DECLARE @aliceId AS nchar(36) = CAST(NEWID() AS nchar(36));
	DECLARE @bobId AS nchar(36) = CAST(NEWID() AS nchar(36));

	-- 'Pass123$'
	DECLARE @alicePassword AS nvarchar(200) = N'AQAAAAEAACcQAAAAEFSsEthAqGVBLj1P1gF9puxtXm18lKHlmuh9J/Ib0KKBO3GjQvxymJbzpSqy0zuOHg==';

	-- 'Pass123$'
	DECLARE @bobPassword AS nvarchar(200) = N'AQAAAAEAACcQAAAAEOzr1Zwpoo1pKsTa+S65mBZVG4GIy6IYH/IAED6TvBA+FIMg8u/xb0b/cfexV7SHNw==';

	INSERT INTO dbo.AspNetUsers(Id, AccessFailedCount, ConcurrencyStamp, EmailConfirmed, LockoutEnabled,
		NormalizedUserName, PasswordHash, PhoneNumberConfirmed, SecurityStamp, TwoFactorEnabled, UserName)
	VALUES
	(@aliceId, 0, NEWID(), 0, 1, 'ALICE', @alicePassword, 0, 'NNJ4SLBPCVUDKXAQXJHCBKQTFEYUAPBC', 0, 'alice'),
	(@bobId, 0, NEWID(), 0, 1, 'BOB', @bobPassword, 0, 'OBDOPOU5YQ5WQXCR3DITKL6L5IDPYHHJ', 0, 'bob');

	INSERT INTO dbo.AspNetUserClaims (ClaimType, ClaimValue, UserId)
	VALUES
	(N'name', N'Alice Smith', @aliceId),
	(N'given_name', N'Alice', @aliceId),
	(N'family_name', N'Smith', @aliceId),
	(N'email', N'AliceSmith@email.com', @aliceId),
	(N'email_verified', N'true', @aliceId),
	(N'website', N'http://alice.com/', @aliceId),
	(N'address', @address, @aliceId),
	(N'name', N'Bob Smith', @bobId),
	(N'given_name', N'Bob', @bobId),
	(N'family_name', N'Smith', @bobId),
	(N'email', N'BobSmith@email.com', @bobId),
	(N'email_verified', N'true', @bobId),
	(N'location', N'somewhere', @bobId),
	(N'website', N'http://bob.com/', @bobId),
	(N'address', @address, @bobId);
END;
