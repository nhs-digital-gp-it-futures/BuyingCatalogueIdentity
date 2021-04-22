Feature: Delete an existing Organisation Relationship
    As an Authority User
    I want to be able to delete a relationship between two Organisations
    so that I can manage Proxy Buying Relationships

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

@5151
Scenario: An authority user can delete an existing Organisation relationship
    Given a user is logged in
        | Username             | Password        | Scope        |
        | PostmanPat@email.com | An0therPa$$w0rd | Organisation |
    And Organisation PrimaryOrganisation has a Parent Relationship to Organisation RelatedOrganisation
    When a DELETE request to RelatedOrganisations is made to delete the relationship between a parent Organisation with name PrimaryOrganisation and a child Organisation with name RelatedOrganisation
    Then a response with status code 204 is returned
    When a GET request for RelatedOrganisations is made for an Organisation with name PrimaryOrganisation
    Then a response with status code 200 is returned
    And a response with an empty body is returned

@5151
Scenario: An authority user can delete an existing Organisation relationship that will not effect another relationship of the same type
    Given a user is logged in
        | Username             | Password        | Scope        |
        | PostmanPat@email.com | An0therPa$$w0rd | Organisation |
    And Organisation PrimaryOrganisation has a Parent Relationship to Organisation RelatedOrganisation
    And Organisation UnrelatedOrganisation has a Parent Relationship to Organisation RelatedOrganisation
    When a DELETE request to RelatedOrganisations is made to delete the relationship between a parent Organisation with name PrimaryOrganisation and a child Organisation with name RelatedOrganisation
    Then a response with status code 204 is returned
    When a GET request for RelatedOrganisations is made for an Organisation with name UnrelatedOrganisation
    Then a response with status code 200 is returned
    And the RelatedOrganisation is returned with the following values
         | Name                | OdsCode |
         | RelatedOrganisation | Ods 2   |

Scenario: An authority user tries to delete a relationship that does not exist
    Given a user is logged in
        | Username             | Password        | Scope        |
        | PostmanPat@email.com | An0therPa$$w0rd | Organisation |
    When a DELETE request to RelatedOrganisations is made to delete the relationship between a parent Organisation with name PrimaryOrganisation and a child Organisation with name UnrelatedOrganisation
    Then a response with status code 204 is returned

@5151
Scenario: Organisation does not exist
    Given a user is logged in
        | Username             | Password        | Scope        |
        | PostmanPat@email.com | An0therPa$$w0rd | Organisation |
    And an Organisation with name NonExistentOrganisation does not exist
    When a DELETE request to RelatedOrganisations is made to delete the relationship between a parent Organisation with name NonExistentOrganisation and a child Organisation with name RelatedOrganisation
    Then a response with status code 404 is returned

@5151
Scenario: the Related Organisation does not exist
    Given a user is logged in
        | Username             | Password        | Scope        |
        | PostmanPat@email.com | An0therPa$$w0rd | Organisation |
    And an Organisation with name NonExistentOrganisation does not exist
    When a DELETE request to RelatedOrganisations is made to delete the relationship between a parent Organisation with name PrimaryOrganisation and a child Organisation with name NonExistentOrganisation
    Then a response with status code 204 is returned

@5151
Scenario: Organisation and Related Organisation exist but have no relationship to eachother
    Given a user is logged in
        | Username             | Password        | Scope        |
        | PostmanPat@email.com | An0therPa$$w0rd | Organisation |
    When a DELETE request to RelatedOrganisations is made to delete the relationship between a parent Organisation with name PrimaryOrganisation and a child Organisation with name RelatedOrganisation
    Then a response with status code 204 is returned

@5151
Scenario: a non-authority user cannot delete an Organisation relationship
    Given a user is logged in
        | Username             | Password        | Scope        |
        | PennyLane@email.com  | S0mePa$$w0rd    | Organisation |
    And Organisation PrimaryOrganisation has a Parent Relationship to Organisation RelatedOrganisation
    When a DELETE request to RelatedOrganisations is made to delete the relationship between a parent Organisation with name PrimaryOrganisation and a child Organisation with name RelatedOrganisation
    Then a response with status code 403 is returned

@5151
Scenario: a user that is not logged in can not delete an Organisation relationship
    When a DELETE request to RelatedOrganisations is made to delete the relationship between a parent Organisation with name PrimaryOrganisation and a child Organisation with name RelatedOrganisation
    Then a response with status code 401 is returned

@5151
Scenario: Service Failure
    Given a user is logged in
        | Username             | Password        | Scope        |
        | PostmanPat@email.com | An0therPa$$w0rd | Organisation |
    And the call to the database will fail
    When a DELETE request to RelatedOrganisations is made to delete the relationship between a parent Organisation with name PrimaryOrganisation and a child Organisation with name RelatedOrganisation
    Then a response with status code 500 is returned
