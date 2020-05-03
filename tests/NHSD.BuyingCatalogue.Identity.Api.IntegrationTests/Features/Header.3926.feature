Feature: Header UI
    As an Authority User
    I want to see the contents of the page header

Background:
	Given the user navigates directly to the login page

@3926
Scenario: 1. The page contains a header
	Then the page contains element with Data ID header-banner

@3926
Scenario: 2. The NHS Header contains the NHS Header logo
	Then the page contains element with Data ID header-banner
	And element with Data ID header-banner contains element with Data ID nhs-digital-logo

@3926
Scenario: 3. The NHS Header logo should link to the public browse homepage
	Then element with Data ID header-banner contains a link to http://localhost:8080/publicbrowse

@3926
Scenario: 4. The NHS Header logo should contain an accessibility label
	Then element with Data ID header-logo has the accessibility text Buying Catalogue Homepage

@3926
Scenario: 5. The NHS Header logo should contain a title
	Then element with Data ID header-logo-title has text NHS Digital logo