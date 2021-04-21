Feature: Display an Organisations Unrelated Organisations
    As an Authority User
    I want to view Unrelated Organisations when an Organisation is Related To Other Organisations
    so that i can manage Proxy Buying Relationships

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

@5149
Scenario: An Organisation Exists and is Related to another Organisation
    Given Organisation PrimaryOrganisation has a Parent Relationship to Organisation RelatedOrganisation
    And a user is logged in
         | Username            | Password     | Scope        |
         | PennyLane@email.com | S0mePa$$w0rd | Organisation |
    When a GET request for UnrelatedOrganisations is made for an Organisation with name PrimaryOrganisation
    Then a response with status code 200 is returned
    And a list of Related Organisations is returned with the following values
         | Name                  | OdsCode |
         | UnRelatedOrganisation | Ods 3   |

@5149
Scenario: An Organisation Exists but has no relations to any other organisation
    Given a user is logged in
         | Username            | Password     | Scope        |
         | PennyLane@email.com | S0mePa$$w0rd | Organisation |
    When a GET request for UnrelatedOrganisations is made for an Organisation with name PrimaryOrganisation
    Then a response with status code 200 is returned
    And a list of Related Organisations is returned with the following values
         | Name                  | OdsCode |
         | RelatedOrganisation   | Ods 2   |
         | UnRelatedOrganisation | Ods 3   |

@5149
Scenario: The List of Unrelated Organisations Should not contain the calling Organisation
    Given a user is logged in
         | Username            | Password     | Scope        |
         | PennyLane@email.com | S0mePa$$w0rd | Organisation |
    When a GET request for UnrelatedOrganisations is made for an Organisation with name PrimaryOrganisation
    Then a response with status code 200 is returned
    And a list of Related Organisations is returned that does not contain the following values
         | Name                | OdsCode |
         | PrimaryOrganisation | Ods 1   |

@5149
Scenario: Organisation is not found
    Given a user is logged in
         | Username            | Password     | Scope        |
         | PennyLane@email.com | S0mePa$$w0rd | Organisation |
    And an Organisation with name NonExistantOrganisation does not exist
    When a GET request for UnrelatedOrganisations is made for an Organisation with name NonExistantOrganisation
    Then a response with status code 404 is returned

@5149
Scenario: A buyer can access the Unrelated Organisations
    Given a user is logged in
         | Username            | Password     | Scope        |
         | PennyLane@email.com | S0mePa$$w0rd | Organisation |
    When a GET request for UnrelatedOrganisations is made for an Organisation with name PrimaryOrganisation
    Then a response with status code 200 is returned

@5149
Scenario: If a user is not authorised then they cannot access the Unrelated Organisations
    When a GET request for UnrelatedOrganisations is made for an Organisation with name PrimaryOrganisation
    Then a response with status code 401 is returned

@5149
Scenario: Service Failure
    Given a user is logged in
        | Username             | Password        | Scope        |
        | PostmanPat@email.com | An0therPa$$w0rd | Organisation |
    And the call to the database will fail
    When a GET request for UnrelatedOrganisations is made for an Organisation with name PrimaryOrganisation
    Then a response with status code 500 is returned

