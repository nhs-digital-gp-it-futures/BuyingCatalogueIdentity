Feature: Display an Organisations Related Organisations by Id in the Authority Section
    As an Authority User
    I want to view an Organisations Related Organisations
    so that i can manage Proxy Buying Relationships

Background: 
    Given Organisations exist
        | Name                  | OdsCode | PrimaryRoleId | CatalogueAgreementSigned | Line1 | Line2      | Line3           | Line4       | Town        | County          | Postcode | Country |
        | PrimaryOrganisation   | Ods 1   | ID 1          | true                     | 12    | Brick Lane | Central Area    | City Centre | Leeds       | West Yorkshire  | LS1 1AW  | England |
        | RelatedOrganisation   | Ods 2   | ID 2          | false                    | 15    | Sun Ave    | End of the Road | Suburb      | York        | North Yorkshire | YO11 4LO | England |
        | UnrelatedOrganisation | Ods 3   | ID 3          | false                    | 18    | Moon Road  | Gherkin Way     | Outskirts   | Blackpool   | Lancashire      | BL69 4BZ | England |
    And Users exist
        | Id     | OrganisationName      | FirstName | LastName | Email                | PhoneNumber | Disabled | Password        | OrganisationFunction |
        | 012345 | PrimaryOrganisation   | Penny     | Lane     | PennyLane@email.com  | 01234567890 | false    | S0mePa$$w0rd    | Buyer                |
        | 123456 | PrimaryOrganisation   | Post      | Pat      | PostmanPat@email.com | 12345678901 | false    | An0therPa$$w0rd | Authority            |

@5148
Scenario: An Organisation is the Parent of another Organisation
    Given Organisation PrimaryOrganisation has a Parent Relationship to Organisation RelatedOrganisation
    And a user is logged in
         | Username            | Password     | Scope        |
         | PennyLane@email.com | S0mePa$$w0rd | Organisation |
    When a GET request for RelatedOrganisations is made for an Organisation with name PrimaryOrganisation
    Then a response with status code 200 is returned
    And the RelatedOrganisation is returned with the following values
         | Name                | OdsCode |
         | RelatedOrganisation | Ods 2   |

@5148
Scenario: An Organisation has no Relations to Another Organisation
    Given a user is logged in
         | Username            | Password     | Scope        |
         | PennyLane@email.com | S0mePa$$w0rd | Organisation |
    When a GET request for RelatedOrganisations is made for an Organisation with name PrimaryOrganisation
    Then a response with status code 200 is returned
    And a response with an empty body is returned

@5148
Scenario: Organisation is not found
    Given a user is logged in
         | Username            | Password     | Scope        |
         | PennyLane@email.com | S0mePa$$w0rd | Organisation |
    And an Organisation with name NonExistentOrganisation does not exist
    When a GET request for RelatedOrganisations is made for an Organisation with name NonExistentOrganisation
    Then a response with status code 404 is returned

@5148
Scenario: A buyer can access the list of RelatedOrganisations
    Given a user is logged in
         | Username            | Password     | Scope        |
         | PennyLane@email.com | S0mePa$$w0rd | Organisation |
    When a GET request for RelatedOrganisations is made for an Organisation with name PrimaryOrganisation
    Then a response with status code 200 is returned

@5148
Scenario: If a user is not authorised then they cannot access the list of RelatedOrganisations
    When a GET request for RelatedOrganisations is made for an Organisation with name PrimaryOrganisation
    Then a response with status code 401 is returned

@5148
Scenario: Service Failure
    Given a user is logged in
         | Username            | Password     | Scope        |
         | PennyLane@email.com | S0mePa$$w0rd | Organisation |
    And the call to the database will fail
    When a GET request for RelatedOrganisations is made for an Organisation with name PrimaryOrganisation 
    Then a response with status code 500 is returned
