Feature: Display all the Users For An Organisation
    As a Authority User
    I want to view all of the Users for an Organisation
    So that I can ensure the information is correct

Background: 
    Given Organisations exist
        | Name           | OdsCode |
        | Organisation 1 | Ods 1   |
    And Users exist
        | Id  | OrganisationName | FirstName | LastName | Email   | PhoneNumber | Disabled | Password |
        | 123 | Organisation 1   | John      | Doe      | a@b.com | 01234567890 | false    | yolo     |

@5147
Scenario: 1. Get all of the users for an organisation
    Given an user is logged in
    	| Username           | Password | Scope        |
		| BobSmith@email.com | Pass123$ | Organisation |
    When a GET request is made for an organisation's users with name Organisation 1
    Then a response with status code 200 is returned
    And the Users list is returned with the following values
        | UserId | FirstName | LastName | EmailAddress | PhoneNumber | IsDisabled |
        | 123    | John      | Doe      | a@b.com      | 01234567890 | False      |
        
@5147
Scenario: 2. If an organisation does not exist, an empty list is returned
    Given an user is logged in
    	| Username           | Password | Scope        |
		| BobSmith@email.com | Pass123$ | Organisation |
    When a GET request is made for an organisation's users with name Organisation 2
    Then a response with status code 200 is returned
    And the Users list is returned with the following values
        | UserId | FirstName | LastName | EmailAddress | PhoneNumber | IsDisabled |

@5147
Scenario: 3. If a user is not authorised then they cannot access the organisations
    When a GET request is made for an organisation's users with name Organisation 1
    Then a response with status code 401 is returned

@5147
Scenario: 4. Service Failure
    Given an user is logged in
    	| Username           | Password | Scope        |
		| BobSmith@email.com | Pass123$ | Organisation |
    Given the call to the database will fail
    When a GET request is made for an organisation's users with name Organisation 1
    Then a response with status code 500 is returned
