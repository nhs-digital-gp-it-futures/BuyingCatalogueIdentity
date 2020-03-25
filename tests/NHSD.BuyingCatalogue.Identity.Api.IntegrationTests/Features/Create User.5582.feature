Feature: Create Buyer User Account
    As a Authority User
    I want to create an account for Users
    So Users can access the Buying Catalogue

Background:
	Given Organisations exist
		| Name           | OdsCode | PrimaryRoleId | CatalogueAgreementSigned | Line1 | Line2      | Line3        | Line4       | Town  | County         | Postcode | Country |
		| Organisation 1 | Ods 1   | ID 1          | true                     | 12    | Brick Lane | Central Area | City Centre | Leeds | West Yorkshire | LS1 1AW  | England |

@5582
Scenario: 1. Create user with valid details
	Given an user is logged in
		| Username           | Password | Scope        |
		| BobSmith@email.com | Pass123$ | Organisation |
	When a user is created for organisation Organisation 1
		| FirstName | LastName   | PhoneNumber | EmailAddress             | OrganisationName |
		| Bob       | Bobkovitch | 0123456789  | bob.bobkovitch@email.com | Organisation 1   |
	Then a response with status code 200 is returned
	When a GET request is made for an organisation's users with name Organisation 1
	Then a response with status code 200 is returned
	And the Users list is returned with the following values excluding ID
		| FirstName | LastName   | EmailAddress             | PhoneNumber | IsDisabled |
		| Bob       | Bobkovitch | bob.bobkovitch@email.com | 0123456789  | False      |

@5582
Scenario: 2. Create user with valid details when unauthorised
	When a user is created for organisation Organisation 1
		| FirstName | LastName   | PhoneNumber | EmailAddress             | OrganisationName |
		| Bob       | Bobkovitch | 0123456789  | bob.bobkovitch@email.com | Organisation 1   |
	Then a response with status code 401 is returned

@5582
Scenario: 3. Service Failure
	Given an user is logged in
		| Username           | Password | Scope        |
		| BobSmith@email.com | Pass123$ | Organisation |
	Given the call to the database will fail
	When a user is created for organisation Organisation 1
		| FirstName | LastName   | PhoneNumber | EmailAddress             | OrganisationName |
		| Bob       | Bobkovitch | 0123456789  | bob.bobkovitch@email.com | Organisation 1   |
	Then a response with status code 500 is returned