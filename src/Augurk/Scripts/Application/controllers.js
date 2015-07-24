/*
 Copyright 2014-2015, Mark Taling
 
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

var AugurkControllers = angular.module('AugurkControllers', ['AugurkServices']);

AugurkControllers.controller('featureController', ['$rootScope', '$scope', '$routeParams', 'featureService',
    function ($rootScope, $scope, $routeParams, featureService) {
        $scope.feature = featureService.get({
            branchName: $routeParams.branchName,
            groupName: $routeParams.groupName,
            featureName: $routeParams.featureName
        });

        // Set the current group on the rootscope
        $rootScope.currentGroupName = $routeParams.groupName;

        // Define a function to merge the test results into the feature
        $scope.mergeTestResults = function () {
            if (!$scope.feature.testResult) {
                return;
            }

            $.each($scope.feature.testResult.scenarioTestResults, function (index, testResult) {
                $.each($scope.feature.scenarios, function (index1, scenario) {
                    //var scenario = $scope.feature.scenarios.filter(function (s) { return s.title = testResult.scenarioTitle; })[0];
                    scenario.testResult = testResult;
                });
                //var scenario = $scope.feature.scenarios.filter(function (s) { return s.title = testResult.scenarioTitle; })[0];
                //scenario.testResult = testResult;
            });

            $scope.feature.testResult.merged = true;
        };
    }
]);

AugurkControllers.controller('menuController', ['$rootScope', '$scope', '$routeParams', 'featureDescriptionService', 'branchService',
    function ($rootScope, $scope, $routeParams, featureDescriptionService, branchService) {

        function createMenu(currentBranch) {
            featureDescriptionService.getFeaturesByBranch(currentBranch,
                function (groups) {
                    $rootScope.featureGroups = processFeatureGroups(currentBranch, groups);
                }
            );

            branchService.getTags(currentBranch).then(function (tags) {
                $scope.tags = $.makeArray();
                $.each(tags, function (index, tag) {
                    $scope.tags.push({ name: tag, features: $.makeArray() })
                })
                $scope.filter.tags = $.makeArray();
            });
        }

        $scope.tags = $.makeArray();

        $scope.filter = {
            tags: $.makeArray(),
            run: function () {
                if ($scope.filter.tags.length == 0) {
                    return;
                }

                var tag = $scope.filter.tags[$scope.filter.tags.length - 1];
                featureDescriptionService.getFeaturesByBranchAndTag(
                    branchService.currentBranch,
                    tag.name,
                    function (features) {
                        tag.features = features;
                    }
                );
            },
            matchFeature: function (featureName) {
                if ($scope.filter.tags.length == 0) {
                    return true;
                }

                var result = false;
                
                $.each($scope.filter.tags, function (index, tag) {
                    if (tag.features.some(function (feature) { return feature.title == featureName; })) {
                        // Set the value
                        result = true;
                        // Break out of the loop
                        return false;
                    }
                });

                return result;
            }
        };

        if (branchService.currentBranch) {
            createMenu(branchService.currentBranch);
        }

        $rootScope.$on('currentBranchChanged', function (event, data) {
            createMenu(data.branch);
        });
    }
]);

AugurkControllers.controller('navbarController', ['$rootScope', '$scope', 'branchService',
    function ($rootScope, $scope, branchService) {

        $scope.currentBranch = branchService.currentBranch;
        $scope.$on('currentBranchChanged', function (event, data) {
            $rootScope.currentBranch = data.branch;
        });

        branchService.branches.then(function (branches) {
            $scope.branches = branches;
        });
    }
]);

function processFeatureGroups(branchName, groups) {
    $.each(groups, function (index, group) {
        group.features = getFlatList(branchName, group.name, group.features, 1, null);
    });

    return groups;
}

function getFlatList(branchName, groupName, featureTree, level, parentTitle) {
    var featureList = $.makeArray();
    $.each(featureTree, function (index, feature) {
        featureList.push({
            title: feature.title,
            level: level,
            parentTitle: parentTitle,
            hasChildren: (feature.childFeatures && feature.childFeatures.length > 0),
            uri: '#feature/' + branchName + '/' + groupName + '/' + feature.title
        });

        if (feature.childFeatures) {
            $.merge(featureList, getFlatList(branchName, groupName, feature.childFeatures, Math.min(level + 1, 5), feature.title));
        }
    });

    return featureList;
}

