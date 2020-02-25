Feature: Login With Invalid Client
	As an Invalid Authority User
	I want not be able to log into the system
    So that I cannot modify any documents

@3540
Scenario: 1. Logging in with an invalid client ID with valid credentials
    Given the client is using invalid client ID and valid secret
	And the credentials for the client are valid
	When a login request is made with redirect URL http://callum.is.great.com
	#Then something bad will happen

@3540
Scenario: 2. Logging in with an invalid client secret with valid credentials
    Given the client is using valid client ID and invalid secret
	And the credentials for the client are valid
	#When A login request is made with redirect URL http://callum.is.great.com
	#Then something bad will happen