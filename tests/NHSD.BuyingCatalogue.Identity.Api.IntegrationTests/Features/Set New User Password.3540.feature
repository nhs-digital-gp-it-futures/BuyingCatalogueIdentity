Feature: Set New User Password
    As a new User
    I want to set my user password
    So I can log in and use the system

Background:
    Given Organisations exist
        | Name           | OdsCode |
        | Organisation 1 | Ods 1   |
    And Users exist
        | Id     | OrganisationName | FirstName | LastName | Email                | PhoneNumber | Disabled | Password        | OrganisationFunction |
        | 012345 | Organisation 1   | Penny     | Lane     | PennyLane@email.com  | 01234567890 | false    | S0mePa$$w0rd    | Buyer                |
        | 123456 | Organisation 1   | Post      | Pat      | PostmanPat@email.com | 12345678901 | false    | An0therPa$$w0rd | Authority            |
    Given a user is logged in
        | Username             | Password        | Scope        |
        | PostmanPat@email.com | An0therPa$$w0rd | Organisation |
    When a POST request is made to create a user for organisation Organisation 1
        | FirstName | LastName   | PhoneNumber | EmailAddress             | OrganisationName |
        | Bob       | Bobkovitch | 0123456789  | bob.bobkovitch@email.com | Organisation 1   |
    Then a response with status code 201 is returned
    And the email sent contains the following information
        | From                           | To                       | Subject                                            |
        | noreply@buyingcatalogue.nhs.uk | bob.bobkovitch@email.com | INTEGRATION_TEST Set password for Buying Catalogue |
    And only one email is sent

@3540
Scenario: 1. User can set password on their account and then log in
    When The email link is clicked
    Then the user is redirected to page identity/account/resetpassword
    When element with Data ID input-reset-password is populated with Password123!
    And element with Data ID input-confirm-reset-password is populated with Password123!
    And element with Data ID reset-password-button is clicked
    Then the user is redirected to page account/resetpasswordconfirmation
    When the user navigates to a restricted web page
    Then the user is redirected to page identity/account/login
    When a login request is made with email address bob.bobkovitch@email.com and password Password123!
#    Then the user is redirected to page identity/Consent
#    When element with Data ID agree-terms-checkbox is clicked
#    And element with Data ID agreement-submit-button is clicked
    Then the user is redirected to page home/privacy
    And the page contains element with ID sampleResourceResult with text Authorized With Sample Resource

@3540
Scenario: 2. User cannot use the same link to set their password twice
    When The email link is clicked
    Then the user is redirected to page identity/account/resetpassword
    When element with Data ID input-reset-password is populated with Password123!
    And element with Data ID input-confirm-reset-password is populated with Password123!
    And element with Data ID reset-password-button is clicked
    Then the user is redirected to page account/resetpasswordconfirmation
    When The email link is clicked
    Then the user is redirected to page account/resetpasswordexpired

@3540
Scenario: 3. User cannot modify the link to set someone elses password
    When The email link is modified to use email address PostmanPat@email.com
    When The email link is clicked
    Then the user is redirected to page account/resetpasswordexpired
