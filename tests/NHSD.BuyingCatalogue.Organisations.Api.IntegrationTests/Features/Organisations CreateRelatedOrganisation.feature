Feature: Create a new Organisation Relationship
    As an Authority User
    I want to be able to create a relationship between two Organisations
    so that I can manage Proxy Buying Relationships

Background: 
    Given Organisations exist
        | Name                  | OdsCode | PrimaryRoleId | CatalogueAgreementSigned | Line1 | Line2      | Line3           | Line4       | Town        | County          | Postcode | Country |
        | PrimaryOrganisation   | Ods 1   | ID 1          | true                     | 12    | Brick Lane | Central Area    | City Centre | Leeds       | West Yorkshire  | LS1 1AW  | England |
        | RelatedOrganisation   | Ods 2   | ID 2          | false                    | 15    | Sun Ave    | End of the Road | Suburb      | York        | North Yorkshire | YO11 4LO | England |
        | UnRelatedOrganisation | Ods 3   | ID 3          | false                    | 18    | Moon Road  | Gherkin Way     | Council Dump| Blackpool   | North Yorkshire | BL69 4BZ | England |
    And Users exist
        | Id     | OrganisationName      | FirstName | LastName | Email                | PhoneNumber | Disabled | Password        | OrganisationFunction |
        | 012345 | PrimaryOrganisation   | Penny     | Lane     | PennyLane@email.com  | 01234567890 | false    | S0mePa$$w0rd    | Buyer                |
        | 123456 | PrimaryOrganisation   | Post      | Pat      | PostmanPat@email.com | 12345678901 | false    | An0therPa$$w0rd | Authority            |

@5150
Scenario: An authority user can create a new Organisation relationship
    Given a user is logged in
        | Username             | Password        | Scope        |
        | PostmanPat@email.com | An0therPa$$w0rd | Organisation |
    When a POST request to RelatedOrganisations is made to add an Organisation with name RelatedOrganisation as a child of an Organisation with name PrimaryOrganisation
    Then a response with status code 204 is returned
    When a GET request for RelatedOrganisations is made for an Organisation with name PrimaryOrganisation
    Then a response with status code 200 is returned
    And the RelatedOrganisation is returned with the following values
        | Name                | OdsCode |
        | RelatedOrganisation | Ods 2   |

@5150
Scenario: An authority user tried to create an existing organisation relationship
    Given a user is logged in
        | Username             | Password        | Scope        |
        | PostmanPat@email.com | An0therPa$$w0rd | Organisation |
    And Organisation PrimaryOrganisation has a Parent Relationship to Organisation RelatedOrganisation
    When a POST request to RelatedOrganisations is made to add an Organisation with name RelatedOrganisation as a child of an Organisation with name PrimaryOrganisation
    Then a response with status code 400 is returned

@5150
Scenario: The Organisation does not exist
    Given a user is logged in
        | Username             | Password        | Scope        |
        | PostmanPat@email.com | An0therPa$$w0rd | Organisation |
    And an Organisation with name NonExistantOrganisation does not exist
    When a POST request to RelatedOrganisations is made to add an Organisation with name RelatedOrganisation as a child of an Organisation with name NonExistantOrganisation
    Then a response with status code 404 is returned

@5150
Scenario: The Related Organisation does not exist
    Given a user is logged in
        | Username             | Password        | Scope        |
        | PostmanPat@email.com | An0therPa$$w0rd | Organisation |
    And an Organisation with name NonExistantOrganisation does not exist
    When a POST request to RelatedOrganisations is made to add an Organisation with name NonExistantOrganisation as a child of an Organisation with name PrimaryOrganisation
    Then a response with status code 400 is returned


@5150
Scenario: a non-authority user cannot create a new Organisation relationship    
    Given a user is logged in
         | Username            | Password     | Scope        |
         | PennyLane@email.com | S0mePa$$w0rd | Organisation |
    When a POST request to RelatedOrganisations is made to add an Organisation with name RelatedOrganisation as a child of an Organisation with name PrimaryOrganisation
    Then a response with status code 403 is returned

@5150
Scenario: a user that is not logged in can not create an Organisation relationship
    When a POST request to RelatedOrganisations is made to add an Organisation with name RelatedOrganisation as a child of an Organisation with name PrimaryOrganisation
    Then a response with status code 401 is returned

@5150
Scenario: Service Failure
    Given a user is logged in
        | Username             | Password        | Scope        |
        | PostmanPat@email.com | An0therPa$$w0rd | Organisation |
    And the call to the database will fail
    When a POST request to RelatedOrganisations is made to add an Organisation with name RelatedOrganisation as a child of an Organisation with name PrimaryOrganisation
    Then a response with status code 500 is returned
