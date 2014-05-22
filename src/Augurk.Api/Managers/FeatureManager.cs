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
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Augurk.Entities;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Augurk.Api.Managers
{
    /// <summary>
    /// Provides methods to persist and retrieve features from storage.
    /// </summary>
    public class FeatureManager
    {
        /// <summary>
        /// Gets or sets the JsonSerializerSettings that should be used when (de)serializing.
        /// </summary>
        internal static JsonSerializerSettings JsonSerializerSettings { get; set; }

        /// <summary>
        /// Gets the feature that matches the provided criteria.
        /// </summary>
        /// <param name="branchName">The name of the branch in which the feature exists.</param>
        /// <param name="groupName">The name of the group under which the feature is positioned.</param>
        /// <param name="title">The title of the feature.</param>
        /// <returns>
        /// A <see cref="DisplayableFeature"/> instance describing the requested feature; 
        /// or <c>null</c> if the feature cannot be found.
        /// </returns>
        public DisplayableFeature GetFeature(string branchName, string groupName, string title)
        {
            using (IDbConnection connection =new SqlConnection(ConfigurationManager.ConnectionStrings["FeatureStore"].ConnectionString))
            {
                connection.Open();
                using (IDbCommand command = connection.CreateCommand())
                {
                    command.CommandText = "GetFeature";
                    command.CommandType = CommandType.StoredProcedure;

                    var featureNameParameter = command.CreateParameter();
                    featureNameParameter.ParameterName = "title";
                    featureNameParameter.Value = title;
                    featureNameParameter.DbType = DbType.String;
                    command.Parameters.Add(featureNameParameter);

                    var branchNameParameter = command.CreateParameter();
                    branchNameParameter.ParameterName = "branchName";
                    branchNameParameter.Value = branchName;
                    branchNameParameter.DbType = DbType.String;
                    command.Parameters.Add(branchNameParameter);

                    using (IDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return DeserializeFeature(reader.GetString(0));
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Gets groups containing the descriptions for all features for the specified branch.
        /// </summary>
        /// <param name="branchName">The name of the branch for which the feature descriptions should be retrieved.</param>
        /// <returns>An enumerable collection of <see cref="FeatureDescription"/> instances.</returns>
        public IEnumerable<Group> GetGroupedFeatureDescriptions(string branchName)
        {
            Dictionary<string, List<FeatureDescription>> featureDescriptions = new Dictionary<string, List<FeatureDescription>>();
            Dictionary<string, Group> groups = new Dictionary<string, Group>();

            using (IDbConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["FeatureStore"].ConnectionString))
            {
                connection.Open();
                using (IDbCommand command = connection.CreateCommand())
                {
                    command.CommandText = "GetFeatureDescriptions";
                    command.CommandType = CommandType.StoredProcedure;

                    var branchNameParameter = command.CreateParameter();
                    branchNameParameter.ParameterName = "branchName";
                    branchNameParameter.Value = branchName;
                    branchNameParameter.DbType = DbType.String;
                    command.Parameters.Add(branchNameParameter);

                    using (IDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var featureDescription = new FeatureDescription()
                                {
                                    Title = reader.GetString(0),
                                };

                            string groupName = reader.GetString(1);

                            if (reader.IsDBNull(2))
                            {
                                if (!groups.ContainsKey(groupName))
                                {
                                    // Create a new group
                                    groups.Add(groupName, new Group()
                                        {
                                            Name=groupName,
                                            Features = new List<FeatureDescription>()
                                        });
                                }

                                // Add the feature to the group
                                ((List<FeatureDescription>) groups[groupName].Features).Add(featureDescription);
                            }
                            else
                            {
                                string parentTitle = reader.GetString(2);

                                if (!featureDescriptions.ContainsKey(parentTitle))
                                {
                                    featureDescriptions.Add(parentTitle, new List<FeatureDescription>());
                                }

                                featureDescriptions[parentTitle].Add(featureDescription);
                            }
                        }
                    }

                    // Map the lower levels
                    foreach (var feature in groups.Values.SelectMany(group => group.Features))
                    {
                        AddChildren(feature, featureDescriptions);
                    }
                }
            }

            return groups.Values.OrderBy(group => group.Name).ToList();
        }

        private void AddChildren(FeatureDescription feature, Dictionary<string, List<FeatureDescription>> childRepository)
        {
            if (childRepository.ContainsKey(feature.Title))
            {
                feature.ChildFeatures = childRepository[feature.Title];
                childRepository[feature.Title].ForEach(f => AddChildren(f, childRepository));
            }
        }

        public void InsertOrUpdateFeature(Feature feature, string branchName, string groupName)
        {
            var processor = new FeatureProcessor();
            string parentTitle = processor.DetermineParent(feature);

            using (IDbConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["FeatureStore"].ConnectionString))
            {
                connection.Open();
                using (IDbCommand command = connection.CreateCommand())
                {
                    command.CommandText = "InsertOrUpdateFeature";
                    command.CommandType = CommandType.StoredProcedure;

                    var titleParameter = command.CreateParameter();
                    titleParameter.ParameterName = "title";
                    titleParameter.Value = feature.Title;
                    titleParameter.DbType = DbType.String;
                    command.Parameters.Add(titleParameter);

                    var branchNameParameter = command.CreateParameter();
                    branchNameParameter.ParameterName = "branchName";
                    branchNameParameter.Value = branchName;
                    branchNameParameter.DbType = DbType.String;
                    command.Parameters.Add(branchNameParameter);

                    var groupNameParameter = command.CreateParameter();
                    groupNameParameter.ParameterName = "groupName";
                    groupNameParameter.Value = groupName;
                    groupNameParameter.DbType = DbType.String;
                    command.Parameters.Add(groupNameParameter);

                    var parentTitleParameter = command.CreateParameter();
                    parentTitleParameter.ParameterName = "parentTitle";
                    parentTitleParameter.Value = String.IsNullOrWhiteSpace(parentTitle) ? (object)DBNull.Value : parentTitle;
                    parentTitleParameter.DbType = DbType.String;
                    command.Parameters.Add(parentTitleParameter);

                    var serializedFeatureParameter = command.CreateParameter();
                    serializedFeatureParameter.ParameterName = "serializedFeature";
                    serializedFeatureParameter.Value = SerializeFeature(feature);
                    serializedFeatureParameter.DbType = DbType.String;
                    command.Parameters.Add(serializedFeatureParameter);

                    command.ExecuteNonQuery();
                }
            }
        }

        public void DeleteFeature(string branchName, string title)
        {
            using (IDbConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["FeatureStore"].ConnectionString))
            {
                connection.Open();
                using (IDbCommand command = connection.CreateCommand())
                {
                    command.CommandText = "DeleteFeature";
                    command.CommandType = CommandType.StoredProcedure;

                    var titleParameter = command.CreateParameter();
                    titleParameter.ParameterName = "title";
                    titleParameter.Value = title;
                    titleParameter.DbType = DbType.String;
                    command.Parameters.Add(titleParameter);

                    var branchNameParameter = command.CreateParameter();
                    branchNameParameter.ParameterName = "branchName";
                    branchNameParameter.Value = branchName;
                    branchNameParameter.DbType = DbType.String;
                    command.Parameters.Add(branchNameParameter);

                    command.ExecuteNonQuery();
                }
            }
        }

        public void DeleteFeatures(string branchName)
        {
            using (IDbConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["FeatureStore"].ConnectionString))
            {
                connection.Open();
                using (IDbCommand command = connection.CreateCommand())
                {
                    command.CommandText = "DeleteBranch";
                    command.CommandType = CommandType.StoredProcedure;

                    var branchNameParameter = command.CreateParameter();
                    branchNameParameter.ParameterName = "branchName";
                    branchNameParameter.Value = branchName;
                    branchNameParameter.DbType = DbType.String;
                    command.Parameters.Add(branchNameParameter);

                    command.ExecuteNonQuery();
                }
            }
        }

        public void DeleteFeatures(string branchName, string groupName)
        {
            using (IDbConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["FeatureStore"].ConnectionString))
            {
                connection.Open();
                using (IDbCommand command = connection.CreateCommand())
                {
                    command.CommandText = "DeleteGroup";
                    command.CommandType = CommandType.StoredProcedure;

                    var branchNameParameter = command.CreateParameter();
                    branchNameParameter.ParameterName = "branchName";
                    branchNameParameter.Value = branchName;
                    branchNameParameter.DbType = DbType.String;
                    command.Parameters.Add(branchNameParameter);

                    var groupNameParameter = command.CreateParameter();
                    groupNameParameter.ParameterName = "groupName";
                    groupNameParameter.Value = groupName;
                    groupNameParameter.DbType = DbType.String;
                    command.Parameters.Add(groupNameParameter);

                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Serializes the provided <see cref="Feature"/> to Json.
        /// </summary>
        private string SerializeFeature(Feature feature)
        {
            return JsonConvert.SerializeObject(feature, JsonSerializerSettings);
        }

        /// <summary>
        /// Deserializes the provided Json string to an <see cref="DisplayableFeature"/>.
        /// </summary>
        private DisplayableFeature DeserializeFeature(string serializedFeature)
        {
            return JsonConvert.DeserializeObject<DisplayableFeature>(serializedFeature, JsonSerializerSettings);
        }
    }
}