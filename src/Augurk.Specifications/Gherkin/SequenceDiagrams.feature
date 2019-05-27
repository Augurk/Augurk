@augurk @documentation @childOf:UML
Feature: Sequence Diagrams

Within feature descriptions one can use sequence diagrams to provide additional context for a specific feature. This works by parsing *sequence diagram
code blocks* with an embedded [js-sequence-diagrams](https://bramp.github.io/js-sequence-diagrams/) parser. For example, the following markdown will lead
to the graph below it (note that the language name *sequence* is case-sensitive).

    ```sequence
	Label: A simple Sequence Diagram
    Foo->Bar: Hello
    ```
```sequence
Label: A simple Sequence Diagram
Foo->Bar: Hello
```

Using the syntax of js-sequence-diagrams we can also create more complex diagrams, such as this one:

```sequence
"Augurk-CLI"->Augurk: Check Version
Augurk-->"Augurk-CLI": 2.8.0
"Augurk-CLI"->Augurk: Upload Features
Augurk->Analyzer: Check for dependencies
Augurk-->"Augurk-CLI": HTTP201 (Created)
```

Scenario: Using labels
Providing a label in the first line of the Sequence Diagram block will render it in Augurk
	Given the first line in an Sequence Diagram block is "Label: A simple Sequence Diagram" 
	When I open this feature in Augurk
	Then the label "A simple Sequence Diagram" is rendered below the sequence diagram drawing