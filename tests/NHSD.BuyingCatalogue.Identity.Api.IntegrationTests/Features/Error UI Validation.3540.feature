Feature: Error UI Validation
    As an Authority User
    I want to see the error page
    So that I know when something has gone wrong

    Background: 
    Given Organisations exist
        | Name           | OdsCode |
        | Organisation 1 | Ods 1   |
    And Users exist
        | Id  | OrganisationName | FirstName | LastName | Email         | PhoneNumber | Disabled | Password          |
        | 123 | Organisation 1   | John      | Doe      | test@user.com | 01234567890 | false    | testingtesting123 |
    When the user navigates to a restricted web page
    Then the user is redirected to page identity/account/login
    When the redirect URL is modified to be invalid
    Then the user is redirected to page identity/account/login
    When a login request is made with email address test@user.com and password testingtesting123
    Then the user is redirected to page identity/error
    
@3540
Scenario: 1. The NHS Header is displayed
    Then the page contains element with Data ID header-banner
    And element with Data ID header-banner contains a link to https://digital.nhs.uk/
    And element with Data ID header-banner contains element with Data ID nhs-digital-logo
    
@3540
Scenario: 2. The Title of the page is displayed
    Then the page contains element with Data ID error-page-title
    And element with Data ID error-page-title has tag h1
    And element with Data ID error-page-title has text Error
    
@3540
Scenario: 3. The Description of the page is displayed
    Then the page contains element with Data ID error-page-description
    And element with Data ID error-page-description has tag h2
    And element with Data ID error-page-description has text An error occurred while processing your request.
    
@3540
Scenario: 4. The NHS Footer is displayed
    Then the page contains element with Data ID nhsuk-footer
    And element with Data ID nhsuk-footer contains a link to https://www.nhs.uk/nhs-sites/
    And element with Data ID nhsuk-footer contains a link to https://www.nhs.uk/about-us/
    And element with Data ID nhsuk-footer contains a link to https://www.nhs.uk/contact-us/
    And element with Data ID nhsuk-footer contains a link to https://www.nhs.uk/personalisation/login.aspx
    And element with Data ID nhsuk-footer contains a link to https://www.nhs.uk/about-us/sitemap/
    And element with Data ID nhsuk-footer contains a link to https://www.nhs.uk/accessibility/
    And element with Data ID nhsuk-footer contains a link to https://www.nhs.uk/our-policies/
    And element with Data ID nhsuk-footer contains a link to https://www.nhs.uk/our-policies/cookies-policy/
