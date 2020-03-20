Feature: Login With Valid Client
	As a Authority User
	I want to login to the Buying Catalogue
	So that I can view records

Background: 
    Given Organisations exist
        | Name           | OdsCode |
        | Organisation 1 | Ods 1   |
    And Users exist
        | Id  | OrganisationName | FirstName | LastName | Email         | PhoneNumber | Disabled | Password          |
        | 123 | Organisation 1   | John      | Doe      | test@user.com | 01234567890 | false    | testingtesting123 |

@3540
Scenario: 1. Logging in with an existing client with valid credentials
    When the user navigates to a restricted web page
    Then the user is redirected to page account/login
    When a login request is made with email address test@user.com and password testingtesting123
    Then the user is redirected to page home/privacy
    And the page contains element with ID sampleResourceResult with text Authorized With Sample Resource

@3540
Scenario: 2. Logging in with an existing client with invalid username
    When the user navigates to a restricted web page
    Then the user is redirected to page account/login
    When a login request is made with email address NonExistent@email.com and password testingtesting123
    Then the user is redirected to page account/login
    And the page contains a validation summary with text Enter a valid email address and password

@3540
Scenario: 3. Logging in with an existing client with invalid password
    When the user navigates to a restricted web page
    Then the user is redirected to page account/login
    When a login request is made with email address test@user.com and password Invalid
    Then the user is redirected to page account/login
    And the page contains a validation summary with text Enter a valid email address and password

@3540
Scenario: 4. Logging in with an existing client with empty username
    When the user navigates to a restricted web page
    Then the user is redirected to page account/login
    When a login request is made with email address  and password Invalid
    Then the user is redirected to page account/login
    And the page contains an email address error with text Enter your email address

@3540
Scenario: 5. Logging in with an existing client with empty password
    When the user navigates to a restricted web page
    Then the user is redirected to page account/login
    When a login request is made with email address test@user.com and no password
    Then the user is redirected to page account/login
    And the page contains a password error with text Enter your password

@3540
Scenario: 6. Logging in with an invalid email address
    When the user navigates to a restricted web page
    Then the user is redirected to page account/login
    When a login request is made with email address AliceSmith and password Pass123$
    Then the user is redirected to page account/login
    And the page contains an email address error with text Enter a valid email address
