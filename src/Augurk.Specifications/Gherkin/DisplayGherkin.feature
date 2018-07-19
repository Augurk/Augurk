@augurk @documentation
Feature: Display Gherkin Scenarios
Augurk supports the full extend of Gherkin. However, ocasionally Gherkin scenarios can be somewhat overwelming.  
In order to improve the readablity of those scenarios, some additional UI options are provided.

For one, **Augurk** fully supports _Markdown_ in  feature summaries. Allowing you to use

+	Lists
+	**Bold**, and or _Italic_
+	[Links](https://en.wikipedia.org/wiki/Markdown)
+	Diacritics (e.g. é, à, ü)


Scenario: Scenario with a large data table
In order to improve readability, data tables can be collapsed by clicking their header

	Given this scenario has a large data table
	| Language Culture Name | Display Name             | Culture Code |
	| af-ZA                 | Afrikaans - South Africa | 0x0436       |
	| zh-CN                 | Chinese - China          | 0x0804       |
	| zh-HK                 | Chinese - Hong Kong SAR  | 0x0C04       |
	| cs-CZ                 | Czech - Czech Republic   | 0x0405       |
	| da-DK                 | Danish - Denmark         | 0x0406       |
	| nl-BE                 | Dutch - Belgium          | 0x0813       |
	| nl-NL                 | Dutch - The Netherlands  | 0x0413       |
	| en-AU                 | English - Australia      | 0x0C09       |
	| en-CA                 | English - Canada         | 0x1009       |
	| en-IE                 | English - Ireland        | 0x1809       |
	| en-ZA                 | English - South Africa   | 0x1C09       |
	| en-GB                 | English - United Kingdom | 0x0809       |
	| en-US                 | English - United States  | 0x0409       |
	| et-EE                 | Estonian - Estonia       | 0x0425       |
	| fo-FO                 | Faroese - Faroe Islands  | 0x0438       |
	| fa-IR                 | Farsi - Iran             | 0x0429       |
	| fi-FI                 | Finnish - Finland        | 0x040B       |
	| fr-BE                 | French - Belgium         | 0x080C       |
	| fr-CA                 | French - Canada          | 0x0C0C       |
	| fr-FR                 | French - France          | 0x040C       |
	| fr-LU                 | French - Luxembourg      | 0x140C       |
	| fr-MC                 | French - Monaco          | 0x180C       |
	| fr-CH                 | French - Switzerland     | 0x100C       |
	| de-AT                 | German - Austria         | 0x0C07       |
	| de-DE                 | German - Germany         | 0x0407       |
	| de-LI                 | German - Liechtenstein   | 0x1407       |
	| de-LU                 | German - Luxembourg      | 0x1007       |
	| de-CH                 | German - Switzerland     | 0x0807       |
	When I click the header of the table in the given step
	Then the table in the given step is collapsed

Scenario: Collapsing scenario
In order to improve readability, scenarios can be collapsed by clicking their title
	Given I am viewing this scenario in Augurk
	When I click the title of this scenario
	Then the scenario is collapsed

@notImplemented
Scenario Outline: Readable Scenario Outlines
Scenario outlines provide can provide a lot of examples. To make verification easier, 
clicking on a scenario will result in the placeholders being replaced by the actual values.

	Given this scenario outline contains multiple examples
	When I click in the example described as '<description>'
	Then the placeholder above is updated with the value of the description column
	And <expectation>

	Examples: 
	| description | expectation                           |
	| Click me!   | 'Click me!' is shown in the When step |
	| Or me!      | 'Or me!' is shown in the When step    |

Scenario: Markdown enhanced scenario summary
*Occasionally*, it is useful to **emphasize** certain words or phrases.

	Given the summary of this scenario is written in Markdown
	When I view this scenario in Augurk
	Then the summary of this scenario is rendered as rich-text

Scenario: Attempted **Markdown** in a scenario title
Markdown is not support in scenario and feature titles
	Given the title of this scenario is written in Markdown
	When I view this scenario in Augurk
	Then the title of this scenario is rendered as plain-text

Scenario: Fancy diacritics in the steps (e.g. é, à, ü)
Markdown is not support in scenario and feature titles, diacritics however, are.
  Given the title of this scenario is written in Markdown 
  When I view this scenario in Augurk 
  Then the title of this scenario is rendered as "Fancy diacritics in the steps (e.g. é, à, ü)" 
