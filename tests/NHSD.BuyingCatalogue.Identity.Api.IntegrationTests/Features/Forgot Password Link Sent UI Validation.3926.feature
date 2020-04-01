Feature: Forgot Password Link Sent UI
    As a User
    I want to view the forgot password link sent page
    So that I know to check my emails for a reset link

    Background: 
    When the user navigates to a restricted web page
    Then the user is redirected to page identity/account/login
    When the user clicks on the forgot password button
    Then the user is redirected to page identity/account/forgotpassword
    When element with Data ID input-email-address is populated with test@email.com
    And element with Data ID submit-button is clicked
    Then the user is redirected to page identity/account/forgotpasswordlinksent

@3926
Scenario: 1. The NHS Header is displayed correctly
    Then the page contains element with Data ID header-banner
    And element with Data ID header-banner contains a link to https://digital.nhs.uk/
    And element with Data ID header-banner contains element with Data ID nhs-digital-logo

@3926
Scenario: 2. The Forgot Password Link Sent page title is displayed correctly
    Then the page contains element with Data ID page-title
    And element with Data ID page-title has tag h1
    And element with Data ID page-title has text You've been sent a link

@3926
Scenario: 3. The The Forgot Password Link Sent page description is displayed correctly
    Then the page contains element with Data ID page-description
    And element with Data ID page-description has tag h2
    And element with Data ID page-description has text A link to set your password has been sent to the email address you provided.
    
@3926
Scenario: 4. The back to log in link is displayed correctly
    Then the page contains element with Data ID go-back-link
    And element with Data ID go-back-link contains a link to identity/account/login with text < Back to log in

@3926
Scenario: 5. The NHS Footer is displayed
    Then the page contains element with Data ID nhsuk-footer
    And element with Data ID nhsuk-footer contains a link to https://www.nhs.uk/nhs-sites/
    And element with Data ID nhsuk-footer contains a link to https://www.nhs.uk/about-us/
    And element with Data ID nhsuk-footer contains a link to https://www.nhs.uk/contact-us/
    And element with Data ID nhsuk-footer contains a link to https://www.nhs.uk/personalisation/login.aspx
    And element with Data ID nhsuk-footer contains a link to https://www.nhs.uk/about-us/sitemap/
    And element with Data ID nhsuk-footer contains a link to https://www.nhs.uk/accessibility/
    And element with Data ID nhsuk-footer contains a link to https://www.nhs.uk/our-policies/
    And element with Data ID nhsuk-footer contains a link to https://www.nhs.uk/our-policies/cookies-policy/
