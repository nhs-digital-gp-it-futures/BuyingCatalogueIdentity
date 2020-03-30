Feature: Forgot Password UI
    As a User
    I want to see the forgot password page
    So that I can apply to have my password reset

Background: 
    When the user navigates to a restricted web page
    Then the user is redirected to page account/login
    When the user clicks on the forgot password button
    Then the user is redirected to page account/forgotpassword

@3926
Scenario: 1. The NHS Header is displayed correctly
    Then the page contains element with Data ID header-banner
    And element with Data ID header-banner contains a link to https://digital.nhs.uk/
    And element with Data ID header-banner contains element with Data ID nhs-digital-logo
    
@3926
Scenario: 2. The Forgot Password page title is displayed correctly
    Then the page contains element with Data ID page-title
    And element with Data ID page-title has tag h1
    And element with Data ID page-title has text Enter your email address
    
@3926
Scenario: 3. The Login page description is displayed correctly
    Then the page contains element with Data ID page-description
    And element with Data ID page-description has tag h2
    And element with Data ID page-description has text You'll be sent a link to the address you provide so you can choose a password.
    
@3926
Scenario: 4. The Email Address input element is displayed correctly
    Then the page contains element with Data ID EmailAddress-input
    And element with Data ID EmailAddress-input has tag input
    And element with Data ID EmailAddress-input has label with text Email address
    
@3926
Scenario: 6. The Log in button is displayed correctly
    Then the page contains element with Data ID submit
    And element with Data ID submit has tag button
    And element with Data ID submit is of type submit
    And element with Data ID submit has text Submit
     
@3926
Scenario: 9. The NHS Footer is displayed
    Then the page contains element with Data ID nhsuk-footer
    And element with Data ID nhsuk-footer contains a link to https://www.nhs.uk/nhs-sites/
    And element with Data ID nhsuk-footer contains a link to https://www.nhs.uk/about-us/
    And element with Data ID nhsuk-footer contains a link to https://www.nhs.uk/contact-us/
    And element with Data ID nhsuk-footer contains a link to https://www.nhs.uk/personalisation/login.aspx
    And element with Data ID nhsuk-footer contains a link to https://www.nhs.uk/about-us/sitemap/
    And element with Data ID nhsuk-footer contains a link to https://www.nhs.uk/accessibility/
    And element with Data ID nhsuk-footer contains a link to https://www.nhs.uk/our-policies/
    And element with Data ID nhsuk-footer contains a link to https://www.nhs.uk/our-policies/cookies-policy/
