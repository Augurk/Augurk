﻿// Copyright (c) Augurk. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using Augurk.Api.Managers;
using Augurk.Entities;
using Augurk.Entities.Analysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Augurk.Api
{
    /// <summary>
    /// Processes analysis reports together with their associated features in order to discover connections between them.
    /// </summary>
    public class Analyzer
    {
        private readonly IFeatureManager _featureManager;
        private readonly IAnalysisReportManager _analysisReportManager;

        public Analyzer(IFeatureManager featureManager, IAnalysisReportManager analysisReportManager)
        {
            _featureManager = featureManager;
            _analysisReportManager = analysisReportManager;
        }

        /// <summary>
        /// Processes all analysis reports and features for the provided <paramref name="productName">product</paramref> at the provided <paramref name="version"/>
        /// and stores the results of the analysis.
        /// </summary>
        /// <param name="productName">Name of the product to process.</param>
        /// <param name="version">Version of the product to process.</param>
        public async Task AnalyzeAndPersistResultsAsync(string productName, string version)
        {
            // Because we will analyze the entire product, remove the old results
            await _analysisReportManager.DeleteDbInvocationsAsync(productName, version);

            // Collect all features for the provided product/version combination
            var features = await _featureManager.GetDbFeaturesByProductAndVersionAsync(productName, version);

            // Collect all analysisreports for the provided product/version combination
            var reports = _analysisReportManager.GetAnalysisReportsByProductAndVersionAsync(productName, version);

            if (reports.Any())
            {
                // Analyze the features based on the provided report
                await AnalyzeAndPersistAsync(productName, version, features, reports);
            }
        }

        /// <summary>
        /// Processes the provided <paramref name="feature"/> after it has been uploaded and stores the results of the analysis.
        /// </summary>
        /// <param name="productName">Name of the product containing the feature.</param>
        /// <param name="version">Version of the porudct containing the feature.</param>
        /// <param name="feature">A <see cref="DbFeature"/> representing the feature to process.</param>
        public async Task AnalyzeAndPeristResultsAsync(string productName, string version, DbFeature feature)
        {
            // Collect all analysisreports for the provided product/version combination
            var reports = _analysisReportManager.GetAnalysisReportsByProductAndVersionAsync(productName, version);

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
            foreach (var feature in features)
            {
                feature.DirectInvocationSignatures = new List<string>();
            }

            var invocationsToPostProcess = new List<(DbInvocation dbInvocation, Invocation invocationToFlatten)>();

            // Determine the possible entrypoints
            foreach (var invocation in reports.SelectMany(report => report.RootInvocations).Where(ri => ri.RegularExpressions?.Length > 0))
            {
                // Collect the features which perform this invocation
                var invokingFeatures = new List<DbFeature>();
                foreach (var expression in invocation.RegularExpressions)
                {
                    invokingFeatures.AddRange(
                        featureStepMaps.Where(map => map.WhereSteps.Any(step => Regex.IsMatch(step, expression)))
                        .Select(map => map.Feature));
                }

                // Only process this invocation if it is actually used
                if (invokingFeatures.Any())
                {
                    // Update the features
                    var localInvocations = GetHighestLocalInvocations(invocation).ToList();
                    var localInvocationSignatures = localInvocations.SelectMany(i =>
                    {
                        var invocations = new List<string> { i.Signature };
                        // Add the interface definitions, if available
                        if (i.InterfaceDefinitions != null)
                        {
                            invocations.AddRange(i.InterfaceDefinitions);
                        }
                        return invocations;
                    }).ToList();
                    invokingFeatures.ForEach(feature => feature.DirectInvocationSignatures.AddRange(localInvocationSignatures));

                    // Prepare the invocation for storage
                    activeInvocations.AddRange(localInvocations.SelectMany(i =>
                    {
                        var dbInvocations = new List<DbInvocation>();

                        // Add the current invocation
                        var dbInvocation = new DbInvocation()
                        {
                            Signature = i.Signature
                        };
                        dbInvocations.Add(dbInvocation);
                        invocationsToPostProcess.Add((dbInvocation, i));

                        // Add a copy for each interface definition
                        if (i.InterfaceDefinitions != null)
                        {
                            var dbis = i.InterfaceDefinitions.Select(definition =>
                                new DbInvocation()
                                {
                                    Signature = definition
                                }
                            ).ToList();
                            dbInvocations.AddRange(dbis);
                            dbis.ForEach(dbi => invocationsToPostProcess.Add((dbi, i)));
                        }

                        return dbInvocations;
                    }));
                }
            }

            var strippedDbInvocations = activeInvocations.Select(ai => ai.Signature).ToList();
            // Flatten the invocations
            foreach (var invocation in invocationsToPostProcess)
            {
                invocation.dbInvocation.InvokedSignatures = FlattenUntilAMatchIsFound(invocation.invocationToFlatten, strippedDbInvocations);
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

        private string[] FlattenUntilAMatchIsFound(Invocation invocation, IEnumerable<string> possibleMatches)
        {
            var invocations = new List<string>();
            foreach (var inv in invocation.Invocations ?? Array.Empty<Invocation>())
            {
                FlattenUntilAMatchIsFound(inv, invocations, possibleMatches);
            }
            return invocations.ToArray();
        }

        private void FlattenUntilAMatchIsFound(Invocation invocation, List<string> invocations, IEnumerable<string> possibleMatches)
        {
            invocations.Add(invocation.Signature);
            if (!possibleMatches.Contains(invocation.Signature))
            {
                foreach (var inv in invocation.Invocations ?? Array.Empty<Invocation>())
                {
                    FlattenUntilAMatchIsFound(inv, invocations, possibleMatches);
                }
            }
        }

        private IEnumerable<Invocation> GetHighestLocalInvocations(Invocation invocation)
        {
            if (invocation.AutomationTargets != null)
            {
                return invocation.AutomationTargets.SelectMany(target =>
                    invocation.Invocations.Descendants(i => i.Invocations).Where(i => i.Signature == target));
            }

            var invocations = new List<Invocation>();
            foreach (var childInvocation in invocation.Invocations ?? Array.Empty<Invocation>())
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
