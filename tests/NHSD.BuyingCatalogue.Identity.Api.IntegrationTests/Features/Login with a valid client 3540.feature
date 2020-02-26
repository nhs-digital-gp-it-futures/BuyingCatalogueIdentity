Feature: Login With Valid Client
	As a Authority User
	I want to login to the system
	So that I can view records

Background: 
    Given There are Users in the database
        | EmailAddress | Password |
        | alice        | Pass123$ |

@3540
Scenario: 1. Logging in with an existing client with valid credentials
	Given the client is using valid client ID and valid secret 
	And the credentials for the client are valid
    When the user navigates to a restricted web page
    Then the user is redirected to the login screen
    When a login request is made
    Then the user is redirected to the restricted web page
    And the response should not contain unauthorised

@3540
Scenario: 2. Logging in with an existing client with invalid username
	Given the client is using valid client ID and valid secret 
    And the credentials for the client have an invalid username
    When the user navigates to a restricted web page
    Then the user is redirected to the login screen
    When a login request is made
    Then the user is redirected to the login screen
    And an error is displayed on the login screen

@3540
Scenario: 3. Logging in with an existing client with invalid password
	Given the client is using valid client ID and valid secret 
    And the credentials for the client have an invalid password
    When the user navigates to a restricted web page
    Then the user is redirected to the login screen
    When a login request is made
    Then a failed response is received with Unauthorized