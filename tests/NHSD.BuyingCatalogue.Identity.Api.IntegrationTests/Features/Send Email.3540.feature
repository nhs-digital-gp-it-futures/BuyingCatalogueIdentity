Feature: Send Email Confirmation Link
    As an Authority User
    I want to send a reset password link to users after creating their accounts
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
	Given a user is logged in
		| Username             | Password        | Scope        |
		| PostmanPat@email.com | An0therPa$$w0rd | Organisation |
	When a POST request is made to create a user for organisation Organisation 1
		| FirstName | LastName   | PhoneNumber | EmailAddress             | OrganisationName |
		| Bob       | Bobkovitch | 0123456789  | bob.bobkovitch@email.com | Organisation 1   |
	Then a response with status code 200 is returned
	And the email sent contains the following information
		| From                           | To                       | Subject                           | ResetPasswordLink         |
		| noreply@buyingcatalogue.nhs.uk | bob.bobkovitch@email.com | Set password for Buying Catalogue | https://www.google.co.uk/ |
    And only one email is sent

@3540
Scenario: 2. User does not receive email when non-authority user tries to create their account
	Given a user is logged in
		| Username            | Password     | Scope        |
		| PennyLane@email.com | S0mePa$$w0rd | Organisation |
	When a POST request is made to create a user for organisation Organisation 1
		| FirstName | LastName   | PhoneNumber | EmailAddress             | OrganisationName |
		| Bob       | Bobkovitch | 0123456789  | bob.bobkovitch@email.com | Organisation 1   |
	Then a response with status code 403 is returned
	And no email is sent

@3540
Scenario: 3. User does not receive email because of service failure
	Given a user is logged in
		| Username             | Password        | Scope        |
		| PostmanPat@email.com | An0therPa$$w0rd | Organisation |
	And the call to the database will fail
	When a POST request is made to create a user for organisation Organisation 1
		| FirstName | LastName   | PhoneNumber | EmailAddress             | OrganisationName |
		| Bob       | Bobkovitch | 0123456789  | bob.bobkovitch@email.com | Organisation 1   |
	Then a response with status code 500 is returned
	And no email is sent
