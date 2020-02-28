Feature: Login With Valid Client
	As a Authority User
	I want to login to the Buying Catalogue
	So that I can view records

Background: 
    Given There are Users in the database
        | EmailAddress | Password |
        | alice        | Pass123$ |

@3540
Scenario: 1. Logging in with an existing client with valid credentials
    When the user navigates to a restricted web page
    Then the user is redirected to identity server page account/login
    When a login request is made with username alice and password Pass123$
    Then the user is redirected to client page home/privacy
    And the page contains paragraph with text Authorized With Sample Resource

@3540
Scenario: 2. Logging in with an existing client with invalid username
    When the user navigates to a restricted web page
    Then the user is redirected to identity server page account/login
    When a login request is made with username Invalid and password Pass123$
    Then the user is redirected to identity server page account/login

@3540
Scenario: 3. Logging in with an existing client with invalid username
    When the user navigates to a restricted web page
    Then the user is redirected to identity server page account/login
    When a login request is made with username alice and password Invalid
    Then the user is redirected to identity server page account/login