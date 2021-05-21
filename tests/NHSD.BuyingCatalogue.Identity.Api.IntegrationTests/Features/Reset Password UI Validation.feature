Feature: Reset Password UI
    As an User
    I want to view the reset password page
    So that I can request an account

Background:
    Given Organisations exist
        | Name           | OdsCode |
        | Organisation 1 | Ods 1   |
    And Users exist
        | Id  | OrganisationName | FirstName | LastName | Email          | PhoneNumber | Disabled | Password          |
        | 123 | Organisation 1   | John      | Doe      | test@user.com  | 01234567890 | false    | testingtesting123 |
    When the user with ID 123 has a valid password reset token
    And the user navigates to identity url account/resetpassword with a password reset token
    Then the user is redirected to page account/resetpassword

@3926
Scenario: The NHS Header is displayed correctly
	Then the page contains element with Data ID header-banner

@3926
Scenario: The Reset Password page title is displayed correctly
    Then the page contains element with Data ID page-title
    And element with Data ID page-title has tag h1
    And element with Data ID page-title has text Choose your password

@3926
Scenario: The Reset Password page description is displayed correctly
    Then the page contains element with Data ID page-description
    And element with Data ID page-description has tag p
    And element with Data ID page-description has text Please read the password policy before proceeding.

@3926
Scenario: The Password input element is displayed correctly
    Then the page contains element with Data ID input-reset-password
    And element with Data ID input-reset-password has tag input
    And element with Data ID input-reset-password is of type password
    And element with Data ID input-reset-password has label with text Enter a password

@3926
Scenario: The confirm Reset Password input element is displayed correctly
    Then the page contains element with Data ID input-confirm-reset-password
    And element with Data ID input-confirm-reset-password has tag input
    And element with Data ID input-confirm-reset-password is of type password
    And element with Data ID input-confirm-reset-password has label with text Confirm password

@3926
Scenario: The Reset Password button is displayed correctly
    Then the page contains element with Data ID reset-password-button
    And element with Data ID reset-password-button has tag button
    And element with Data ID reset-password-button has text Save password

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
