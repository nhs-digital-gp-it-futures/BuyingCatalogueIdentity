﻿Feature: Disable User
    As an Authority User
    I want to be able to disable Users accounts
    So that the user would not be able to access the Buying Catalogue

Background:
    Given Organisations exist
        | Name           | OdsCode |
        | Organisation 1 | Ods 1   |
    And Users exist
        | Id  | OrganisationName | FirstName | LastName | Email             | PhoneNumber | Disabled | Password | OrganisationFunction |
        | 123 | Organisation 1   | John      | Doe      | authority@doe.com | 01234567890 | false    | yolo     | Authority            |
        | 234 | Organisation 1   | Jane      | Doe      | buyer@doe.com     | 01234567890 | false    | oloy     | Buyer                |

@3543
Scenario: An authority user can disable a users account
    Given a user is logged in
        | Username          | Password | Scope        |
        | authority@doe.com | yolo     | Organisation |
    When a POST request is made to disable user with id 234
    Then a response with status code 204 is returned
    And the database has user with id 234
        | Name     | PhoneNumber | EmailAddress  | Disabled | OrganisationName |
        | Jane Doe | 01234567890 | buyer@doe.com | true     | Organisation 1   |

@3543
Scenario: User is not found
    Given a user is logged in
        | Username          | Password | Scope        |
        | authority@doe.com | yolo     | Organisation |
    When a POST request is made to disable user with id unknown
    Then a response with status code 404 is returned

@3543
Scenario: A non-authority user cannot disable a users account
    Given a user is logged in
        | Username      | Password | Scope        |
        | buyer@doe.com | oloy     | Organisation |
    When a POST request is made to disable user with id 234
    Then a response with status code 403 is returned

@3543
Scenario: Disabling a user when unauthorised
    When a POST request is made to disable user with id 234
    Then a response with status code 401 is returned

@3543
Scenario: Service Failure
    Given a user is logged in
        | Username          | Password | Scope        |
        | authority@doe.com | yolo     | Organisation |
    Given the call to the database will fail
    When a POST request is made to disable user with id 234
    Then a response with status code 500 is returned
