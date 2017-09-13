@childOf:CustomizationthroughTags @augurk @documentation
Feature: Nesting Features
When using Gherkin to describe your software, the number of feature files will quickly increase to a nearly unmanageable amount.
To combat the ever growing set of feature files becoming completely unmanageable, Augurk offers the option to nest feature files in a hierarchical way.

Scenario: This feature is a child of the Server-side Tags feature
Augurk allows you to nest features by declaring its parent with a parent: or childOf: tag.
	Given a tag "childOf:CustomizationthroughTags" is placed on this feature
	When I look for this feature in Augurks menu
	Then I will find it collapsed under "Server-side Tags"

Scenario: Opening a nested feature
Augurk will not display childOf tags on a feature.
	Given the tag "childOf:CustomizationthroughTags" is placed on this feature
	When I open this feature in Augurk
	Then "childOf:Server-sideTags" is not displayed as part of the tags on the feature

Scenario: A feature has been nested under a feature that does not exist
If the parent-feature does not exist, the feature will be shown on the root level.
	Given the tag "childOf:NonExistingFeature" is placed on a feature
	When I look for that feature in Augurks menu
	Then I will find it at the root level of its group