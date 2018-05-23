using Augurk.Api.Managers;
using Augurk.Entities;
using Augurk.Entities.Analysis;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Augurk.Api
{
    /// <summary>
    /// TODO
    /// </summary>
    public class Analyzer
    {
        private readonly FeatureManager _featureManager;
        private readonly AnalysisReportManager _analysisReportManager;

        public Analyzer(FeatureManager featureManager, AnalysisReportManager analysisReportManager)
        {
            _featureManager = featureManager;
            _analysisReportManager = analysisReportManager;
        }

        public async Task AnalyzeAndPersistResultsAsync(string productName, string version)
        {
            // Because we will analyze the entire product, remove the old results
            await _analysisReportManager.DeleteDbInvocationsAsync(productName, version);

            // Collect all features for the provided product/version combination
            IEnumerable<DbFeature> features = await _featureManager.GetDbFeaturesByProductAndVersionAsync(productName, version);

            // Collect all analysisreports for the provided product/version combination
            IEnumerable<AnalysisReport> reports = await _analysisReportManager.GetAnalysisReportsByProductAndVersionAsync(productName, version);

            if (reports.Any())
            {
                // Analyze the features based on the provided report
                await AnalyzeAndPersistAsync(productName, version, features, reports);
            }
        }

        public async Task AnalyzeAndPeristResultsAsync(string productName, string version, DbFeature feature)
        {
            // Collect all analysisreports for the provided product/version combination
            IEnumerable<AnalysisReport> reports = await _analysisReportManager.GetAnalysisReportsByProductAndVersionAsync(productName, version);

            if (reports.Any())
            {
                // Analyze the features based on the provided report
                await AnalyzeAndPersistAsync(productName, version, new[] { feature }, reports);
            }
        }

        private async Task AnalyzeAndPersistAsync(string productName, string version, IEnumerable<DbFeature> features, IEnumerable<AnalysisReport> reports)
        {
            // Collect the Wheresteps of the feature for easy access
            var featureStepMaps =
                from feature in features
                select new
                {
                    Feature = feature,
                    WhereSteps = feature.Scenarios.SelectMany(scenario =>
                        scenario.Steps.Where(step => step.StepKeyword == StepKeyword.When).Select(step => step.Content)).ToList()
                };

            var activeInvocations = new List<DbInvocation>();

            // Remove any old invocations in these features
            foreach(var feature in features)
            {
                feature.DirectInvocationSignatures = new List<string>();
            }

            // Determine the possible entrypoints
            foreach(var invocation in reports.SelectMany(report => report.RootInvocations).Where(ri => ri.RegularExpressions?.Length > 0))
            {
                // Collect the features which perform this invocation
                var invokingFeatures = new List<DbFeature>();
                foreach (var expression in invocation.RegularExpressions) {
                    invokingFeatures.AddRange(
                        featureStepMaps.Where(map => map.WhereSteps.Any(step => Regex.IsMatch(step, expression)))
                        .Select(map => map.Feature));
                }

                // Only process this invocation if it is actually used
                if (invokingFeatures.Any())
                {
                    // Update the features
                    var localInvocations = GetHighestLocalInvocations(invocation).ToList();
                    var localInvocationSignatures = localInvocations.Select(i => i.Signature).ToList();
                    invokingFeatures.ForEach(feature => feature.DirectInvocationSignatures.AddRange(localInvocationSignatures));

                    // Prepare the invocation for storage
                    activeInvocations.AddRange(localInvocations.SelectMany(i =>
                    {
                        List<DbInvocation> dbInvocations = new List<DbInvocation>();
                        string[] invokedSignatures = Flatten(i);

                        // Add the current invocation
                        dbInvocations.Add(new DbInvocation()
                        {
                            Signature = i.Signature,
                            InvokedSignatures = invokedSignatures
                        });

                        // Add a copy for each interface definition
                        if (i.InterfaceDefinitions != null)
                        {
                            dbInvocations.AddRange(i.InterfaceDefinitions.Select(definition =>
                                new DbInvocation()
                                {
                                    Signature = definition,
                                    InvokedSignatures = invokedSignatures
                                }
                            ));
                        }

                        return dbInvocations;
                    }));
                }
            }

            // Make sure we only save unique signatures
            foreach (var feature in features)
            {
                feature.DirectInvocationSignatures = new List<string>(feature.DirectInvocationSignatures.Distinct());
            }

            // Persist the features
            await _featureManager.PersistDbFeatures(features);

            // Persist the activeInvocations
            await _analysisReportManager.PersistDbInvocationsAsync(productName, version, activeInvocations.GroupBy(i => i.Signature).Select(g => g.First()));
        }

        private string[] Flatten(Invocation invocation)
        {
            List<string> invocations = new List<string>();
            foreach (var inv in invocation.Invocations ?? new Invocation[0])
            {
                Flatten(inv, invocations);
            }
            return invocations.ToArray();
        }

        private void Flatten(Invocation invocation, List<string> invocations)
        {
            invocations.Add(invocation.Signature);
            foreach (var inv in invocation.Invocations ?? new Invocation[0])
            {
                Flatten(inv, invocations);
            }
        }

        private IEnumerable<Invocation> GetHighestLocalInvocations(Invocation invocation)
        {
            if (invocation.AutomationTargets != null)
            {
                return invocation.AutomationTargets.SelectMany(target =>
                    invocation.Invocations.Descendants(i => i.Invocations).Where(i => i.Signature == target));
            }

            List<Invocation> invocations = new List<Invocation>();
            foreach (var childInvocation in invocation.Invocations ?? new Invocation[0])
            {
                if (childInvocation.Local)
                {
                    invocations.Add(childInvocation);
                }
                else
                {
                    invocations.AddRange(GetHighestLocalInvocations(childInvocation));
                }
            }
            return invocations;
        }
    }
}
