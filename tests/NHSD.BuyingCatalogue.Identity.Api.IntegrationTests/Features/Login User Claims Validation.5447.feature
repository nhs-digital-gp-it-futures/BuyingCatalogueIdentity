Feature: Checking that the users claims are correct when they login
	As an Authority Auser
	I want to login and view the pages that my scope specifies
	So that I can make full use of the system

Background:
	Given Organisations exist
		| Name           | OdsCode |
		| Organisation 1 | Ods 1   |
		| Organisation 2 | Ods 2   |
	And Users exist
		| Id     | OrganisationName | FirstName | LastName | Email                  | PhoneNumber | Disabled | Password        | OrganisationFunction |
		| 012345 |                  | Penny     | Lane     | PennyLane@email.com    | 01234567890 | false    | S0mePa$$w0rd    | Buyer                |
		| 901234 | Organisation 1   | Snake     | Pliskin  | SnakePliskin@email.com | 23456789012 | false    | S0mePa$$w0rd    | Buyer                |
		| 890123 |                  | Jack      | Burton   | JackBurton@email.com   | 34567890123 | false    | An0therPa$$w0rd | Authority            |
		| 123456 | Organisation 2   | Post      | Pat      | PostmanPat@email.com   | 12345678901 | false    | An0therPa$$w0rd | Authority            |

@5447
Scenario: 1. Null scope yields the expected claims
	Given a user is logged in
		| Username            | Password     | Scope |
		| PennyLane@email.com | S0mePa$$w0rd | NULL  |
	And the claims contains the following information
		| ClaimType               | ClaimValue          |
		| client_id               | PasswordClient      |
		| sub                     | 012345              |
		| idp                     | local               |
		| preferred_username      | PennyLane@email.com |
		| unique_name             | PennyLane@email.com |
		| given_name              | Penny               |
		| family_name             | Lane                |
		| name                    | Penny Lane          |
		| email                   | PennyLane@email.com |
		| email_verified          | true                |
		| organisationFunction    | Buyer               |
		| scope                   | profile             |
		| ordering                | Manage              |

@5447
Scenario: 2. Unknown scope yields no access token
	Given a user is logged in
		| Username            | Password     | Scope   |
		| PennyLane@email.com | S0mePa$$w0rd | Unknown |
	Then the access token should be empty

@5447
Scenario: 3. Get the claims of a buyer user with invalid primary organisation id
	Given a user is logged in
		| Username            | Password     | Scope   |
		| PennyLane@email.com | S0mePa$$w0rd | profile |
	And the claims contains the following information
		| ClaimType               | ClaimValue          |
		| client_id               | PasswordClient      |
		| sub                     | 012345              |
		| idp                     | local               |
		| preferred_username      | PennyLane@email.com |
		| unique_name             | PennyLane@email.com |
		| given_name              | Penny               |
		| family_name             | Lane                |
		| name                    | Penny Lane          |
		| email                   | PennyLane@email.com |
		| email_verified          | true                |
		| organisationFunction    | Buyer               |
		| scope                   | profile             |
		| ordering                | Manage              |

Scenario: 4. Get the claims of a buyer user with valid primary organisation id
	Given a user is logged in
		| Username            | Password     | Scope   |
		| SnakePliskin@email.com | S0mePa$$w0rd | profile |
	And the claims contains the following information
		| ClaimType               | ClaimValue             |
		| client_id               | PasswordClient         |
		| sub                     | 901234                 |
		| idp                     | local                  |
		| preferred_username      | SnakePliskin@email.com |
		| unique_name             | SnakePliskin@email.com |
		| given_name              | Snake                  |
		| family_name             | Pliskin                |
		| name                    | Snake Pliskin          |
		| email                   | SnakePliskin@email.com |
		| email_verified          | true                   |
		| organisationFunction    | Buyer                  |
		| scope                   | profile                |
		| ordering                | Manage                 |
		| primaryOrganisationName | Organisation 1         |


@5447
Scenario: 5. Get the claims of an authority user with valid primary organisation id
	Given a user is logged in
		| Username             | Password        | Scope        |
		| PostmanPat@email.com | An0therPa$$w0rd | Organisation |
	And the claims contains the following information
		| ClaimType               | ClaimValue           |
		| aud                     | Organisation         |
		| client_id               | PasswordClient       |
		| sub                     | 123456               |
		| idp                     | local                |
		| preferred_username      | PostmanPat@email.com |
		| unique_name             | PostmanPat@email.com |
		| given_name              | Post                 |
		| family_name             | Pat                  |
		| name                    | Post Pat             |
		| email                   | PostmanPat@email.com |
		| email_verified          | true                 |
		| organisationFunction    | Authority            |
		| scope                   | Organisation         |
		| organisation            | Manage               |
		| account                 | Manage               |
		| primaryOrganisationName | Organisation 2       |

@5447
Scenario: 6. Get the claims of an authority user with invalid primary organisation id
	Given a user is logged in
		| Username             | Password        | Scope        |
		| JackBurton@email.com | An0therPa$$w0rd | Organisation |
	And the claims contains the following information
		| ClaimType            | ClaimValue           |
		| aud                  | Organisation         |
		| client_id            | PasswordClient       |
		| sub                  | 890123               |
		| idp                  | local                |
		| preferred_username   | JackBurton@email.com |
		| unique_name          | JackBurton@email.com |
		| given_name           | Jack                 |
		| family_name          | Burton               |
		| name                 | Jack Burton          |
		| email                | JackBurton@email.com |
		| email_verified       | true                 |
		| organisationFunction | Authority            |
		| scope                | Organisation         |
		| organisation         | Manage               |
		| account              | Manage               |
