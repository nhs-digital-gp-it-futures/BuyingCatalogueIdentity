Feature: Checking that the users claims are correct when they login
	As an Authority Auser
	I want to login and view the pages that my scope specifies
	So that I can make full use of the system

Background:
	Given Organisations exist
		| Name           | OdsCode |
		| Organisation 1 | Ods 1   |
	And Users exist
		| Id     | OrganisationName | FirstName | LastName | Email                | PhoneNumber | Disabled | Password        |
		| 012345 |                  | Penny     | Lane     | PennyLane@email.com  | 01234567890 | false    | S0mePa$$w0rd    |
		| 123456 | Organisation 1   | Post      | Pat      | PostmanPat@email.com | 12345678901 | false    | An0therPa$$w0rd |

@5447
Scenario: 1. Get the claims of a user who provides no scope
	Given an authority user is logged in
		| Username            | Password     | Scope |
		| PennyLane@email.com | S0mePa$$w0rd | NULL  |
    Then the access token should be empty

@5447
Scenario: 2. Get the claims of a user who's scope allows them to not access anything
	Given an authority user is logged in
		| Username            | Password     | Scope   |
		| PennyLane@email.com | S0mePa$$w0rd | profile |
	And the claims contains the following information
		| client_id      | sub    | idp   | preferred_username  | unique_name         | given_name | family_name | name       | email               | email_verified | organisationFunction | scope   |
		| PasswordClient | 012345 | local | PennyLane@email.com | PennyLane@email.com | Penny      | Lane        | Penny Lane | PennyLane@email.com | true           | TestUser             | profile |

@5447
Scenario: 3. Get the claims of a user who can access the organisations
	Given an authority user is logged in
		| Username             | Password        | Scope        |
		| PostmanPat@email.com | An0therPa$$w0rd | Organisation |
	And the claims contains the following information
		| aud          | client_id      | sub    | idp   | preferred_username   | unique_name          | given_name | family_name | name     | email                | email_verified | organisationFunction | scope        |
		| Organisation | PasswordClient | 123456 | local | PostmanPat@email.com | PostmanPat@email.com | Post       | Pat         | Post Pat | PostmanPat@email.com | true           | TestUser             | Organisation |