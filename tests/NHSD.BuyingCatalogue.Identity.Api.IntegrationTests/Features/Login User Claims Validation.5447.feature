Feature: Checking that the users claims are correct when they login
	As an Authority Auser
	I want to login and view the pages that my scope specifies
	So that I can make full use of the system

Background:
	Given Organisations exist
		| Name           | OdsCode |
		| Organisation 1 | Ods 1   |
	And Users exist
		| Id     | OrganisationName | FirstName | LastName | Email                | PhoneNumber | Disabled | Password        | OrganisationFunction |
		| 012345 |                  | Penny     | Lane     | PennyLane@email.com  | 01234567890 | false    | S0mePa$$w0rd    | Buyer                |
		| 123456 | Organisation 1   | Post      | Pat      | PostmanPat@email.com | 12345678901 | false    | An0therPa$$w0rd | Authority            |

@5447
Scenario: 1. Null scope yields the expected claims
	Given a user is logged in
		| Username            | Password     | Scope |
		| PennyLane@email.com | S0mePa$$w0rd | NULL  |
	And the claims contains the following information
		| ClaimType            | ClaimValue          |
		| client_id            | PasswordClient      |
		| sub                  | 012345              |
		| idp                  | local               |
		| preferred_username   | PennyLane@email.com |
		| unique_name          | PennyLane@email.com |
		| given_name           | Penny               |
		| family_name          | Lane                |
		| name                 | Penny Lane          |
		| email                | PennyLane@email.com |
		| email_verified       | true                |
		| organisationFunction | Buyer               |
		| scope                | profile             |

@5447
Scenario: 2. Unknown scope yields no access token
	Given a user is logged in
		| Username            | Password     | Scope   |
		| PennyLane@email.com | S0mePa$$w0rd | Unknown |
	Then the access token should be empty

@5447
Scenario: 3. Get the claims of a buyer user
	Given a user is logged in
		| Username            | Password     | Scope   |
		| PennyLane@email.com | S0mePa$$w0rd | profile |
	And the claims contains the following information
		| ClaimType            | ClaimValue          |
		| client_id            | PasswordClient      |
		| sub                  | 012345              |
		| idp                  | local               |
		| preferred_username   | PennyLane@email.com |
		| unique_name          | PennyLane@email.com |
		| given_name           | Penny               |
		| family_name          | Lane                |
		| name                 | Penny Lane          |
		| email                | PennyLane@email.com |
		| email_verified       | true                |
		| organisationFunction | Buyer               |
		| scope                | profile             |

@5447
Scenario: 4. Get the claims of an authority user
	Given a user is logged in
		| Username             | Password        | Scope        |
		| PostmanPat@email.com | An0therPa$$w0rd | Organisation |
	And the claims contains the following information
		| ClaimType            | ClaimValue           |
		| aud                  | Organisation         |
		| client_id            | PasswordClient       |
		| sub                  | 123456               |
		| idp                  | local                |
		| preferred_username   | PostmanPat@email.com |
		| unique_name          | PostmanPat@email.com |
		| given_name           | Post                 |
		| family_name          | Pat                  |
		| name                 | Post Pat             |
		| email                | PostmanPat@email.com |
		| email_verified       | true                 |
		| organisationFunction | Authority            |
		| scope                | Organisation         |