﻿Feature: Forgot Password
    As a User
    I want to view the forgot password page
    So that I can apply to have my password reset

Background: 
    When the user navigates to a restricted web page
    Then the user is redirected to page identity/account/login
    When the user clicks element with Data ID forgot-password-link
    Then the user is redirected to page identity/account/forgotpassword

@3926
Scenario: Entering a valid email address lets the user proceed
    When element with Data ID input-email-address is populated with test@email.com
    And element with Data ID submit-button is clicked
    Then the user is redirected to page identity/account/forgotpasswordlinksent

@3926
Scenario: Entering an invalid email address gives the user a relevant error message
    When element with Data ID input-email-address is populated with not.an.email.address
    And element with Data ID submit-button is clicked
    Then the user is redirected to page identity/account/forgotpassword
    And the element with Data ID field-email-address has validation error with text Enter an email address in the correct format, like name@example.com

@3926
Scenario: Entering no email address gives the user a relevant error message
    When element with Data ID submit-button is clicked
    Then the user is redirected to page identity/account/forgotpassword
    And the element with Data ID field-email-address has validation error with text Enter an email address
