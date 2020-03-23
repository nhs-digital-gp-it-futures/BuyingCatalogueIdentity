Feature: Display an Organisations by Id in the Authority Section
  As an Authority User
  I want to view an Organisation information for a single orgnisation
  So that I can manage access to the Buying Catalogue

Background:
    Given Organisations exist
        | Name           | OdsCode | PrimaryRoleId | CatalogueAgreementSigned | Line1 | Line2      | Line3           | Line4       | Town  | County          | Postcode | Country |
        | Organisation 1 | Ods 1   | ID 1          | true                     | 12    | Brick Lane | Central Area    | City Centre | Leeds | West Yorkshire  | LS1 1AW  | England |
        | Organisation 2 | Ods 2   | ID 2          | false                    | 15    | Sun Ave    | End of the Road | Suburb      | York  | North Yorkshire | YO11 4LO | England |

@5147
Scenario: 1. Get the details of a single organisation
	Given an user is logged in
		| Username           | Password | Scope                |
		| BobSmith@email.com | Pass123$ | profile Organisation |
	When a GET request is made for an organisation with name Organisation 1
	Then a response with status code 200 is returned
	And the Organisation is returned with the following values
		| Name           | OdsCode | PrimaryRoleId | CatalogueAgreementSigned | Line1 | Line2      | Line3        | Line4       | Town  | County         | Postcode | Country |
		| Organisation 1 | Ods 1   | ID 1          | true                     | 12    | Brick Lane | Central Area | City Centre | Leeds | West Yorkshire | LS1 1AW  | England |

@5147
Scenario: 2. Organisation is not found
	Given an user is logged in
		| Username           | Password | Scope        |
		| BobSmith@email.com | Pass123$ | Organisation |
	And an Organisation with name Organisation 3 does not exist
	When a GET request is made for an organisation with name Organisation 3
	Then a response with status code 404 is returned

@5147
Scenario: 3. If a user is not authorised then they cannot access the organisation
    When a GET request is made for an organisation with name Organisation 1
    Then a response with status code 401 is returned

@5147
Scenario: 4. Service Failure
	Given an user is logged in
		| Username           | Password | Scope        |
		| BobSmith@email.com | Pass123$ | Organisation |
	And the call to the database to set the field will fail
	When a GET request is made for an organisation with name Organisation 1
	Then a response with status code 500 is returned
	Then a response with status code 500 is returned
