@ignore
Feature: Accept Agreement page UI
    As a User
    I want to be able to view the Accept Agreement page
    So that I can read and accept the terms and conditions

Background:
    Given Organisations exist
        | Name           | OdsCode |
        | Organisation 1 | Ods 1   |
    And Users exist
        | Id  | OrganisationName | FirstName | LastName | Email         | PhoneNumber | Disabled | Password          | CatalogueAgreementSigned |
        | 123 | Organisation 1   | John      | Doe      | test@user.com | 01234567890 | false    | testingtesting123 | false                    |
    When the user navigates to a restricted web page
    Then the user is redirected to page identity/account/login
    When a login request is made with email address test@user.com and password testingtesting123
    Then the user is redirected to page identity/Consent

@3924
Scenario: 1. The NHS Header is displayed correctly
    Then the page contains element with Data ID header-banner

@3924
Scenario: 2. The back to home page element links to the public browse homepage
    Then the go back element should link to the public browse homepage

@3924
Scenario: 3. The back to log in link shows the correct text
    Then the page contains element with Data ID go-back-link
    And element with Data ID go-back-link has text Back to homepage

@3924
Scenario: 4. The Accept Agreement page title is displayed correctly
    Then the page contains element with Data ID page-title
    And element with Data ID page-title has tag h1
    And element with Data ID page-title has text Buying Catalogue End User Agreement

@3924
Scenario: 5. The Accept Agreement page description is displayed correctly
    Then the page contains element with Data ID page-description
    And element with Data ID page-description has tag h2
    And element with Data ID page-description has text Please read this agreement carefully before using the NHS Digital Buying Catalogue

@3924
Scenario: 6. The Checkbox element is displayed correctly
    Then the page contains element with Data ID agree-terms-checkbox
    And element with Data ID agree-terms-checkbox has tag input
    And element with Data ID agree-terms-checkbox is of type checkbox
    And element with Data ID agree-terms-checkbox has label with text Accept End User Agreement

@3924
Scenario: 7. The Submit button is displayed correctly
    Then the page contains element with Data ID agreement-submit-button
    And element with Data ID agreement-submit-button has tag button
    And element with Data ID agreement-submit-button is of type submit
    And element with Data ID agreement-submit-button has text Continue

@3924
Scenario: 8. The NHS Footer is displayed
    Then the page contains element with Data ID nhsuk-footer
    And element with Data ID nhsuk-footer contains a link to /guide
    And element with Data ID nhsuk-footer contains a link to /guide#contact-us
    And element with Data ID nhsuk-footer contains a link to https://digital.nhs.uk/
    And element with Data ID nhsuk-footer contains a link to https://digital.nhs.uk/services/future-gp-it-systems-and-services
    And element with Data ID nhsuk-footer contains a link to https://gpitbjss.atlassian.net/wiki/spaces/GPITF/overview

@3924
Scenario: 9. The NHS Legal is displayed
    Then the page contains element with Data ID nhsuk-legal-panel
    And element with Data ID nhsuk-legal-panel contains a link to https://digital.nhs.uk/about-nhs-digital/privacy-and-cookies
    And element with Data ID nhsuk-legal-panel contains a link to https://host.docker.internal/document/terms-of-use.pdf
