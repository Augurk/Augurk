Augurk
======

Augurk is a work-in-progress living documentation system created especially to be used 
in conjunction with [SpecFlow](http://www.specflow.org) and 
[Visual Studio Team Foundation Server](http://msdn.microsoft.com/en-us/vstudio/ff637362.aspx).

Augurk provides an accessible, easy-to-use overview of all the features describing your application(s).

##Inspiration##
Augurk is greatly influenced by [Relish](https://www.relishapp.com/)
and [Pickles](http://www.picklesdoc.com/). While Relish is a great SaaS application,
it requires you to publish your documentation to the cloud. While this is not an issue when your sources live in the cloud as well,
in other cases it is often still preferable to keep the documentation within the corporate intranet.<br />
This leaves Pickles. Although Pickles adresses the cloud issue (it was designed for that purpose), it has
some short-comings of its own. Pickles generates a neat (Dynamic)HTML website for each project/solution you
require it to. It does, however, not incorporate these. This means not only will you get a website for each solution,
you can multiply your number of websites with the number of branches you maintain simultaneously.

Augurk has been designed to address exactly these issues. It allows you to publish your
features from multiple solutions and branches to a single location, providing you with a single access-point
for all your documentation. While Augurk is based upon the same principles as Pickles, it has 
been rebuild completely from the ground-up, allowing Augurk to provide all the same functionality but simultenously
improving and extending it where necessary.

###Augurk?###
The <em>Gherkin</em> language is used to write <em>Cucumber</em> specifications. 
Seeing as a <em>Gherkin</em> is a pickled cucumber, the naming of <em>Pickles</em>
and <em>Relish</em> (a pickled food item) make perfect sense. Since that doesn't leave much
<em>Gherkin</em> related terms in the English language, it was an almost natural choice to switch
to a different language. Since <em>Codename Augurk</em> is being developed in The Netherlands, 
<em>Augurk</em> (meaning pickle[d cucumber]) was a perfect fit.

##ToDo##

+ Description on Github page
+ Proper starting page
+ Proper branch page
+ Separate MSBuild task to publish testresults
+ Command line tool to publish features and testresults
+ Maintenance pages (Remove branches, declare server side tags)