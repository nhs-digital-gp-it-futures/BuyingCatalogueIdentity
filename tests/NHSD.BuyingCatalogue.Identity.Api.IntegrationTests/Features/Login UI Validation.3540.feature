Feature: Login UI
    As an Authority User
    I want to see the login page
    So that I can log into my application

    Background: 
    When the user navigates to a restricted web page
    Then the user is redirected to page account/login

@3540
Scenario: 1. The NHS Header is displayed correctly
    Then the page contains element with Data ID header-banner
    And element with Data ID header-banner contains a link to https://digital.nhs.uk/
    And element with Data ID header-banner contains element with Data ID nhs-digital-logo
    
@3540
Scenario: 2. The Login page title is displayed correctly
    Then the page contains element with Data ID login-page-title
    And element with Data ID login-page-title has tag h1
    And element with Data ID login-page-title has text Buying Catalogue log in
    
@3540
Scenario: 3. The Login page description is displayed correctly
    Then the page contains element with Data ID login-page-description
    And element with Data ID login-page-description has tag h2
    And element with Data ID login-page-description has text Enter your details to access the full functionality of this website.
    
@3540
Scenario: 4. The Email Address input element is displayed correctly
    Then the page contains element with Data ID login-email-address
    And element with Data ID login-email-address has tag input
    And element with Data ID login-email-address is of type email
    And element with Data ID login-email-address has label with text Email address
    
@3540
Scenario: 5. The Password input element is displayed correctly
    Then the page contains element with Data ID login-password
    And element with Data ID login-password has tag input
    And element with Data ID login-password is of type password
    And element with Data ID login-password has label with text Password
    
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
        
@3540
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
