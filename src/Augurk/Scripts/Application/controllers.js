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

AugurkControllers.controller('featureController', ['$rootScope', '$scope', '$routeParams', 'featureService', 'featureVersionService',
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

AugurkControllers.controller('menuController', ['$rootScope', '$scope', '$routeParams', 'featureDescriptionService', 'productService', 'versionService',
    function ($rootScope, $scope, $routeParams, featureDescriptionService, productService, versionService) {
        function createMenu(currentProduct, currentVersion) {
            versionService.getTagsForVersion(currentProduct, currentVersion).then(function (tags) {
                $scope.tags = $.makeArray();
                $.each(tags, function (index, tag) {
                    $scope.tags.push({ name: tag, features: $.makeArray() });
                });
                $scope.filter.tags = $.makeArray();
            });

            versionService.getGroupsByProductAndVersion(currentProduct, currentVersion).then(
                function (groups) {
                    $rootScope.featureGroups = processFeatureGroups(currentProduct, groups);
                }
            );
        }

        $scope.tags = $.makeArray();
        $scope.versions = $.makeArray();

        $scope.filter = {
            tags: $.makeArray(),
            run: function () {
                if ($scope.filter.tags.length == 0) {
                    return;
                }

                var tag = $scope.filter.tags[$scope.filter.tags.length - 1];
                featureDescriptionService.getFeaturesByProductVersionAndTag(
                    productService.currentProduct,
                    $scope.currentVersion,
                    tag.name)
                    .then(function (features) {
                        tag.features = features;
                    });
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

	                return true;
                });

                return result;
            }
        };

        // Load the versions for the selected product
        $rootScope.$on('currentProductChanged', function (event, data) {
            productService.getVersions(data.product).then(function (versions) {
                $scope.availableVersions = versions;
                // Load the first version
                $scope.currentVersion = versions[0];
                $rootScope.$broadcast('currentVersionChanged', { product: data.product, version: $scope.currentVersion });
            });
        });

        // Update the version in the menu
        $rootScope.$on('$routeChangeSuccess', function () {
            if ($routeParams.version && $routeParams.version != $scope.currentVersion) {
                $scope.currentVersion = $routeParams.version;
                $rootScope.$broadcast('currentVersionChanged', { product: $routeParams.productName, version: $routeParams.version });
            }
        });

        // Create the menu for the selected product and version
        $rootScope.$on('currentVersionChanged', function (event, data) {
            // Clear the selected tags before reloading the menu
            $scope.filter.tags = $.makeArray();
            createMenu(data.product, data.version);
        });
    }
]);

AugurkControllers.controller('navbarController', ['$rootScope', '$scope', 'productService',
    function ($rootScope, $scope, productService) {

        $scope.currentProduct = productService.currentProduct;
        $scope.$on('currentProductChanged', function (event, data) {
            $rootScope.currentProduct = data.product;
        });

        productService.products.then(function (products) {
            $scope.products = products.sort();
        });
    }
]);

function processFeatureGroups(productName, groups) {
    $.each(groups, function (index, group) {
        group.features = getFlatList(productName, group.name, group.features, 1, null);
    });

    return groups;
}

function getFlatList(productName, groupName, featureTree, level, parentTitle) {
    var featureList = $.makeArray();
    $.each(featureTree, function (index, feature) {
        featureList.push({
            title: feature.title,
            level: level,
            parentTitle: parentTitle,
            hasChildren: (feature.childFeatures && feature.childFeatures.length > 0),
            uri: '#feature/' + productName + '/' + groupName + '/' + feature.title + '/' + feature.latestVersion
        });

        if (feature.childFeatures) {
            $.merge(featureList, getFlatList(productName, groupName, feature.childFeatures, Math.min(level + 1, 5), feature.title));
        }
    });

    return featureList;
}

