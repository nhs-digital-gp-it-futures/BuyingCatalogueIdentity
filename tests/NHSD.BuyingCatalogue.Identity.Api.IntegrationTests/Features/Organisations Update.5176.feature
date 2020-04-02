Feature: Edit Buyer Organisation
    As a Authority User
    I want to edit an Organisation's account
    So that I can change an Organisation's data

Background:
	Given Organisations exist
		| Name           | OdsCode | PrimaryRoleId | CatalogueAgreementSigned | Line1 | Line2      | Line3        | Line4       | Town  | County         | Postcode | Country |
		| Organisation 1 | Ods 1   | ID 1          | true                     | 12    | Brick Lane | Central Area | City Centre | Leeds | West Yorkshire | LS1 1AW  | England |
	And Users exist
		| Id  | OrganisationName | FirstName | LastName | Email             | PhoneNumber | Disabled | Password       | OrganisationFunction |
		| 123 | Organisation 1   | John      | Doe      | authority@doe.com | 01234567890 | false    | Str0nkP4s5w0rd | Authority            |
		| 234 | Organisation 1   | Jane      | Doe      | buyer@doe.com     | 01234567890 | false    | W3AkP4s5w0rd   | Buyer                |

@5176
Scenario: 1. A authority user can update organisations Catalogue Agreement Status
	Given a user is logged in
		| Username          | Password       | Scope        |
		| authority@doe.com | Str0nkP4s5w0rd | Organisation |
	When a PUT request is made to update an organisation with name Organisation 1
		| CatalogueAgreementSigned |
		| false                    |
	Then a response with status code 204 is returned
	When a GET request is made for an organisation with name Organisation 1
	Then a response with status code 200 is returned
	And the Organisation is returned with the following values
		| Name           | OdsCode | PrimaryRoleId | CatalogueAgreementSigned | Line1 | Line2      | Line3        | Line4       | Town  | County         | Postcode | Country |
		| Organisation 1 | Ods 1   | ID 1          | false                    | 12    | Brick Lane | Central Area | City Centre | Leeds | West Yorkshire | LS1 1AW  | England |

@5176
Scenario: 2. A non-authority user can not update organisations
	Given a user is logged in
		| Username      | Password     | Scope        |
		| buyer@doe.com | W3AkP4s5w0rd | Organisation |
	When a PUT request is made to update an organisation with name Organisation 1
		| CatalogueAgreementSigned |
		| false                    |
	Then a response with status code 403 is returned

@5176
Scenario: 3. A user that is not logged in can not update organisations
	When a PUT request is made to update an organisation with name Organisation 1
		| CatalogueAgreementSigned |
		| false                    |
	Then a response with status code 401 is returned

@5176
Scenario: 4. Service Failure
	Given a user is logged in
		| Username          | Password       | Scope        |
		| authority@doe.com | Str0nkP4s5w0rd | Organisation |
	And the call to the database will fail
	When  a PUT request is made to update an organisation with name Organisation 1
		| CatalogueAgreementSigned |
		| false                    |
	Then a response with status code 500 is returned