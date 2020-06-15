Feature: Display all of the Service Recipients for an organisation
    As a Buyer
    I want to be able to view all of the Service Recipients for an organisation
    So that I can ensure all of the information is correct

Background:
    Given Organisations exist
        | Name           | OdsCode | PrimaryRoleId | CatalogueAgreementSigned | Line1 | Line2      | Line3        | Line4       | Town  | County         | Postcode | Country |
        | Organisation 1 | Ods 1   | ID 1          | true                     | 12    | Brick Lane | Central Area | City Centre | Leeds | West Yorkshire | LS1 1AW  | England |
    And Users exist
        | Id  | OrganisationName | FirstName | LastName | Email         | PhoneNumber | Disabled | Password     | OrganisationFunction |
        | 234 | Organisation 1   | Jane      | Doe      | buyer@doe.com | 01234567890 | false    | W3AkP4s5w0rd | Buyer                |

@7412
Scenario: 1. Get a list of service recipients from an organisation
    Given Ods Child Organisations exist for organisation Ods 1
        | Name               | OdsCode | PrimaryRoleId |
        | Ods Organisation 1 | B1G     | RO177         |
        | Ods Organisation 2 | B2G     | RO177         |
        | Ods Organisation 3 | B3G     | RO177         |
        | Ods Organisation 4 | N0G     | RO171         |
    Given a user is logged in
        | Username      | Password     | Scope        |
        | buyer@doe.com | W3AkP4s5w0rd | Organisation |
    When the user makes a request to retrieve the service recipients with an organisation name Organisation 1
    Then a response with status code 200 is returned
    And The organisation service recipient is returned with the following values
        | Name               | OdsCode |
        | Organisation 1     | Ods 1   |
        | Ods Organisation 1 | B1G     |
        | Ods Organisation 2 | B2G     |
        | Ods Organisation 3 | B3G     |

@7412
Scenario: 2. Get a list of service recipients from an organisation with no children
    Given Ods Child Organisations exist for organisation Ods 1
        | Name               | OdsCode | PrimaryRoleId |
    Given a user is logged in
        | Username      | Password     | Scope        |
        | buyer@doe.com | W3AkP4s5w0rd | Organisation |
    When the user makes a request to retrieve the service recipients with an organisation name Organisation 1
    Then a response with status code 200 is returned
    And The organisation service recipient is returned with the following values
        | Name               | OdsCode |
        | Organisation 1     | Ods 1   |

@7412
Scenario: 3. A buyer user cannot access the service recipients for an organisation they dont belong to
    Given a user is logged in
        | Username      | Password     | Scope        |
        | buyer@doe.com | W3AkP4s5w0rd | Organisation |
    When the user makes a request to retrieve the service recipients with an organisation name Organisation 2
    Then a response with status code 403 is returned

@7412
Scenario: 4. If a user is not authorised, they cannot view a list of service recipients
    When the user makes a request to retrieve the service recipients with an organisation name Organisation 1
    Then a response with status code 401 is returned

@7412
Scenario: 5. Service Failure
    Given a user is logged in
        | Username      | Password     | Scope        |
        | buyer@doe.com | W3AkP4s5w0rd | Organisation |
    And the call to the database will fail
    When the user makes a request to retrieve the service recipients with an organisation name Organisation 1
    Then a response with status code 500 is returned
