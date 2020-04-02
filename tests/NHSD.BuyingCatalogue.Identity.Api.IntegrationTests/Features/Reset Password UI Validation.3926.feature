Feature: Reset Password UI
    As an User
    I want to view the reset password page
    So that I can request an account

Background:
    When the user navigates to identity url account/resetpassword
	Then the user is redirected to page account/resetpassword

@3926
Scenario: 1. The NHS Header is displayed correctly
	Then the page contains element with Data ID header-banner
	And element with Data ID header-banner contains a link to https://digital.nhs.uk/
	And element with Data ID header-banner contains element with Data ID nhs-digital-logo

@3926
Scenario: 2. The Reset Password page title is displayed correctly
	Then the page contains element with Data ID page-title
	And element with Data ID page-title has tag h1
	And element with Data ID page-title has text Choose your password

@3926
Scenario: 3. The Reset Password page description is displayed correctly
	Then the page contains element with Data ID page-description
	And element with Data ID page-description has tag h2
	And element with Data ID page-description has text Please read the password policy before proceeding.

@3926
Scenario: 4. The Password input element is displayed correctly
	Then the page contains element with Data ID input-reset-password
	And element with Data ID input-reset-password has tag input
	And element with Data ID input-reset-password is of type password
	And element with Data ID input-reset-password has label with text Enter a password

@3926
Scenario: 5. The confirm Reset Password input element is displayed correctly
	Then the page contains element with Data ID input-confirm-reset-password
	And element with Data ID input-confirm-reset-password has tag input
	And element with Data ID input-confirm-reset-password is of type password
	And element with Data ID input-confirm-reset-password has label with text Confirm password

@3926
Scenario: 6. The Reset Password button is displayed correctly
	Then the page contains element with Data ID reset-password-button
	And element with Data ID reset-password-button has tag button
	And element with Data ID reset-password-button has text Save password

@3926
Scenario: 7. The NHS Footer is displayed
	Then the page contains element with Data ID nhsuk-footer
	And element with Data ID nhsuk-footer contains a link to https://www.nhs.uk/nhs-sites/
	And element with Data ID nhsuk-footer contains a link to https://www.nhs.uk/about-us/
	And element with Data ID nhsuk-footer contains a link to https://www.nhs.uk/contact-us/
	And element with Data ID nhsuk-footer contains a link to https://www.nhs.uk/personalisation/login.aspx
	And element with Data ID nhsuk-footer contains a link to https://www.nhs.uk/about-us/sitemap/
	And element with Data ID nhsuk-footer contains a link to https://www.nhs.uk/accessibility/
	And element with Data ID nhsuk-footer contains a link to https://www.nhs.uk/our-policies/
	And element with Data ID nhsuk-footer contains a link to https://www.nhs.uk/our-policies/cookies-policy/
