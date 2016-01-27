/*
 Copyright 2014-2016, Mark Taling
 
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

angular.module('Augurk').controller('featureController', ['$rootScope', '$scope', '$routeParams', 'featureService', 'featureVersionService',
    function ($rootScope, $scope, $routeParams, featureService, featureVersionService) {
        $scope.feature = featureService.get({
            productName: $routeParams.productName,
            groupName: $routeParams.groupName,
            featureName: $routeParams.featureName,
            version: $routeParams.version
        });

	    featureVersionService.versions.then(function(data) {
		    $scope.availableVersions = data;
	    });

        $rootScope.$on('currentVersionChanged', function (event, data) {
	        featureVersionService.versions.then(function(data) {
		        $scope.availableVersions = data;
	        });
        });

	    $scope.product = $routeParams.productName;
	    $scope.group = $routeParams.groupName;

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