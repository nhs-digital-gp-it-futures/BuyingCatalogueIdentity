Feature: Reset Password
    As a User 
    I want to be able to reset my password
    So that I can login with correct details

Background:
    When the user navigates to identity url account/resetpassword
    Then the user is redirected to page account/resetpassword
@ignore
@3926
Scenario: 1. Entering a valid password, allows the user to proceed
    When element with Data ID input-reset-password is populated with Password123!
    And element with Data ID input-confirm-reset-password is populated with Password123!
    And element with Data ID reset-password-button is clicked
    Then the user is redirected to page account/resetpasswordconfirmation
@ignore
@3926
Scenario: 2. Not adhering to the password policy but entering matching passwords, but gives the user a relevant error message
    When element with Data ID input-reset-password is populated with Pass1
    And element with Data ID input-confirm-reset-password is populated with Pass1
    And element with Data ID reset-password-button is clicked
    Then the user is redirected to page account/resetpassword
    And the element with Data ID field-reset-password has validation error with text The password you’ve entered does not meet the criteria

@3926
Scenario: 3. Adhering to the password policy but entering mismatched passwords gives the user a relevant error message
    When element with Data ID input-reset-password is populated with Password123!
    And element with Data ID input-confirm-reset-password is populated with Password124!
    And element with Data ID reset-password-button is clicked
    Then the user is redirected to page account/resetpassword
    And the element with Data ID field-confirm-reset-password has validation error with text Passwords do not match
@ignore
@3926
Scenario: 4. Not adhering to the password policy & entering mismatched passwords gives the user a relevant error message
    When element with Data ID input-reset-password is populated with Pass1
    And element with Data ID input-confirm-reset-password is populated with Pass2
    And element with Data ID reset-password-button is clicked
    Then the user is redirected to page account/resetpassword
    And the element with Data ID field-reset-password has validation error with text The password you’ve entered does not meet the criteria
    And the element with Data ID field-confirm-reset-password has validation error with text Passwords do not match

@3926
Scenario: 5. Pressing submit without entering a password gives the user a relevant error message
    When element with Data ID reset-password-button is clicked
    Then the user is redirected to page account/resetpassword
    And the element with Data ID field-reset-password has validation error with text Enter a password
