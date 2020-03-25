Feature: Display all the Users For An Organisation
    As a Authority User
    I want to view all of the Users for an Organisation
    So that I can ensure the information is correct

Background: 
    Given Organisations exist
        | Name           | OdsCode |
        | Organisation 1 | Ods 1   |
    And Users exist
        | Id  | OrganisationName | FirstName | LastName | Email             | PhoneNumber | Disabled | Password | OrganisationFunction |
        | 123 | Organisation 1   | John      | Doe      | authority@doe.com | 01234567890 | false    | yolo     | Authority            |
        | 234 | Organisation 1   | Jane      | Doe      | buyer@doe.com     | 01234567890 | false    | oloy     | Buyer                |

@5147
Scenario: 1. As an Authority user, get all of the users for an organisation
    Given an user is logged in
    	| Username          | Password | Scope        |
    	| authority@doe.com | yolo     | Organisation |
    When a GET request is made for an organisation's users with name Organisation 1
    Then a response with status code 200 is returned
    And the Users list is returned with the following values
        | UserId | FirstName | LastName | EmailAddress      | PhoneNumber | IsDisabled |
        | 123    | John      | Doe      | authority@doe.com | 01234567890 | False      |
        | 234    | Jane      | Doe      | buyer@doe.com     | 01234567890 | False      |
        
@5147
Scenario: 2. If an organisation does not exist, an empty list is returned
    Given an user is logged in
    	| Username          | Password | Scope        |
    	| authority@doe.com | yolo     | Organisation |
    When a GET request is made for an organisation's users with name Organisation 2
    Then a response with status code 200 is returned
    And the Users list is returned with the following values
        | UserId | FirstName | LastName | EmailAddress | PhoneNumber | IsDisabled |

@5147
Scenario: 3. If a user is not authorised then they cannot access the organisations
    Given an user is logged in
    	| Username      | Password | Scope        |
    	| buyer@doe.com | oloy     | Organisation |
    When a GET request is made for an organisation's users with name Organisation 1
    Then a response with status code 403 is returned

@5147
Scenario: 4. If a user is not logged in then they cannot access the organisations
    When a GET request is made for an organisation's users with name Organisation 1
    Then a response with status code 401 is returned

@5147
Scenario: 5. Service Failure
    Given an user is logged in
    	| Username          | Password | Scope        |
    	| authority@doe.com | yolo     | Organisation |
    Given the call to the database to set the field will fail
    When a GET request is made for an organisation's users with name Organisation 1
    Then a response with status code 500 is returned
