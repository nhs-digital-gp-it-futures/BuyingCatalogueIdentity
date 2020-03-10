DECLARE @aliceEmail AS nvarchar(50) = N'AliceSmith@email.com';
DECLARE @bobEmail AS nvarchar(50) = N'BobSmith@email.com';

IF NOT EXISTS(
  SELECT *
  FROM dbo.AspNetUsers
  WHERE UserName IN (@aliceEmail, @bobEmail))
BEGIN
	DECLARE @address AS nchar(108) = N'{ "street_address": "One Hacker Way", "locality": "Heidelberg", "postal_code": 69118, "country": "Germany" }';

	DECLARE @aliceId AS nchar(36) = CAST(NEWID() AS nchar(36));
	DECLARE @bobId AS nchar(36) = CAST(NEWID() AS nchar(36));

	DECLARE @aliceNormalizedEmail AS nvarchar(50) = UPPER(@aliceEmail);
    DECLARE @bobNormalizedEmail AS nvarchar(50) = UPPER(@bobEmail);

    -- 'Pass123$'
	DECLARE @alicePassword AS nvarchar(200) = N'AQAAAAEAACcQAAAAEFSsEthAqGVBLj1P1gF9puxtXm18lKHlmuh9J/Ib0KKBO3GjQvxymJbzpSqy0zuOHg==';

	-- 'Pass123$'
	DECLARE @bobPassword AS nvarchar(200) = N'AQAAAAEAACcQAAAAEOzr1Zwpoo1pKsTa+S65mBZVG4GIy6IYH/IAED6TvBA+FIMg8u/xb0b/cfexV7SHNw==';

	INSERT INTO dbo.AspNetUsers(Id, UserName, NormalizedUserName, Email, NormalizedEmail, AccessFailedCount, ConcurrencyStamp,
        EmailConfirmed, LockoutEnabled, PasswordHash, PhoneNumberConfirmed, SecurityStamp, TwoFactorEnabled)
	VALUES
	(@aliceId, @aliceEmail, @aliceNormalizedEmail, @aliceEmail, @aliceNormalizedEmail, 0, NEWID(), 0, 1, @alicePassword, 0, 'NNJ4SLBPCVUDKXAQXJHCBKQTFEYUAPBC', 0),
	(@bobId, @bobEmail, @bobNormalizedEmail, @bobEmail, @bobNormalizedEmail, 0, NEWID(), 0, 1, @bobPassword, 0, 'OBDOPOU5YQ5WQXCR3DITKL6L5IDPYHHJ', 0);

	INSERT INTO dbo.AspNetUserClaims (ClaimType, ClaimValue, UserId)
	VALUES
	(N'name', N'Alice Smith', @aliceId),
	(N'given_name', N'Alice', @aliceId),
	(N'family_name', N'Smith', @aliceId),
	(N'email_verified', N'true', @aliceId),
	(N'website', N'http://alice.com/', @aliceId),
	(N'address', @address, @aliceId),
	(N'name', N'Bob Smith', @bobId),
	(N'given_name', N'Bob', @bobId),
	(N'family_name', N'Smith', @bobId),
	(N'email_verified', N'true', @bobId),
	(N'location', N'somewhere', @bobId),
	(N'website', N'http://bob.com/', @bobId),
	(N'address', @address, @bobId);
END;
