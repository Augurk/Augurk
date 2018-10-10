@augurk @documentation
Feature: UML

Augurk supports the usage of UML within feature descriptions by parsing *UML code blocks* with an embedded [nomnoml](http://nomnoml.com) parser.
For instance, the following markdown will lead to the graph below it.

    ```UML
	Label: A simple Class Diagram
    [Foo]->[Bar]
    ```

```UML
Label: A simple Class Diagram
[Foo]->[Bar]
```

Of course, even complexer graphs can be created by using all that [nomnoml](http://nomnoml.com) has to offer, like this (slightly misused) state diagram:
```UML
\#direction: right
[<start>start]->[<input>markdown]
[markdown]->[parse markdown]
[markdown]->[resolve feature references]
[markdown]->[render UML]
[<state>parse markdown]->[<state>feature description]
[<state>resolve feature references]->[feature description]
[<state>render UML]->[feature description]
[feature description]->[<end>e]
[<note>What you're looking at]-->[feature description]
```

Scenario: Using directives
In order to use a #directive, it needs to be escaped
	Given the directive "#direction: right" is included in the UML block
	And the directive is preceded by a back-slash (\)
	When I open this feature in Augurk
	Then the UML block is rendered as a UML drawing

Scenario: Using labels
Providing a label in the first line of the UML block will render it in Augurk
	Given the first line in an UML block is "Label: A simple Class Diagram" 
	When I open this feature in Augurk
	Then the label "A simple Class Diagram" is rendered below the UML drawing