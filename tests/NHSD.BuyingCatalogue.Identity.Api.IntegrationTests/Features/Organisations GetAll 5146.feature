Feature: Display all the Organisations in an Authority Section
	As a Authority User
	I want to view all of the Organisations in the solution
	So that I can ensure the information is correct

Background: 
    Given Organisations exist
        | Name           | OdsCode |
        | Organisation 1 | Ods 1   |
        | Organisation 2 | Ods 2   |
        | Organisation 3 | Ods 3   |

@5146
Scenario: 1. Get all of the organisations
    Given an authority user is logged in
	When a GET request is made for the Organisations section
    Then a response with status code 200 is returned
    And the Organisations list is returned with the following values
        | Name           | OdsCode |
        | Organisation 1 | Ods 1   |
        | Organisation 2 | Ods 2   |
        | Organisation 3 | Ods 3   |

@5146
Scenario: 2. Service Failure
    Given an authority user is logged in
	Given the call to the database to set the field will fail
	When a GET request is made for the Organisations section
	Then a response with status code 500 is returned
