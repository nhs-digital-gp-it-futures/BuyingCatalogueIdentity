Feature: Buying Catalogue Identity healthchecks
    As BuyingCatalogueIdentity
    I want to be be able to check the health of my dependencies
    So that I can behave accordingly

@5648
Scenario: 1. Database Server is up
	Given The Database Server is up for ISAPI
	When the dependency health-check endpoint is hit for ISAPI
	Then the response will be Healthy

@5648
Scenario: 2. Database Server is down
	Given The Database Server is down for ISAPI
	When the dependency health-check endpoint is hit for ISAPI
	Then the response will be Unhealthy
