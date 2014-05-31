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
        $scope.mergeTestResults = function() {
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

AugurkControllers.controller('menuController', ['$rootScope', '$routeParams', 'featureDescriptionService', 'branchService',
    function ($rootScope, $routeParams, featureDescriptionService, branchService) {

        function createMenu(currentBranch) {
            featureDescriptionService.query(
                { branchName: currentBranch },
                function(groups) {
                    $rootScope.featureGroups = processFeatureGroups(currentBranch, groups);
                }
            );
        }
        
        if (branchService.currentBranch) {
            createMenu(branchService.currentBranch);
        }
        
        $rootScope.$on('currentBranchChanged', function (event, data) {
            createMenu(data.branch);
        });
    }
]);

AugurkControllers.controller('navbarController', ['$scope', 'branchService',
    function ($scope, branchService) {

        $scope.currentBranch = branchService.currentBranch;
        $scope.$on('currentBranchChanged', function(event, data) {
            $scope.currentBranch = data.branch;
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

