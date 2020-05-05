Feature: Login With Valid Client
	As a Authority User
	I want to login to the Buying Catalogue
	So that I can view records

Background: 
    Given Organisations exist
        | Name           | OdsCode |
        | Organisation 1 | Ods 1   |
    And Users exist
        | Id  | OrganisationName | FirstName | LastName | Email          | PhoneNumber | Disabled | Password          |
        | 123 | Organisation 1   | John      | Doe      | test@user.com  | 01234567890 | false    | testingtesting123 |
        | 234 | Organisation 1   | Jane      | Doe      | test2@user.com | 01234567890 | true     | testingtesting321 |

@3540
Scenario: 1. Logging in with an existing client with valid credentials
    When the user navigates to a restricted web page
    Then the user is redirected to page identity/account/login
    When a login request is made with email address test@user.com and password testingtesting123
    Then the user is redirected to page home/privacy
    And the page contains element with ID sampleResourceResult with text Authorized With Sample Resource

@3540
Scenario: 2. Logging in with an existing client with invalid username
    When the user navigates to a restricted web page
    Then the user is redirected to page identity/account/login
    When a login request is made with email address NonExistent@email.com and password testingtesting123
    Then the user is redirected to page identity/account/login
    And the page contains a validation summary with text Enter a valid email address and password at position 0
    And the page contains a validation summary with text Enter a valid email address and password at position 1

@3540
Scenario: 3. Logging in with an existing client with invalid password
    When the user navigates to a restricted web page
    Then the user is redirected to page identity/account/login
    When a login request is made with email address test@user.com and password Invalid
    Then the user is redirected to page identity/account/login
    And the page contains a validation summary with text Enter a valid email address and password at position 0
    And the page contains a validation summary with text Enter a valid email address and password at position 1
    And element with Data ID input-password has an empty value

@3540
Scenario: 4. Logging in with an existing client with empty username
    When the user navigates to a restricted web page
    Then the user is redirected to page identity/account/login
    When a login request is made with email address  and password Invalid
    Then the user is redirected to page identity/account/login
    And the element with Data ID field-email-address has validation error with text Enter your email address

@3540
Scenario: 5. Logging in with an existing client with empty password
    When the user navigates to a restricted web page
    Then the user is redirected to page identity/account/login
    When a login request is made with email address test@user.com and no password
    Then the user is redirected to page identity/account/login
    And the element with Data ID field-password has validation error with text Enter your password

@3540
Scenario: 6. Logging in with an invalid email address
    When the user navigates to a restricted web page
    Then the user is redirected to page identity/account/login
    When a login request is made with email address AliceSmith and password Pass123$
    Then the user is redirected to page identity/account/login
    And the element with Data ID field-email-address has validation error with text Enter a valid email address

@3543
Scenario: 7. Logging in with a disabled user account
    When the user navigates to a restricted web page
    Then the user is redirected to page identity/account/login
    When a login request is made with email address test2@user.com and password testingtesting321
    Then the user is redirected to page identity/account/login
    And the page contains a validation summary with text There is a problem accessing your account.\n\nContact the account administrator at: exeter.helpdesk@nhs.net or call 0300 303 4034 at position 0

@3540 
Scenario: 8. Navigating to the forgot password page from login, then pressing the back button, preserves the return url
    When the user navigates to a restricted web page
    Then the user is redirected to page identity/account/login
    When the user clicks element with Data ID forgot-password-link
    Then the user is redirected to page identity/account/forgotpassword
    When the user clicks element with Data ID go-back-link
    Then the user is redirected to page identity/account/login
    When a login request is made with email address test@user.com and password testingtesting123
    Then the user is redirected to page home/privacy
    And the page contains element with ID sampleResourceResult with text Authorized With Sample Resource
    
@3540 
Scenario: 9. Navigating to the registration page from login, then pressing the back button, preserves the return url
    When the user navigates to a restricted web page
    Then the user is redirected to page identity/account/login
    When the user clicks element with Data ID request-account-link
    Then the user is redirected to page identity/account/registration
    When the user clicks element with Data ID go-back-link
    Then the user is redirected to page identity/account/login
    When a login request is made with email address test@user.com and password testingtesting123
    Then the user is redirected to page home/privacy
    And the page contains element with ID sampleResourceResult with text Authorized With Sample Resource

@3926
Scenario: 10. Navigating directly to the login page, should return the user to the public browse page
    Given the user navigates directly to the login page
    When a login request is made with email address test@user.com and password testingtesting123
    Then the user is redirected to page /back-from-admin