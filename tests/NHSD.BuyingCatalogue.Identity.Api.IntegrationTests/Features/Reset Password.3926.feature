Feature: Reset Password
    As a User 
    I want to be able to reset my password
    So that I can login with correct details

Background:
Given Organisations exist
        | Name           | OdsCode |
        | Organisation 1 | Ods 1   |
    And Users exist
        | Id  | OrganisationName | FirstName | LastName | Email          | PhoneNumber | Disabled | Password          |
        | 123 | Organisation 1   | John      | Doe      | test@user.com  | 01234567890 | false    | testingtesting123 |
        | 234 | Organisation 1   | Other     | Doe      | other@user.com | 01234567890 | false    | testingtesting123 |
    When the user navigates to a restricted web page
    Then the user is redirected to page identity/account/login
    When the user clicks element with Data ID forgot-password-link
    Then the user is redirected to page identity/account/forgotpassword
    When element with Data ID input-email-address is populated with test@user.com
    And element with Data ID submit-button is clicked
    Then the user is redirected to page identity/account/forgotpasswordlinksent
    And only one email is sent
    And the email sent contains the following information
        | From                           | To            | Subject                                          | ResetPasswordLink              |
        | noreply@buyingcatalogue.nhs.uk | test@user.com | INTEGRATION_TEST Buying Catalogue password reset | identity/account/resetpassword |

@3926
Scenario: 1. Entering a valid password, allows the user to proceed
    When The email link is clicked
    Then the user is redirected to page identity/account/resetpassword
    When element with Data ID input-reset-password is populated with Password123!
    And element with Data ID input-confirm-reset-password is populated with Password123!
    And element with Data ID reset-password-button is clicked
    Then the user is redirected to page account/resetpasswordconfirmation

@3926
Scenario: 2. User cannot use the same link to set their password twice
    When The email link is clicked
    Then the user is redirected to page identity/account/resetpassword
    When element with Data ID input-reset-password is populated with Password123!
    And element with Data ID input-confirm-reset-password is populated with Password123!
    And element with Data ID reset-password-button is clicked
    Then the user is redirected to page account/resetpasswordconfirmation
    When The email link is clicked
    Then the user is redirected to page account/resetpasswordexpired

@3926
Scenario: 3. User cannot modify the link to set someone elses password
    When The email link is modified to use email address other@user.com
    When The email link is clicked
    Then the user is redirected to page account/resetpasswordexpired

@3926
Scenario: 4. Not adhering to the password policy but entering matching passwords, but gives the user a relevant error message
    When The email link is clicked
    Then the user is redirected to page identity/account/resetpassword
    When element with Data ID input-reset-password is populated with Pass1
    And element with Data ID input-confirm-reset-password is populated with Pass1
    And element with Data ID reset-password-button is clicked
    Then the user is redirected to page account/resetpassword
    And the element with Data ID field-reset-password has validation error with text The password you’ve entered does not meet the criteria

@3926
Scenario: 5. Adhering to the password policy but entering mismatched passwords gives the user a relevant error message
    When The email link is clicked
    Then the user is redirected to page identity/account/resetpassword
    When element with Data ID input-reset-password is populated with Password123!
    And element with Data ID input-confirm-reset-password is populated with Password124!
    And element with Data ID reset-password-button is clicked
    Then the user is redirected to page account/resetpassword
    And the element with Data ID field-confirm-reset-password has validation error with text Passwords do not match

@3926
Scenario: 6. Not adhering to the password policy & entering mismatched passwords gives the user a relevant error message
    When The email link is clicked
    Then the user is redirected to page identity/account/resetpassword
    When element with Data ID input-reset-password is populated with Pass1
    And element with Data ID input-confirm-reset-password is populated with Pass2
    And element with Data ID reset-password-button is clicked
    Then the user is redirected to page account/resetpassword
    And the element with Data ID field-confirm-reset-password has validation error with text Passwords do not match

@3926
Scenario: 7. Pressing submit without entering a password gives the user a relevant error message
    When The email link is clicked
    Then the user is redirected to page identity/account/resetpassword
    When element with Data ID reset-password-button is clicked
    Then the user is redirected to page account/resetpassword
    And the element with Data ID field-reset-password has validation error with text Enter a password
