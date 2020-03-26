Feature: Send Email Confirmation Link
    As a Authority User
    I want to send confirmation link to users after creating their accounts
    So Users can complete their registration

Background:
	Given Organisations exist
		| Name           | OdsCode |
		| Organisation 1 | Ods 1   |
	And Users exist
		| Id     | OrganisationName | FirstName | LastName | Email                | PhoneNumber | Disabled | Password        | OrganisationFunction |
		| 012345 | Organisation 1   | Penny     | Lane     | PennyLane@email.com  | 01234567890 | false    | S0mePa$$w0rd    | Buyer                |
		| 123456 | Organisation 1   | Post      | Pat      | PostmanPat@email.com | 12345678901 | false    | An0therPa$$w0rd | Authority            |

@3540
Scenario: 1. User receives email after their account is created
	Given an user is logged in
		| Username             | Password        | Scope        |
		| PostmanPat@email.com | An0therPa$$w0rd | Organisation |
	When a POST request is made to create a user for organisation Organisation 1
		| FirstName | LastName   | PhoneNumber | EmailAddress             | OrganisationName |
		| Bob       | Bobkovitch | 0123456789  | bob.bobkovitch@email.com | Organisation 1   |
	Then a response with status code 200 is returned
	And an email containing the following information is sent
		| From                 | To                       | Subject          | Body                                                                                                      |
		| admin.user@email.com | bob.bobkovitch@email.com | Account Creation | Hello, please click this link to complete your registration http://nhsd.create.users.co.uk/b0bsS3cr3tL1nk |
	And the sent email contains the following information
		| From                 | To                       | Subject          | Body                                          |
		| admin.user@email.com | bob.bobkovitch@email.com | Account Creation | http://nhsd.create.users.co.uk/b0bsS3cr3tL1nk |

@3540
Scenario: 2. User does not receive email when non-authority user tries to create their account
	Given an user is logged in
		| Username            | Password     | Scope |
		| PennyLane@email.com | S0mePa$$w0rd | NULL  |
	When a POST request is made to create a user for organisation Organisation 1
		| FirstName | LastName   | PhoneNumber | EmailAddress             | OrganisationName |
		| Bob       | Bobkovitch | 0123456789  | bob.bobkovitch@email.com | Organisation 1   |
	Then a response with status code 401 is returned
	And email inbox contains no new emails

@3540
Scenario: 3. User does not receive email because of service failure
	Given an user is logged in
		| Username             | Password        | Scope        |
		| PostmanPat@email.com | An0therPa$$w0rd | Organisation |
	And the call to the database will fail
	When a POST request is made to create a user for organisation Organisation 1
		| FirstName | LastName   | PhoneNumber | EmailAddress             | OrganisationName |
		| Bob       | Bobkovitch | 0123456789  | bob.bobkovitch@email.com | Organisation 1   |
	Then a response with status code 500 is returned
	And email inbox contains no new emails
