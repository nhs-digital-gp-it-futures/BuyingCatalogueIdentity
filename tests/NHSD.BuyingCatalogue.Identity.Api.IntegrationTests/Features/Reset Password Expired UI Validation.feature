﻿Feature: Reset Password Link Expired UI
    As an User
    I want to view the reset password link expired page
    So that I can request a new link

Background:
    When the user navigates to identity url account/resetpasswordexpired
    Then the user is redirected to page account/resetpasswordexpired

@3926
Scenario: The NHS Header is displayed correctly
    Then the page contains element with Data ID header-banner

@3926
Scenario: The Reset Password Link Expired page title is displayed correctly
    Then the page contains element with Data ID page-title
    And element with Data ID page-title has tag h1
    And element with Data ID page-title has text Password link expired

@3926
Scenario: The Reset Password Link Expired page description is displayed correctly
    Then the page contains element with Data ID page-description
    And element with Data ID page-description has tag p
    And element with Data ID page-description has text This link is no longer valid. You must request a new one.

@3926
Scenario: The Reset Password button is displayed correctly
    Then the page contains element with Data ID request-new-link-button
    And element with Data ID request-new-link-button has tag button
    And element with Data ID request-new-link-button has text Request new link

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
