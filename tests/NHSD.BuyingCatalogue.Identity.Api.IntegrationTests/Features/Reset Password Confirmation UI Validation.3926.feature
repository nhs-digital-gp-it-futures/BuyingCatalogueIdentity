﻿Feature: Reset Password Confirmation UI
    As an User
    I want to view the reset password confirmation page
    So that I can request an account

Background:
	When the user navigates to identity url account/resetpasswordconfirmation
	Then the user is redirected to page account/resetpasswordconfirmation

@3926
Scenario: 1. The NHS Header is displayed correctly
	Then the page contains element with Data ID header-banner
	And element with Data ID header-banner contains a link to /
	And element with Data ID header-banner contains element with Data ID nhs-digital-logo

@3926
Scenario: 2. The Back to log in link displays correctly
	Then the page contains element with Data ID go-back-link
	And element with Data ID go-back-link contains a link to identity/account/login with text Back to log in

@3926
Scenario: 3. The Reset Password Confirmation page title is displayed correctly
	Then the page contains element with Data ID page-title
	And element with Data ID page-title has tag h1
	And element with Data ID page-title has text Password set

@3926
Scenario: 4. The Reset Password Confirmation page description is displayed correctly
	Then the page contains element with Data ID page-description
	And element with Data ID page-description has tag h2
	And element with Data ID page-description has text You can now use your new password to log in.

@3926
Scenario: 5. The NHS Footer is displayed
    Then the page contains element with Data ID nhsuk-footer
    And element with Data ID nhsuk-footer contains a link to /guide
    And element with Data ID nhsuk-footer contains a link to /guide#contact-us
    And element with Data ID nhsuk-footer contains a link to https://digital.nhs.uk/
    And element with Data ID nhsuk-footer contains a link to https://digital.nhs.uk/services/future-gp-it-systems-and-services
    And element with Data ID nhsuk-footer contains a link to https://gpitbjss.atlassian.net/wiki/spaces/GPITF/overview

@3926
Scenario: 6. The NHS Legal is displayed
    Then the page contains element with Data ID nhsuk-legal-panel
    And element with Data ID nhsuk-legal-panel contains a link to https://digital.nhs.uk/about-nhs-digital/privacy-and-cookies
