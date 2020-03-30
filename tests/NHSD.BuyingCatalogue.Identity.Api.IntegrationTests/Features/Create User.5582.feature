Feature: Create Buyer User Account
    As a Authority User
    I want to create an account for Users
    So Users can access the Buying Catalogue

Background:
	Given Organisations exist
		| Name           | OdsCode |
		| Organisation 1 | Ods 1   |
		| Organisation 2 | Ods 2   |
	And Users exist
		| Id     | OrganisationName | FirstName | LastName | Email                | PhoneNumber | Disabled | Password        | OrganisationFunction |
		| 012345 | Organisation 1   | Penny     | Lane     | PennyLane@email.com  | 01234567890 | false    | S0mePa$$w0rd    | Buyer                |
		| 123456 | Organisation 1   | Post      | Pat      | PostmanPat@email.com | 12345678901 | false    | An0therPa$$w0rd | Authority            |

@5582
Scenario: 1. A authority user can create a user
	Given an user is logged in
		| Username             | Password        | Scope        |
		| PostmanPat@email.com | An0therPa$$w0rd | Organisation |
	When a POST request is made to create a user for organisation Organisation 2
		| FirstName | LastName   | PhoneNumber | EmailAddress             | OrganisationName |
		| Bob       | Bobkovitch | 0123456789  | bob.bobkovitch@email.com | Organisation 2   |
	Then a response with status code 200 is returned
	When a GET request is made for an organisation's users with name Organisation 2
	Then a response with status code 200 is returned
	And the Users list is returned with the following values excluding ID
		| FirstName | LastName   | EmailAddress             | PhoneNumber | IsDisabled |
		| Bob       | Bobkovitch | bob.bobkovitch@email.com | 0123456789  | False      |

@5582
Scenario: 2. A non authority user cannot create a user
	Given an user is logged in
		| Username            | Password     | Scope        |
		| PennyLane@email.com | S0mePa$$w0rd | Organisation |
	When a POST request is made to create a user for organisation Organisation 2
		| FirstName | LastName   | PhoneNumber | EmailAddress             | OrganisationName |
		| Bob       | Bobkovitch | 0123456789  | bob.bobkovitch@email.com | Organisation 1   |
	Then a response with status code 403 is returned

@5582
Scenario: 3. Create user with valid details when unauthorised
	When a POST request is made to create a user for organisation Organisation 2
		| FirstName | LastName   | PhoneNumber | EmailAddress             | OrganisationName |
		| Bob       | Bobkovitch | 0123456789  | bob.bobkovitch@email.com | Organisation 1   |
	Then a response with status code 401 is returned

@5582
Scenario: 4. Service Failure
	Given an user is logged in
		| Username             | Password        | Scope        |
		| PostmanPat@email.com | An0therPa$$w0rd | Organisation |
	Given the call to the database will fail
	When a POST request is made to create a user for organisation Organisation 2
		| FirstName | LastName   | PhoneNumber | EmailAddress             | OrganisationName |
		| Bob       | Bobkovitch | 0123456789  | bob.bobkovitch@email.com | Organisation 1   |
	Then a response with status code 500 is returned