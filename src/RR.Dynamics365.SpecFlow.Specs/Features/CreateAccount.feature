Feature: CreateAccount

Scenario: Should Create an Account with name Awersome Account and Default numberofemployees = 10
	Given an account named MainAccount created with the following values
		| Property | Value            |
		| name     | Awersome Account |
	Given an existing account named Altername with the following values is available within 0 seconds
		| Property | Value            |
		| name     | Awersome Account |
	Given an account named Altername2 exists with the following values
		| Property | Value            |
		| name     | Awersome Account |
	Then I expect an account exists with the following values
		| Property          | Value            |
		| name              | Awersome Account |
		| numberofemployees | 10               |

Scenario: Should Update an Account name
	Given an account named MainAccount created with the following values
		| Property | Value            |
		| name     | Awersome Account |
	When I update MainAccount with the following values
		| Property | Value            |
		| name     | Bad Account |
	Then I expect an account exists with the following values
		| Property          | Value       |
		| name              | Bad Account |

Scenario: Should map lookup fields by an entity's primary name
	Given a contact named MainContact created with the following values
		| Property | Value     |
		| fullname | John Snow |
	Given an account named MainAccount created with the following values
		| Property         | Value            |
		| name             | Awersome Account |
		| primarycontactid | John Snow        |

Scenario: Should map lookup fields by an entity's alias
	Given a contact named MainContact created with the following values
		| Property | Value     |
		| fullname | John Snow |
	Given an account named MainAccount created with the following values
		| Property         | Value            |
		| name             | Awersome Account |
		| primarycontactid | MainContact      |

Scenario: Should create a list of accounts
	Given entities account created with the following values
		| Alias | name             |
		| Acc1  | Awersome Account |
		| Acc2  | Bad Account      |
	Given entities account exist with the following values
		| Alias  | name             |
		| Acc1.1 | Awersome Account |
		| Acc2.1 | Bad Account      |