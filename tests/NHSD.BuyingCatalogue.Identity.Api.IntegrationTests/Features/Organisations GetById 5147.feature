Feature: Display an Organisations by Id in the Authority Section
  As an Authority User
  I want to view an Organisation information for a single organisation
  So that I can manage access to the Buying Catalogue

Background:
	Given Organisations exist
		| Name           | OdsCode | PrimaryRoleId | CatalogueAgreementSigned | Line1 | Line2      | Line3           | Line4       | Town  | County          | Postcode | Country |
		| Organisation 1 | Ods 1   | ID 1          | true                     | 12    | Brick Lane | Central Area    | City Centre | Leeds | West Yorkshire  | LS1 1AW  | England |
		| Organisation 2 | Ods 2   | ID 2          | false                    | 15    | Sun Ave    | End of the Road | Suburb      | York  | North Yorkshire | YO11 4LO | England |
	And Users exist
		| Id     | OrganisationName | FirstName | LastName | Email                | PhoneNumber | Disabled | Password        | OrganisationFunction |
		| 012345 | Organisation 1   | Penny     | Lane     | PennyLane@email.com  | 01234567890 | false    | S0mePa$$w0rd    | Buyer                |
		| 123456 | Organisation 1   | Post      | Pat      | PostmanPat@email.com | 12345678901 | false    | An0therPa$$w0rd | Authority            |

@5147
Scenario: 1. Get the details of a single organisation
	Given a user is logged in
		| Username             | Password        | Scope        |
		| PostmanPat@email.com | An0therPa$$w0rd | Organisation |
	When a GET request is made for an organisation with name Organisation 1
	Then a response with status code 200 is returned
	And the Organisation is returned with the following values
		| Name           | OdsCode | PrimaryRoleId | CatalogueAgreementSigned | Line1 | Line2      | Line3        | Line4       | Town  | County         | Postcode | Country |
		| Organisation 1 | Ods 1   | ID 1          | true                     | 12    | Brick Lane | Central Area | City Centre | Leeds | West Yorkshire | LS1 1AW  | England |

@5147
Scenario: 2. Organisation is not found
	Given a user is logged in
		| Username             | Password        | Scope        |
		| PostmanPat@email.com | An0therPa$$w0rd | Organisation |
	And an Organisation with name Organisation 3 does not exist
	When a GET request is made for an organisation with name Organisation 3
	Then a response with status code 404 is returned

@5147
Scenario: 3. A non authority user cannot access the organisations
	Given a user is logged in
		| Username            | Password     | Scope        |
		| PennyLane@email.com | S0mePa$$w0rd | Organisation |
	When a GET request is made for an organisation's users with name Organisation 1
	Then a response with status code 403 is returned

@5147
Scenario: 4. If a user is not authorised then they cannot access the organisation
	When a GET request is made for an organisation with name Organisation 1
	Then a response with status code 401 is returned

@5147
Scenario: 5. Service Failure
	Given a user is logged in
		| Username             | Password        | Scope        |
		| PostmanPat@email.com | An0therPa$$w0rd | Organisation |
	And the call to the database will fail
	When a GET request is made for an organisation with name Organisation 1
	Then a response with status code 500 is returned