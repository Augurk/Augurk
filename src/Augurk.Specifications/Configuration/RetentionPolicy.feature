@augurk @documentation
Feature: Retention Policy
Augurk allows for a retention policy to be configured for features which 
follow a specified version pattern. Versions of features which have a retention
will automatically be removed once the retention period has elapsed.

Scenario: Publishing a prerelease feature file with a retention policy
By default the retention filter is configured to match semantic
prerelease versions.

	Given a configured retention policy of 30 days
	And retention filter '[0-9\.]+-.*'
	When I publish a feature file as version 1.0.0-alpha001
	Then the feature file will receive a retention of 30 days

Scenario: Publishing a definitive feature file with a retention policy
By default the retention filter is configured to only match 
semantic prerelease versions. Publishing a 1.0.0 version does
not match this.

	Given a configured retention policy of 30 days
	And retention filter '[0-9\.]+-.*'
	When I publish a feature file as version 1.0.0
	Then the feature file will receive indefinite retention

Scenario: Configuring a retention policy
Once published, the retention of a feature will 
not be altered when the retention policy is configured

	Given a feature file with version 1.0.0-alpha001 and indefinite retention
	And retention filter '[0-9\.]+-.*'
	When I configure the retention policy for 30 days
	Then the feature file has indefinite retention