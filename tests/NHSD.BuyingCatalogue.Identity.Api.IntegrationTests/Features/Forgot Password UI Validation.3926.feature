Feature: Forgot Password UI
    As a User
    I want to view the forgot password page
    So that I can apply to have my password reset

Background:
    When the user navigates to a restricted web page
    Then the user is redirected to page identity/account/login
    When the user clicks element with Data ID forgot-password-link
    Then the user is redirected to page identity/account/forgotpassword

@3926
Scenario: 1. The NHS Header is displayed correctly
    Then the page contains element with Data ID header-banner

@3926
Scenario: 2. The back to log in link is displayed correctly
    Then the page contains element with Data ID go-back-link
    And element with Data ID go-back-link contains a link to identity/account/login with text Back to log in

@3926
Scenario: 3. The Forgot Password page title is displayed correctly
    Then the page contains element with Data ID page-title
    And element with Data ID page-title has tag h1
    And element with Data ID page-title has text Enter your email address

@3926
Scenario: 4. The Forgot Password page description is displayed correctly
    Then the page contains element with Data ID page-description
    And element with Data ID page-description has tag h2
    And element with Data ID page-description has text You'll be sent a link to the address you provide so you can choose a password.

@3926
Scenario: 5. The Email Address input element is displayed correctly
    Then the page contains element with Data ID input-email-address
    And element with Data ID input-email-address has tag input
    And element with Data ID input-email-address has label with text Email address

@3926
Scenario: 6. The Submit button is displayed correctly
    Then the page contains element with Data ID submit-button
    And element with Data ID submit-button has tag button
    And element with Data ID submit-button is of type submit
    And element with Data ID submit-button has text Submit

@3926
Scenario: 7. The NHS Footer is displayed
    Then the page contains element with Data ID nhsuk-footer
    And element with Data ID nhsuk-footer contains a link to /guide
    And element with Data ID nhsuk-footer contains a link to /guide#contact-us
    And element with Data ID nhsuk-footer contains a link to https://digital.nhs.uk/
    And element with Data ID nhsuk-footer contains a link to https://digital.nhs.uk/services/future-gp-it-systems-and-services
    And element with Data ID nhsuk-footer contains a link to https://gpitbjss.atlassian.net/wiki/spaces/GPITF/overview

@3926
Scenario: 8. The NHS Legal is displayed
    Then the page contains element with Data ID nhsuk-legal-panel
    And element with Data ID nhsuk-legal-panel contains a link to https://digital.nhs.uk/about-nhs-digital/privacy-and-cookies
    And element with Data ID nhsuk-legal-panel contains a link to https://host.docker.internal/document/terms-of-use.pdf
