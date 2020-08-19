Feature: Login UI
    As an Authority User
    I want to see the login page
    So that I can log into my application

Background:
    When the user navigates to a restricted web page
    Then the user is redirected to page identity/account/login

@3540
Scenario: 1. The NHS Header is displayed correctly
    Then the page contains element with Data ID header-banner

@3540
Scenario: 2. The Login page title is displayed correctly
    Then the page contains element with Data ID page-title
    And element with Data ID page-title has tag h1
    And element with Data ID page-title has text Buying Catalogue log in

@3540
Scenario: 3. The Login page description is displayed correctly
    Then the page contains element with Data ID page-description
    And element with Data ID page-description has tag h2
    And element with Data ID page-description has text Enter your details to place a new order or complete an order you’ve already started.

@3540
Scenario: 4. The Email Address input element is displayed correctly
    Then the page contains element with Data ID input-email-address
    Then the page contains element with Data ID label-email-address
    And element with Data ID input-email-address has tag input
    And element with Data ID label-email-address has text Email address

@3540
Scenario: 5. The Password input element is displayed correctly
    Then the page contains element with Data ID input-password
    Then the page contains element with Data ID label-password
    And element with Data ID input-password has tag input
    And element with Data ID input-password is of type password
    And element with Data ID label-password has text Password

@3540
Scenario: 6. The Log in button is displayed correctly
    Then the page contains element with Data ID login-submit-button
    And element with Data ID login-submit-button has tag button
    And element with Data ID login-submit-button is of type submit
    And element with Data ID login-submit-button has text Log in

@3540
Scenario: 7. The Forgot Password link is displayed correctly
    Then the page contains element with Data ID forgot-password-link
    And element with Data ID forgot-password-link has text Forgot password?

@3540
Scenario: 8. The Request Account link is displayed correctly
    Then the page contains element with Data ID request-account-link
    And element with Data ID request-account-link has text Don't have an account? Request one now
    And element with Data ID request-account-link is a link to Account/Registration

@3540
Scenario: 9. The NHS Footer is displayed
    Then the page contains element with Data ID nhsuk-footer
    And element with Data ID nhsuk-footer contains a link to /guide
    And element with Data ID nhsuk-footer contains a link to /guide#contact-us
    And element with Data ID nhsuk-footer contains a link to https://digital.nhs.uk/
    And element with Data ID nhsuk-footer contains a link to https://digital.nhs.uk/services/future-gp-it-systems-and-services
    And element with Data ID nhsuk-footer contains a link to https://gpitbjss.atlassian.net/wiki/spaces/GPITF/overview

@3540
Scenario: 10. The NHS Legal is displayed
    Then the page contains element with Data ID nhsuk-legal-panel
    And element with Data ID nhsuk-legal-panel contains a link to https://digital.nhs.uk/about-nhs-digital/privacy-and-cookies
    And element with Data ID nhsuk-legal-panel contains a link to /document/terms-of-use.pdf
