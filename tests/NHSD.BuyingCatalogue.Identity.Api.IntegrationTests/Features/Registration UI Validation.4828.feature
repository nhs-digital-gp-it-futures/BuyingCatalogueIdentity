Feature: Registration UI
    As an Buyer User
    I want to see the registration page
    So that I can request an account

    Background: 
    When the user navigates to a restricted web page
    Then the user is redirected to page identity/account/login
    Given the user clicks element with Data ID request-account-link
    Then the user is redirected to page identity/account/registration

@4828
Scenario: 1. The NHS Header is displayed correctly
    Then the page contains element with Data ID header-banner
    And element with Data ID header-banner contains a link to https://digital.nhs.uk/
    And element with Data ID header-banner contains element with Data ID nhs-digital-logo

@4828
Scenario: 2. The back to log in link is displayed correctly
    Then the page contains element with Data ID go-back-link
    And element with Data ID go-back-link contains a link to identity/account/login with text Back to log in

@4828
Scenario: 3. The Registration page title is displayed correctly
    Then the page contains element with Data ID registration-page-title
    And element with Data ID registration-page-title has tag h1
    And element with Data ID registration-page-title has text Request Buying Catalogue account
    
@4828
Scenario: 4. The Registration page description is displayed correctly
    Then the page contains element with Data ID registration-page-description
    And element with Data ID registration-page-description has tag h2
    And element with Data ID registration-page-description has text This is how to register for a user account. You’ll need one to place orders on the Buying Catalogue.
     
@4828
Scenario: 5. The Request an Account button is displayed correctly
    Then the page contains element with Data ID request-account-button
    And element with Data ID request-account-button has tag a
    And element with Data ID request-account-button is of type button
    And element with Data ID request-account-button has text Request an account
    And element with Data ID request-account-button is email link to address buying.catalogue@nhs.net
        
@4828
Scenario: 6. The NHS Footer is displayed
    Then the page contains element with Data ID nhsuk-footer
    And element with Data ID nhsuk-footer contains a link to https://www.nhs.uk/nhs-sites/
    And element with Data ID nhsuk-footer contains a link to https://www.nhs.uk/about-us/
    And element with Data ID nhsuk-footer contains a link to https://www.nhs.uk/contact-us/
    And element with Data ID nhsuk-footer contains a link to https://www.nhs.uk/personalisation/login.aspx
    And element with Data ID nhsuk-footer contains a link to https://www.nhs.uk/about-us/sitemap/
    And element with Data ID nhsuk-footer contains a link to https://www.nhs.uk/accessibility/
    And element with Data ID nhsuk-footer contains a link to https://www.nhs.uk/our-policies/
    And element with Data ID nhsuk-footer contains a link to https://www.nhs.uk/our-policies/cookies-policy/
