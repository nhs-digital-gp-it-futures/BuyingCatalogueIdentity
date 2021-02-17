Feature: Buying Catalogue Identity healthchecks
    As BuyingCatalogueIdentity
    I want to be be able to check the health of my dependencies
    So that I can behave accordingly

@3926
Scenario: Smtp Server is up
    Given The Smtp Server is up
    When the dependency health-check endpoint is hit
    Then the response will be Healthy

@3926
Scenario: Smtp Server is down
    Given The Smtp Server is down
    When the dependency health-check endpoint is hit
    Then the response will be Degraded

@5648
Scenario: Database Server is up
    When the dependency health-check endpoint is hit
    Then the response will be Healthy

@5648
Scenario: Database Server is down
    Given The Database Server is down
    When the dependency health-check endpoint is hit
    Then the response will be Unhealthy
