﻿Feature: Registration UI
    As an Buyer User
    I want to see the registration page
    So that I can request an account

    Background: 
    When the user navigates to a restricted web page
    Then the user is redirected to page identity/account/login
    When the user clicks element with Data ID request-account-link
    Then the user is redirected to page identity/account/registration

@4828
Scenario: The NHS Header is displayed correctly
    Then the page contains element with Data ID header-banner

@4828
Scenario: The back to log in link is displayed correctly
    Then the page contains element with Data ID go-back-link
    And element with Data ID go-back-link contains a link to identity/account/login with text Back to log in

@4828
Scenario: The Registration page title is displayed correctly
    Then the page contains element with Data ID registration-page-title
    And element with Data ID registration-page-title has tag h1
    And element with Data ID registration-page-title has text Request Buying Catalogue account
    
@4828
Scenario: The Registration page description is displayed correctly
    Then the page contains element with Data ID registration-page-description
    And element with Data ID registration-page-description has tag p
    And element with Data ID registration-page-description has text This is how to register for a user account. You’ll need one to place orders on the Buying Catalogue.
     
@4828
Scenario: The Request an Account button is displayed correctly
    Then the page contains element with Data ID request-account-button
    And element with Data ID request-account-button has tag a
    And element with Data ID request-account-button is of type button
    And element with Data ID request-account-button has text Request an account
    And element with Data ID request-account-button is email link to address buying.catalogue@nhs.net
        
@4828
Scenario: The NHS Footer is displayed
    Then the page contains element with Data ID nhsuk-footer
    And element with Data ID nhsuk-footer contains a link to /guide
    And element with Data ID nhsuk-footer contains a link to /guide#contact-us
    And element with Data ID nhsuk-footer contains a link to https://digital.nhs.uk/
    And element with Data ID nhsuk-footer contains a link to https://digital.nhs.uk/services/future-gp-it-systems-and-services
    And element with Data ID nhsuk-footer contains a link to https://gpitbjss.atlassian.net/wiki/spaces/GPITF/overview

@4828
Scenario: The NHS Legal is displayed
    Then the page contains element with Data ID nhsuk-legal-panel
    And element with Data ID nhsuk-legal-panel contains a link to /privacy-policy
    And element with Data ID nhsuk-legal-panel contains a link to /document/terms-of-use.pdf
