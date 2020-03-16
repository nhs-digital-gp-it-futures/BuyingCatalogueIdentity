Feature: Logout
    As a Registered User
    I want to logout 
    So that I will need to enter my credentials to access the Buying Catalogue

Background:
	Given There are Users in the database
		| EmailAddress         | Password |
		| AliceSmith@email.com | Pass123$ |

@3568
Scenario: 1. User logs out and is unable to access protected resources without logging in first
	Given a user has successfully logged in with email address AliceSmith@email.com and password Pass123$
	When the user clicks on logout button
	Then the user is logged out
	When the user navigates to a restricted web page
	Then the user is redirected to page account/login