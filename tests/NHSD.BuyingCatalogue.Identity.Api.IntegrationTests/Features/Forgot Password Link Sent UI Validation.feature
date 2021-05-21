Feature: Forgot Password Link Sent UI
    As a User
    I want to view the forgot password link sent page
    So that I know to check my emails for a reset link

Background:
    When the user navigates to a restricted web page
    Then the user is redirected to page identity/account/login
    When the user clicks element with Data ID forgot-password-link
    Then the user is redirected to page identity/account/forgotpassword
    When element with Data ID input-email-address is populated with test@email.com
    And element with Data ID submit-button is clicked
    Then the user is redirected to page identity/account/forgotpasswordlinksent

@3926
Scenario: The NHS Header is displayed correctly
    Then the page contains element with Data ID header-banner

@3926
Scenario: The Forgot Password Link Sent page title is displayed correctly
    Then the page contains element with Data ID page-title
    And element with Data ID page-title has tag h1
    And element with Data ID page-title has text You've been sent a link

@3926
Scenario: The The Forgot Password Link Sent page description is displayed correctly
    Then the page contains element with Data ID page-description
    And element with Data ID page-description has tag p
    And element with Data ID page-description has text A link to set your password has been sent to the email address you provided.

@3926
Scenario: The back to log in link is displayed correctly
    Then the page contains element with Data ID go-back-link
    And element with Data ID go-back-link contains a link to identity/account/login with text Back to log in

@3926
Scenario: The NHS Footer is displayed
    Then the page contains element with Data ID nhsuk-footer
    And element with Data ID nhsuk-footer contains a link to /guide
    And element with Data ID nhsuk-footer contains a link to /guide#contact-us
    And element with Data ID nhsuk-footer contains a link to https://digital.nhs.uk/
    And element with Data ID nhsuk-footer contains a link to https://digital.nhs.uk/services/future-gp-it-systems-and-services
    And element with Data ID nhsuk-footer contains a link to https://gpitbjss.atlassian.net/wiki/spaces/GPITF/overview

@3926
Scenario: The NHS Legal is displayed
    Then the page contains element with Data ID nhsuk-legal-panel
    And element with Data ID nhsuk-legal-panel contains a link to /privacy-policy
    And element with Data ID nhsuk-legal-panel contains a link to /document/terms-of-use.pdf
