@augurk @documentation
Feature: Link to other features
Occasionally, a feature might reference another feature. Augurk supports this in the feature descriptions like this:  
[Display Gherkin Scenarios]

Scenario: This feature references the Display Gherkin Scenarios feature
Augurk allows other features to be referenced by enclosing their name between \[brackets\]
	Given the description of this feature contains the text "[Display Gherkin Scenarios]" 
	When I view this feature in Augurk
	Then the description contains the text "Display Gherkin Scenarios" linking to that feature