 @augurk @documentation
Feature: Versioning
	As a product grows its features might change over time. Sometimes it might be useful to view an older version of a particular
	feature. To help with this, Augurk supports versioning features files allowing different versions of a feature to be displayed.

Scenario: Displays the most current version of a feature initially
	By default when a feature is selected from the menu its most current version is shown initially

	Given version 1.0.0 of this feature is available in Augurk
	When I select this feature in Augurk's menu
	Then the 1.0.0 version is displayed
