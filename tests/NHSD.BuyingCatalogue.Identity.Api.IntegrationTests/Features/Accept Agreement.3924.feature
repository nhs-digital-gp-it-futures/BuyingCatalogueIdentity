@ignore
Feature: User needs to sign the Accept Agreement page
	As a Authority User
	I want to be able to read & accept the terms and conditions
	So that I can then proceed to the application

Background:
	Given Organisations exist
		| Name           | OdsCode |
		| Organisation 1 | Ods 1   |
	And Users exist
		| Id  | OrganisationName | FirstName | LastName | Email         | PhoneNumber | Disabled | Password          | CatalogueAgreementSigned |
		| 123 | Organisation 1   | John      | Doe      | test@user.com | 01234567890 | false    | testingtesting123 | false                    |

@3924
Scenario: 1. User has not signed the accept agreement, validation is valid
	When the user navigates to a restricted web page
	Then the user is redirected to page identity/account/login
	When a login request is made with email address test@user.com and password testingtesting123
	Then the user is redirected to page identity/Consent
	When element with Data ID agree-terms-checkbox is clicked
	And element with Data ID agreement-submit-button is clicked
	Then the user is redirected to page home/privacy
	And the page contains element with ID sampleResourceResult with text Authorized With Sample Resource

@3924
Scenario: 2. User has not signed the accept agreement, validation is invalid
	When the user navigates to a restricted web page
	Then the user is redirected to page identity/account/login
	When a login request is made with email address test@user.com and password testingtesting123
	Then the user is redirected to page identity/Consent
	When element with Data ID agreement-submit-button is clicked
	Then the user is redirected to page identity/Consent
	And the page contains a validation summary with text Accept End User Agreement at position 0
