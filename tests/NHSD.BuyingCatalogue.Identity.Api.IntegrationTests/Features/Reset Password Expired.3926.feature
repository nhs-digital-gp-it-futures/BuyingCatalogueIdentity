Feature: Reset Password Expired
    As a User
    I want to view the reset password expired page
    So that I can request a new reset password link

Background: 
    Given Organisations exist
        | Name           | OdsCode |
        | Organisation 1 | Ods 1   |
    And Users exist
        | Id  | OrganisationName | FirstName | LastName | Email          | PhoneNumber | Disabled | Password          |
        | 123 | Organisation 1   | John      | Doe      | test@user.com  | 01234567890 | false    | testingtesting123 |
    When the user with ID 123 has an expired password reset token
    And the user navigates to identity url account/resetpassword with a password reset token
    Then the user is redirected to page account/resetpasswordexpired

@3926
Scenario: 1. Clicking the request new link button sends the user to the forgot password page
    When element with Data ID request-new-link-button is clicked
    Then the user is redirected to page identity/account/forgotpassword
