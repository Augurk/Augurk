@ignore @augurk @documentation
Feature: Customization through Tags
Gherkin allows for tags to be placed upon features, scenarios and example-groups to improve automatization and/or implement categorization.
Augurk uses this mechanism to allow its users some control over how their features are displayed.

Scenario: An ignore tag on this feature
Putting an ignore tag on a feature results in a big red box being displayed,
indicating that no automation takes place on the scenarios in that feature.
	Given a tag "ignore" is placed on this feature
	When I open this feature in Augurk
	Then "ignore" is not displayed as part of the tags on the feature
	And a red box containing a warning is dispayed below the title of this feature 

@ignore
Scenario: Decorating this scenario with an ignore tag
Decorating a scenario with an ignore tag does nothing in Augurk.
	Given the tag "ignore" is placed on this scenario
	When I find this scenario in Augurk
	Then "ignore" is displayed as part of the tags on the scenario