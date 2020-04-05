Feature: Create Buyer Account Validation
    As an Authority User
    I want to see errors when creating a new user
    So that I know when something has gone wrong

Background:
	Given Organisations exist
		| Name           | OdsCode |
		| Organisation 1 | Ods 1   |
		| Organisation 2 | Ods 2   |
	And Users exist
		| Id     | OrganisationName | FirstName | LastName | Email                | PhoneNumber | Disabled | Password        | OrganisationFunction |
		| 012345 | Organisation 1   | Penny     | Lane     | PennyLane@email.com  | 01234567890 | false    | S0mePa$$w0rd    | Buyer                |
		| 123456 | Organisation 1   | Post      | Pat      | PostmanPat@email.com | 12345678901 | false    | An0therPa$$w0rd | Authority            |
	And a user is logged in
		| Username             | Password        | Scope        |
		| PostmanPat@email.com | An0therPa$$w0rd | Organisation |

@3540
Scenario: Empty first name yields a first name is required error
	When a POST request is made to create a user for organisation Organisation 2
		| FirstName | LastName   | PhoneNumber | EmailAddress             | OrganisationName |
		|           | Bobkovitch | 0123456789  | bob.bobkovitch@email.com | Organisation 2   |
	Then a response with status code 400 is returned
	And the response contains the following errors
		| ErrorMessageId    | FieldName |
		| FirstNameRequired | FirstName |

@3540
Scenario: A fifty one character first name yields a first name is too long error
	When a POST request is made to create a user for organisation Organisation 2
		| FirstName               | LastName   | PhoneNumber | EmailAddress             | OrganisationName |
		| #a string of length 51# | Bobkovitch | 0123456789  | bob.bobkovitch@email.com | Organisation 2   |
	Then a response with status code 400 is returned
	And the response contains the following errors
		| ErrorMessageId   | FieldName |
		| FirstNameTooLong | FirstName |

@3540
Scenario: Empty last name yields a last name is required error
	When a POST request is made to create a user for organisation Organisation 2
		| FirstName | LastName | PhoneNumber | EmailAddress             | OrganisationName |
		| Bob       |          | 0123456789  | bob.bobkovitch@email.com | Organisation 2   |
	Then a response with status code 400 is returned
	And the response contains the following errors
		| ErrorMessageId   | FieldName |
		| LastNameRequired | LastName  |

@3540
Scenario: A fifty one character last name yields a last name is too long error
	When a POST request is made to create a user for organisation Organisation 2
		| FirstName | LastName                | PhoneNumber | EmailAddress             | OrganisationName |
		| Bob       | #a string of length 51# | 0123456789  | bob.bobkovitch@email.com | Organisation 2   |
	Then a response with status code 400 is returned
	And the response contains the following errors
		| ErrorMessageId  | FieldName |
		| LastNameTooLong | LastName  |

@3540
Scenario: Empty phone number yields a phone number is required error
	When a POST request is made to create a user for organisation Organisation 2
		| FirstName | LastName   | PhoneNumber | EmailAddress             | OrganisationName |
		| Bob       | Bobkovitch |             | bob.bobkovitch@email.com | Organisation 2   |
	Then a response with status code 400 is returned
	And the response contains the following errors
		| ErrorMessageId      | FieldName   |
		| PhoneNumberRequired | PhoneNumber |

@3540
Scenario: Empty email address yields an email is required error
	When a POST request is made to create a user for organisation Organisation 2
		| FirstName | LastName   | PhoneNumber | EmailAddress | OrganisationName |
		| Bob       | Bobkovitch | 0123456789  |              | Organisation 2   |
	Then a response with status code 400 is returned
	And the response contains the following errors
		| ErrorMessageId | FieldName    |
		| EmailRequired  | EmailAddress |

@3540
Scenario: A two hundred and fifty seven character email address yields an email is too long error
	When a POST request is made to create a user for organisation Organisation 2
		| FirstName | LastName   | PhoneNumber | EmailAddress                                      | OrganisationName |
		| Bob       | Bobkovitch | 0123456789  | #a string of length 128#@#a string of length 128# | Organisation 2   |
	Then a response with status code 400 is returned
	And the response contains the following errors
		| ErrorMessageId | FieldName    |
		| EmailTooLong   | EmailAddress |

@3540
Scenario Outline: Email address format combinations yields an invalid email format error
	When a POST request is made to create a user for organisation Organisation 2
		| FirstName | LastName   | PhoneNumber | EmailAddress   | OrganisationName |
		| Bob       | Bobkovitch | 0123456789  | <EmailAddress> | Organisation 2   |
	Then a response with status code 400 is returned
	And the response contains the following errors
		| ErrorMessageId     | FieldName    |
		| EmailInvalidFormat | EmailAddress |

	Examples:
		| EmailAddress |
		| test         |
		| test@        |
		| @test        |
		| test.com     |
		| @            |

@3540
Scenario: Duplicate email address yields an email already exists error
	Given Users exist
		| Id | FirstName | LastName   | Email                    | OrganisationName |
		| U1 | Bob       | Bobkovitch | bob.bobkovitch@email.com | Organisation 2   |
	When a POST request is made to create a user for organisation Organisation 2
		| FirstName | LastName   | PhoneNumber | EmailAddress             | OrganisationName |
		| Bob       | Bobkovitch | 0123456789  | bob.bobkovitch@email.com | Organisation 2   |
	Then a response with status code 400 is returned
	And the response contains the following errors
		| ErrorMessageId     | FieldName    |
		| EmailAlreadyExists | EmailAddress |