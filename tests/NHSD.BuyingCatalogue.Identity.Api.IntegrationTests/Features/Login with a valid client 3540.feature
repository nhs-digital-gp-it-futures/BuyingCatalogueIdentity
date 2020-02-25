Feature: Login With Valid Client
	As a Authority User
	I want to login to the system
	So that I can view records

Background: 
    Given There are Users in the database
        | EmailAddress | Password |
        | michael      | michael  |

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
Scenario: 2. Logging in with an existing client with invalid redirect url
	Given the client is using valid client ID and valid secret 
    And the credentials for the client are valid
    When a login request is made with redirect URL moose.loose.aboot.the.hoose
    #Then something happens, I don't really know what

@3540
Scenario: 3. Logging in with an existing client with invalid username
	Given the client is using valid client ID and valid secret 
    And the credentials for the client have an invalid username
	When a login request is made with redirect URL http://callum.is.great.com
	#Then A failed response is received with Unauthorized

@3540
Scenario: 4. Logging in with an existing client with invalid password
	Given the client is using valid client ID and valid secret 
    And the credentials for the client have an invalid password
	When a login request is made with redirect URL http://callum.is.great.com
	#Then A failed response is received with Unauthorized

@3549
Scenario: 5. Logging in with an existing client with invalid password