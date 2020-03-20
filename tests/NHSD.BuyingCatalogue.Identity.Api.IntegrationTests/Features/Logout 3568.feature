Feature: Logout
    As a Registered User
    I want to logout 
    So that I will need to enter my credentials to access the Buying Catalogue

Background:
    Given Organisations exist
        | Name           | OdsCode |
        | Organisation 1 | Ods 1   |
    And Users exist
        | Id  | OrganisationName | FirstName | LastName | Email   | PhoneNumber | Disabled | Password |
        | 123 | Organisation 1   | John      | Doe      | a@b.com | 01234567890 | false    | yolo     |

@3568
Scenario: 1. User logs out and is unable to access protected resources without logging in first
    Given a user has successfully logged in with email address a@b.com and password yolo
    When the user clicks on logout button
    Then the user is logged out
    When the user navigates to a restricted web page
    Then the user is redirected to page account/login
