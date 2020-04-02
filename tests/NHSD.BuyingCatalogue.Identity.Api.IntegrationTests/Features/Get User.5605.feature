Feature: Get a User by Id
	As an Authority User
	I want to be able to view a User
	So that I can ensure the information is correct

Background:
	Given Organisations exist
		| Name           | OdsCode |
		| Organisation 1 | Ods 1   |
	And Users exist
		| Id  | OrganisationName | FirstName | LastName | Email             | PhoneNumber | Disabled | Password | OrganisationFunction |
		| 123 | Organisation 1   | John      | Doe      | authority@doe.com | 01234567890 | false    | yolo     | Authority            |
		| 234 | Organisation 1   | Jane      | Doe      | buyer@doe.com     | 01234567890 | false    | oloy     | Buyer                |

@5605
Scenario: 1. Searching for an existing user
	Given a user is logged in
		| Username          | Password | Scope        |
		| authority@doe.com | yolo     | Organisation |
	When a GET request is made for a user with id 234
	Then a response with status code 200 is returned
	And a user is returned with the following values
		| Name     | PhoneNumber | EmailAddress  | Disabled | OrganisationName |
		| Jane Doe | 01234567890 | buyer@doe.com | false    | Organisation 1   |

@5605
Scenario: 2. Searching for a user that does not exist
	Given a user is logged in
		| Username          | Password | Scope        |
		| authority@doe.com | yolo     | Organisation |
	When a GET request is made for a user with id unknown
	Then a response with status code 404 is returned

@5605
Scenario: 3. If a user is not authorised then they cannot search for a user
	Given a user is logged in
		| Username      | Password | Scope        |
		| buyer@doe.com | oloy     | Organisation |
	When a GET request is made for a user with id 234
	Then a response with status code 403 is returned

@5605
Scenario: 4. If a user is not logged in then they cannot search for a user
	When a GET request is made for a user with id 234
	Then a response with status code 401 is returned

@5605
Scenario: 5. Service Failure
	Given a user is logged in
		| Username          | Password | Scope        |
		| authority@doe.com | yolo     | Organisation |
	Given the call to the database will fail
	When a GET request is made for a user with id 234
	Then a response with status code 500 is returned
