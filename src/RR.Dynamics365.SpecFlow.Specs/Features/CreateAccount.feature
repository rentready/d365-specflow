Feature: CreateAccount

Scenario: Should Create an Account
	Given an account named MainAccount created with the following values
		| Property | Value            |
		| name     | Awersome Account |
	Then I expect an account exists with the following values
		| Property | Value            |
		| name     | Awersome Account |