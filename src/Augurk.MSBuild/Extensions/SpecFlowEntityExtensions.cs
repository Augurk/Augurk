/*
 Copyright 2014, Mark Taling
 
 Licensed under the Apache License, Version 2.0 (the "License");
 you may not use this file except in compliance with the License.
 You may obtain a copy of the License at
 
 http://www.apache.org/licenses/LICENSE-2.0
 
 Unless required by applicable law or agreed to in writing, software
 distributed under the License is distributed on an "AS IS" BASIS,
 WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 See the License for the specific language governing permissions and
 limitations under the License.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using SpecFlow = TechTalk.SpecFlow.Parser.SyntaxElements;
using Gherkin = TechTalk.SpecFlow.Parser.Gherkin;

namespace Augurk.Entities
{
    /// <summary>
    /// Contains extension methods to transform entities from the <see cref="TechTalk.SpecFlow.Parser"/> namespace 
    /// into entities from the <see cref="Augurk.Entities"/> namespace.
    /// </summary>
    public static class SpecFlowEntityExtensions
    {
        /// <summary>
        /// Converts the provided <see cref="SpecFlow.Feature"/> instance into a <see cref="Augurk.Entities.Feature"/> instance.
        /// </summary>
        /// <param name="feature">The <see cref="SpecFlow.Feature"/> instance that should be converted.</param>
        /// <returns>The converted <see cref="Augurk.Entities.Feature"/> instance.</returns>
        public static Feature ConvertToFeature(this SpecFlow.Feature feature)
        {
            if (feature == null)
            {
                throw new ArgumentNullException("feature");
            }

            return new Feature()
                {
                    Title = feature.Title,
                    Description = feature.Description,
                    Tags = feature.Tags.ConvertToStrings(),
                    Scenarios = feature.Scenarios.Select(scenario => scenario.ConvertToScenario()).ToArray(),
                    Background = feature.Background.ConvertToBackground()
                };
        }

        /// <summary>
        /// Converts the provided <see cref="SpecFlow.Tags"/> instance into an enumerable collection of strings.
        /// </summary>
        /// <param name="tags">The <see cref="SpecFlow.Tags"/> instance that should be converted.</param>
        /// <returns>An enumerable collection of strings.</returns>
        public static IEnumerable<string> ConvertToStrings(this SpecFlow.Tags tags)
        {
            if (tags == null)
            {
                return new string[0];
            }

            return tags.Select(t => t.Name).ToArray();
        }

        /// <summary>
        /// Converts the provided <see cref="SpecFlow.Scenario"/> instance into a <see cref="Augurk.Entities.Scenario"/> instance.
        /// </summary>
        /// <param name="scenario">The <see cref="SpecFlow.Scenario"/> instance that should be converted.</param>
        /// <returns>The converted <see cref="Augurk.Entities.Scenario"/> instance.</returns>
        public static Scenario ConvertToScenario(this SpecFlow.Scenario scenario)
        {
            if (scenario == null)
            {
                throw new ArgumentNullException("scenario");
            }

            SpecFlow.ScenarioOutline outline = scenario as SpecFlow.ScenarioOutline;
            if (outline != null)
            {
                return outline.ConvertToScenario();
            }

            return new Scenario()
                {
                    Title = scenario.Title,
                    Description = scenario.Description,
                    Tags = scenario.Tags.ConvertToStrings(),
                    Steps = scenario.Steps.ConvertToSteps()
                };
        }

        /// <summary>
        /// Converts the provided <see cref="SpecFlow.ScenarioOutline"/> instance into a <see cref="Augurk.Entities.Scenario"/> instance.
        /// </summary>
        /// <param name="scenarioOutline">The <see cref="SpecFlow.ScenarioOutline"/> instance that should be converted.</param>
        /// <returns>The converted <see cref="Augurk.Entities.Scenario"/> instance.</returns>
        public static Scenario ConvertToScenario(this SpecFlow.ScenarioOutline scenarioOutline)
        {
            if (scenarioOutline == null)
            {
                throw new ArgumentNullException("scenarioOutline");
            }

            return new Scenario()
                {
                    Title = scenarioOutline.Title,
                    Description = scenarioOutline.Description,
                    Tags = scenarioOutline.Tags.ConvertToStrings(),
                    Steps = scenarioOutline.Steps.ConvertToSteps(),
                    ExampleSets = scenarioOutline.Examples.ConvertToExampleSets()
                };
        }

        /// <summary>
        /// Converts the provided <see cref="SpecFlow.Background"/> instance into a <see cref="Augurk.Entities.Background"/> instance.
        /// </summary>
        /// <param name="background">The <see cref="SpecFlow.Background"/> instance that should be converted.</param>
        /// <returns>The converted <see cref="Augurk.Entities.Background"/> instance.</returns>
        public static Background ConvertToBackground(this SpecFlow.Background background)
        {
            if (background == null)
            {
                return null;
            }

            return new Background()
            {
                Title = background.Title,
                Keyword = background.Keyword,
                Steps = background.Steps.ConvertToSteps()
            };
        }

        /// <summary>
        /// Converts the provided <see cref="SpecFlow.Examples"/> instance into an enumerable collection of <see cref="Augurk.Entities.ExampleSet"/> instances.
        /// </summary>
        /// <param name="examples">The <see cref="SpecFlow.Examples"/> instance that should be converted.</param>
        /// <returns>An enumerable collection of <see cref="Augurk.Entities.ExampleSet"/> instances.</returns>
        public static IEnumerable<ExampleSet> ConvertToExampleSets(this SpecFlow.Examples examples)
        {
            if (examples == null)
            {
                throw new ArgumentNullException("examples");
            }

            return examples.ExampleSets.Select(exampleSet => exampleSet.ConvertToExampleSet()).ToArray();
        }

        /// <summary>
        /// Converts the provided <see cref="SpecFlow.ExampleSet"/> instance into a<see cref="Augurk.Entities.ExampleSet"/> instance.
        /// </summary>
        /// <param name="exampleSet">The <see cref="SpecFlow.ExampleSet"/> instance that should be converted.</param>
        /// <returns>The converted <see cref="Augurk.Entities.ExampleSet"/> instance.</returns>
        public static ExampleSet ConvertToExampleSet(this SpecFlow.ExampleSet exampleSet)
        {
            if (exampleSet == null)
            {
                throw new ArgumentNullException("exampleSet");
            }

            return new ExampleSet()
                {
                    Title = exampleSet.Title,
                    Description = exampleSet.Description,
                    Keyword = exampleSet.Keyword,
                    Tags = exampleSet.Tags.ConvertToStrings(),
                    Columns = exampleSet.Table.Header.Cells.Select(cell => cell.Value).ToArray(),
                    Rows = exampleSet.Table.Body.Select(row => row.Cells.Select(cell => cell.Value).ToArray()).ToArray()
                };
        } 

        /// <summary>
        /// Converts the provided <see cref="SpecFlow.ScenarioSteps"/> instance into an enumerable collection of <see cref="Augurk.Entities.Step"/> instances.
        /// </summary>
        /// <param name="steps">The <see cref="SpecFlow.ScenarioSteps"/> instance that should be converted.</param>
        /// <returns>An enumerable collection of <see cref="Augurk.Entities.Step"/> instances.</returns>
        public static IEnumerable<Step> ConvertToSteps(this SpecFlow.ScenarioSteps steps)
        {
            if (steps == null)
            {
                return new Step[0];
            }

            return steps.Select(step => step.ConvertToStep()).ToArray();
        }

        /// <summary>
        /// Converts the provided <see cref="SpecFlow.ScenarioStep"/> instance into a <see cref="Augurk.Entities.Step"/> instance.
        /// </summary>
        /// <param name="step">The <see cref="SpecFlow.ScenarioStep"/> instance that should be converted.</param>
        /// <returns>The converted <see cref="Augurk.Entities.Step"/> instance.</returns>
        public static Step ConvertToStep(this SpecFlow.ScenarioStep step)
        {
            if (step == null)
            {
                throw new ArgumentNullException("step");
            }

            return new Step()
                {
                    BlockKeyword = step.ScenarioBlock.ConvertToBlockKeyword(),
                    StepKeyword = step.StepKeyword.ConvertToStepKeyword(),
                    Keyword = step.Keyword,
                    Content = step.Text,
                    TableArgument = step.TableArg.ConvertToTable()
                };
        }

        /// <summary>
        /// Converts the provided <see cref="Gherkin.ScenarioBlock"/> into a <see cref="Augurk.Entities.BlockKeyword"/>.
        /// </summary>
        /// <param name="scenarioBlock">The <see cref="Gherkin.ScenarioBlock"/> that should be converted.</param>
        /// <returns>The converted <see cref="Augurk.Entities.BlockKeyword"/>.</returns>
        public static BlockKeyword ConvertToBlockKeyword(this Gherkin.ScenarioBlock scenarioBlock)
        {
            switch (scenarioBlock)
            {
                case Gherkin.ScenarioBlock.Given:
                    return BlockKeyword.Given;
                case Gherkin.ScenarioBlock.Then:
                    return BlockKeyword.Then;
                case Gherkin.ScenarioBlock.When:
                    return BlockKeyword.When;
                default:
                    return BlockKeyword.None;
            }
        }

        /// <summary>
        /// Converts the provided <see cref="Gherkin.StepKeyword"/> into a <see cref="Augurk.Entities.StepKeyword"/>.
        /// </summary>
        /// <param name="stepKeyword">The <see cref="Gherkin.StepKeyword"/> that should be converted.</param>
        /// <returns>The converted <see cref="Augurk.Entities.StepKeyword"/>.</returns>
        public static StepKeyword ConvertToStepKeyword(this Gherkin.StepKeyword stepKeyword)
        {
            switch (stepKeyword)
            {
                case Gherkin.StepKeyword.Given:
                    return StepKeyword.Given;
                case Gherkin.StepKeyword.Then:
                    return StepKeyword.Then;
                case Gherkin.StepKeyword.When:
                    return StepKeyword.When;
                    case Gherkin.StepKeyword.And:
                    return StepKeyword.And;
                default:
                    return StepKeyword.None;
            }
        }

        /// <summary>
        /// Converts the provided <see cref="SpecFlow.GherkinTable"/> instance into a <see cref="Augurk.Entities.Table"/> instance.
        /// </summary>
        /// <param name="gherkinTable">The <see cref="SpecFlow.GherkinTable"/> instance that should be converted.</param>
        /// <returns>The converted <see cref="Augurk.Entities.Table"/> instance.</returns>
        public static Table ConvertToTable(this SpecFlow.GherkinTable gherkinTable)
        {
            if (gherkinTable == null)
            {
                return null;
            }

            return new Table
                {
                    Columns = gherkinTable.Header.Cells.Select(cell => cell.Value).ToArray(),
                    Rows = gherkinTable.Body.Select(row => row.Cells.Select(cell => cell.Value).ToArray()).ToArray()
                };
        }
    }
}
