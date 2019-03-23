﻿// ------------------------------------------------------------------------------
//  <auto-generated>
//      This code was generated by SpecFlow (http://www.specflow.org/).
//      SpecFlow Version:2.4.0.0
//      SpecFlow Generator Version:2.4.0.0
// 
//      Changes to this file may cause incorrect behavior and will be lost if
//      the code is regenerated.
//  </auto-generated>
// ------------------------------------------------------------------------------
#region Designer generated code
#pragma warning disable
namespace Augurk.Specifications.Gherkin
{
    using TechTalk.SpecFlow;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "2.4.0.0")]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [Microsoft.VisualStudio.TestTools.UnitTesting.TestClassAttribute()]
    [Microsoft.VisualStudio.TestTools.UnitTesting.IgnoreAttribute()]
    public partial class NotYetImplementedFeaturesFeature
    {
        
        private static TechTalk.SpecFlow.ITestRunner testRunner;
        
        private Microsoft.VisualStudio.TestTools.UnitTesting.TestContext _testContext;
        
#line 1 "NotImplementedTag.feature"
#line hidden
        
        public virtual Microsoft.VisualStudio.TestTools.UnitTesting.TestContext TestContext
        {
            get
            {
                return this._testContext;
            }
            set
            {
                this._testContext = value;
            }
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.ClassInitializeAttribute()]
        public static void FeatureSetup(Microsoft.VisualStudio.TestTools.UnitTesting.TestContext testContext)
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner(null, 0);
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "Not (yet) implemented features", @"In order to indicate that a feature has not been implemented yet, it can be decorated with a notImplemented tag.

**The not implemented warning is only displayed for demo purposes, the fact that it is shown indicates, in fact, that this feature has been implemented.**", ProgrammingLanguage.CSharp, new string[] {
                        "childOf:CustomizationthroughTags",
                        "notImplemented",
                        "ignore",
                        "augurk",
                        "documentation"});
            testRunner.OnFeatureStart(featureInfo);
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.ClassCleanupAttribute()]
        public static void FeatureTearDown()
        {
            testRunner.OnFeatureEnd();
            testRunner = null;
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestInitializeAttribute()]
        public virtual void TestInitialize()
        {
            if (((testRunner.FeatureContext != null) 
                        && (testRunner.FeatureContext.FeatureInfo.Title != "Not (yet) implemented features")))
            {
                global::Augurk.Specifications.Gherkin.NotYetImplementedFeaturesFeature.FeatureSetup(null);
            }
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestCleanupAttribute()]
        public virtual void ScenarioTearDown()
        {
            testRunner.OnScenarioEnd();
        }
        
        public virtual void ScenarioInitialize(TechTalk.SpecFlow.ScenarioInfo scenarioInfo)
        {
            testRunner.OnScenarioInitialize(scenarioInfo);
            testRunner.ScenarioContext.ScenarioContainer.RegisterInstanceAs<Microsoft.VisualStudio.TestTools.UnitTesting.TestContext>(_testContext);
        }
        
        public virtual void ScenarioStart()
        {
            testRunner.OnScenarioStart();
        }
        
        public virtual void ScenarioCleanup()
        {
            testRunner.CollectScenarioErrors();
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("Decorating this feature with a notImplemented tag")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "Not (yet) implemented features")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestCategoryAttribute("childOf:CustomizationthroughTags")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestCategoryAttribute("notImplemented")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestCategoryAttribute("augurk")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestCategoryAttribute("documentation")]
        public virtual void DecoratingThisFeatureWithANotImplementedTag()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Decorating this feature with a notImplemented tag", "Decorating a feature with a notImplemented tag will result in a warning being dis" +
                    "played,\r\nindicating that this feature has not yet been implemented.", ((string[])(null)));
#line 7
this.ScenarioInitialize(scenarioInfo);
            this.ScenarioStart();
#line 10
 testRunner.Given("the tag \"notImplemented\" is placed on this feature", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 11
 testRunner.When("I open this feature in Augurk", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 12
 testRunner.Then("\"notImplemented\" is not displayed as part of the tags on the feature", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 13
 testRunner.And("a yellow box containing a warning is dispayed below the title of this feature", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("Decorating this feature with a notImplemented and an ignore tag")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "Not (yet) implemented features")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestCategoryAttribute("childOf:CustomizationthroughTags")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestCategoryAttribute("notImplemented")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestCategoryAttribute("augurk")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestCategoryAttribute("documentation")]
        public virtual void DecoratingThisFeatureWithANotImplementedAndAnIgnoreTag()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Decorating this feature with a notImplemented and an ignore tag", "Decorating a feature with a notImplemented tag in addition to a ignore tag will r" +
                    "esult in the ignore tag being disregarded.", ((string[])(null)));
#line 15
this.ScenarioInitialize(scenarioInfo);
            this.ScenarioStart();
#line 17
 testRunner.Given("the tag \"notImplemented\" is placed on this feature", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 18
 testRunner.And("the tag \"ignore\" is placed on this feature", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 19
 testRunner.When("I open this feature in Augurk", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 20
 testRunner.Then("\"notImplemented\" is not displayed as part of the tags on the feature", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 21
 testRunner.And("\"ignore\" is not displayed as part of the tags on the feature", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 22
 testRunner.And("a yellow box containing a warning is dispayed below the title of this feature", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 23
 testRunner.And("no red box is dispayed below the title of this feature", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("Decorating this scenario with a notImplemented tag")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "Not (yet) implemented features")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestCategoryAttribute("childOf:CustomizationthroughTags")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestCategoryAttribute("notImplemented")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestCategoryAttribute("augurk")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestCategoryAttribute("documentation")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestCategoryAttribute("notImplemented")]
        public virtual void DecoratingThisScenarioWithANotImplementedTag()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Decorating this scenario with a notImplemented tag", "Decorating a scenario with a notImplemented tag does nothing whatsoever.", new string[] {
                        "notImplemented"});
#line 26
this.ScenarioInitialize(scenarioInfo);
            this.ScenarioStart();
#line 28
 testRunner.Given("the tag \"notImplemented\" is placed on this scenario", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 29
 testRunner.When("I find this scenario in Augurk", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 30
 testRunner.Then("\"notImplemented\" is displayed as part of the tags on the scenario", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
    }
}
#pragma warning restore
#endregion