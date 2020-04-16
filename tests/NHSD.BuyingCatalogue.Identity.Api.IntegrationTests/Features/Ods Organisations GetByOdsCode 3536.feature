Feature: Display an Ods Organisation by its Ods Code
  As an Authority User
  I want to view information for a single organisation
  So that I can manage access to the Buying Catalogue

Background:
	Given Ods Organisations exist
		| Name               | OdsCode | PrimaryRoleId | Status    | AddrLn1 | AddrLn2         | AddrLn3         | AddrLn4     | Town     | County          | Postcode | Country |
		| Ods Organisation 1 | B1G     | Buyer         | Active    | 12      | Brick Lane      | Central Area    | City Centre | Leeds    | West Yorkshire  | LS1 1AW  | England |
		| Ods Organisation 2 | B2G     | Buyer         | Disbanded | 37      | Sand Brick Lane | Central Area    | City Centre | Bradford | West Yorkshire  | BD4 1FK  | England |
		| Ods Organisation 3 | B3G     | AlsoBuyer     | Active    | 21      | Jump Street     | All over        | Suburb      | Metairie | Louisiana       | 504      | USA     |
		| Ods Organisation 4 | N0G     | 153XP         | Active    | 15      | Sun Ave         | End of the Road | Suburb      | York     | North Yorkshire | YO11 4LO | England |
	And Organisations exist
		| Name           | OdsCode |
		| Organisation 1 | Ods 1   |
	And Users exist
		| Id     | OrganisationName | FirstName | LastName | Email                | PhoneNumber | Disabled | Password        | OrganisationFunction |
		| 012345 | Organisation 1   | Penny     | Lane     | PennyLane@email.com  | 01234567890 | false    | S0mePa$$w0rd    | Buyer                |
		| 123456 | Organisation 1   | Post      | Pat      | PostmanPat@email.com | 12345678901 | false    | An0therPa$$w0rd | Authority            |

@3536
Scenario: 1. Get the details of a single buyer organisation
	Given a user is logged in
		| Username             | Password        | Scope        |
		| PostmanPat@email.com | An0therPa$$w0rd | Organisation |
	When a GET request is made for an Ods organisation with code B1G
	Then a response with status code 200 is returned
	And the Ods Organisation is returned with the following values
		| OrganisationName   | OdsCode | PrimaryRoleId | Line1 | Line2      | Line3        | Line4       | Town  | County         | Postcode | Country |
		| Ods Organisation 1 | B1G     | Buyer         | 12    | Brick Lane | Central Area | City Centre | Leeds | West Yorkshire | LS1 1AW  | England |

@3536
Scenario: 2. Get the details of a single buyer organisation with different buyer role id
	Given a user is logged in
		| Username             | Password        | Scope        |
		| PostmanPat@email.com | An0therPa$$w0rd | Organisation |
	When a GET request is made for an Ods organisation with code B3G
	Then a response with status code 200 is returned
	And the Ods Organisation is returned with the following values
		| OrganisationName   | OdsCode | PrimaryRoleId | Line1 | Line2       | Line3    | Line4  | Town     | County    | Postcode | Country |
		| Ods Organisation 3 | B3G     | AlsoBuyer     | 21    | Jump Street | All over | Suburb | Metairie | Louisiana | 504      | USA     |

@3536
Scenario: 3. Organisation is not found
	Given a user is logged in
		| Username             | Password        | Scope        |
		| PostmanPat@email.com | An0therPa$$w0rd | Organisation |
	When a GET request is made for an Ods organisation with code N0P3
	Then a response with status code 404 is returned

@3536
Scenario: 4. Non-active organisation is found
	Given a user is logged in
		| Username             | Password        | Scope        |
		| PostmanPat@email.com | An0therPa$$w0rd | Organisation |
	When a GET request is made for an Ods organisation with code B2G
	Then a response with status code 406 is returned

@3536
Scenario: 5. Non-buyer organisation is found
	Given a user is logged in
		| Username             | Password        | Scope        |
		| PostmanPat@email.com | An0therPa$$w0rd | Organisation |
	When a GET request is made for an Ods organisation with code N0G
	Then a response with status code 406 is returned

@3536
Scenario: 6. A non authority user cannot access the organisations
	Given a user is logged in
		| Username            | Password     | Scope        |
		| PennyLane@email.com | S0mePa$$w0rd | Organisation |
	When a GET request is made for an Ods organisation with code B1G
	Then a response with status code 403 is returned

@3536
Scenario: 7. If a user is not authorised then they cannot access the organisation
	When a GET request is made for an Ods organisation with code B1G
	Then a response with status code 401 is returned

@3536
Scenario: 8. Service Failure
	Given a user is logged in
		| Username             | Password        | Scope        |
		| PostmanPat@email.com | An0therPa$$w0rd | Organisation |
	And Ods API is down
	When a GET request is made for an Ods organisation with code B1G
	Then a response with status code 500 is returned
