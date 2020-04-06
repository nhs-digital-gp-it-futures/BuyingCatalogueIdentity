Feature: Reset Password Link Expired UI
    As an User
    I want to view the reset password link expired page
    So that I can request a new link

Background:
    When the user navigates to identity url account/resetpasswordexpired
    Then the user is redirected to page account/resetpasswordexpired

@3926
Scenario: 1. The NHS Header is displayed correctly
    Then the page contains element with Data ID header-banner
    And element with Data ID header-banner contains a link to https://digital.nhs.uk/
    And element with Data ID header-banner contains element with Data ID nhs-digital-logo

@3926
Scenario: 2. The Reset Password Link Expired page title is displayed correctly
    Then the page contains element with Data ID page-title
    And element with Data ID page-title has tag h1
    And element with Data ID page-title has text Password link expired

@3926
Scenario: 3. The Reset Password Link Expired page description is displayed correctly
    Then the page contains element with Data ID page-description
    And element with Data ID page-description has tag h2
    And element with Data ID page-description has text This link is no longer valid. You must request a new one.

@3926
Scenario: 4. The Reset Password button is displayed correctly
    Then the page contains element with Data ID request-new-link-button
    And element with Data ID request-new-link-button has tag button
    And element with Data ID request-new-link-button has text Request new link

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
