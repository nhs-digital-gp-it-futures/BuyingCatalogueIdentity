Feature: Display all the Organisations in an Authority Section
    As a Authority User
    I want to view all of the Organisations in the solution
    So that I can ensure the information is correct

Background:
    Given Organisations exist
        | Name           | OdsCode | PrimaryRoleId | CatalogueAgreementSigned | Line1 | Line2      | Line3           | Line4           | Town  | County          | Postcode | Country |
        | Organisation 1 | Ods 1   | ID 1          | true                     | 12    | Brick Lane | Central Area    | City Centre     | Leeds | West Yorkshire  | LS1 1AW  | England |
        | Organisation 2 | Ods 2   | ID 2          | false                    | 15    | Sun Ave    | End of the Road | Suburb          | York  | North Yorkshire | YO11 4LO | England |
        | Organisation 3 | Ods 3   | ID 3          | true                     | 9     | Lime Road  |                 | Chapel Allerton | Leeds | West Yorkshire  | LS7 2AL  | England |

@5146
Scenario: 1. Get all of the organisations
	Given an user is logged in
		| Username           | Password | Scope        |
		| BobSmith@email.com | Pass123$ | Organisation |
	When a GET request is made for the Organisations section
	Then a response with status code 200 is returned
	And the Organisations list is returned with the following values
		| Name           | OdsCode | PrimaryRoleId | CatalogueAgreementSigned | Line1 | Line2      | Line3           | Line4           | Town  | County          | Postcode | Country |
		| Organisation 1 | Ods 1   | ID 1          | true                     | 12    | Brick Lane | Central Area    | City Centre     | Leeds | West Yorkshire  | LS1 1AW  | England |
		| Organisation 2 | Ods 2   | ID 2          | false                    | 15    | Sun Ave    | End of the Road | Suburb          | York  | North Yorkshire | YO11 4LO | England |
		| Organisation 3 | Ods 3   | ID 3          | true                     | 9     | Lime Road  |                 | Chapel Allerton | Leeds | West Yorkshire  | LS7 2AL  | England |

@5147
Scenario: 2. If a user is not authorised then they cannot access the organisations
    When a GET request is made for the Organisations section
    Then a response with status code 401 is returned

@5146
Scenario: 3. Service Failure
	Given an user is logged in
		| Username           | Password | Scope        |
		| BobSmith@email.com | Pass123$ | Organisation |
	Given the call to the database to set the field will fail
	When a GET request is made for the Organisations section
	Then a response with status code 500 is returned
	Then a response with status code 500 is returned
