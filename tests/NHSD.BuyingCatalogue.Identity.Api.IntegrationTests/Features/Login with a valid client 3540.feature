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
    When the user navigates to the login page with return url http://localhost:8072
    When a login request is made with username alice and password Pass123$
    Then The user is redirected to http://localhost:8072