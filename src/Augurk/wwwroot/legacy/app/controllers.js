/*
 Copyright 2014-2017, Augurk

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

AugurkControllers.controller('homeController', ['$rootScope',
    function ($rootScope) {
        $rootScope.allowMenu = false;
    }
]);


AugurkControllers.controller('productController', ['$rootScope', '$scope', '$routeParams', 'productService',
    function ($rootScope, $scope, $routeParams, productService) {
        $rootScope.allowMenu = true;

        productService.getDescription($routeParams.productName).then(function (productDescription) {
            $scope.productDescription = productDescription;
            $scope.showProductName = productDescription ? productDescription[0] !== "#" : true;
        });

        $scope.productName = $routeParams.productName;
    }
]);

AugurkControllers.controller('featureController', ['$rootScope', '$scope', '$routeParams', 'featureService', 'featureVersionService', 'featureDependencyService', 'configurationService',
    function ($rootScope, $scope, $routeParams, featureService, featureVersionService, featureDependencyService, configurationService) {
        $rootScope.allowMenu = true;

        $scope.feature = featureService.get({
            productName: $routeParams.productName,
            groupName: $routeParams.groupName,
            featureName: $routeParams.featureName,
            version: $routeParams.version
        });

        configurationService.get().$promise.then(function (configuration) {
            $scope.configuration = configuration;

            if (configuration.dependenciesEnabled) {
                $scope.dependencies = featureDependencyService.get({
                    productName: $routeParams.productName,
                    featureName: $routeParams.featureName,
                    version: $routeParams.version
                });
            }
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

        $scope.fillExample = function($event, keys, values){
            var placeholders = $($event.currentTarget).closest('.scenario').find('span.argument');
            placeholders.each(function(index){
                var placeholder = $(this);
                var index = keys.indexOf(placeholder.data('name'));
                if(index > -1)
                {
                    placeholder.html(values[index]);
                }
            });
        }
    }
]);

AugurkControllers.controller('menuController', ['$rootScope', '$scope', '$routeParams', 'featureDescriptionService', 'productService',
    function ($rootScope, $scope, $routeParams, featureDescriptionService, productService) {

        function createMenu(currentProduct) {
            featureDescriptionService.getGroupsByProduct(currentProduct,
                function (groups) {
                    $rootScope.featureGroups = processFeatureGroups(currentProduct, groups);
                }
            );

            productService.getTags(currentProduct).then(function (tags) {
                $scope.tags = $.makeArray();
	            $.each(tags, function(index, tag) {
		            $scope.tags.push({ name: tag, features: $.makeArray() });
	            });
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
                    productService.currentProduct,
                    tag.name,
                    function (features) {
                        tag.features = features;
                    }
                );
            },
            matchFeature: function (featureName) {
                if ($scope.filter.tags.length === 0) {
                    return true;
                }

                var result = false;

                $.each($scope.filter.tags, function (index, tag) {
                    if (tag.features.some(function (feature) { return feature.title === featureName; })) {
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

        if (productService.currentProduct) {
            createMenu(productService.currentProduct);
        }

        $rootScope.$on('currentProductChanged', function (event, data) {
            createMenu(data.product);
        });
    }
]);

AugurkControllers.controller('navbarController', ['$rootScope', '$scope', 'productService', 'customizationService',
    function ($rootScope, $scope, productService, customizationService) {
            customizationService.get().$promise.then(function(customization) {
                setCustomization($rootScope, customization);
            });


        $scope.currentProduct = productService.currentProduct;
        $scope.$on('currentProductChanged', function (event, data) {
            $rootScope.currentProduct = data.product;
        });

        productService.products.then(function (products) {
            $scope.products = products.sort();
        });

    }
]);

AugurkControllers.controller('configurationController', ['$rootScope', '$scope', 'customizationService', 'configurationService', 'versionService', '$http',
    function($rootScope, $scope, customizationService, configurationService, versionService, $http) {
        $rootScope.allowMenu = false;

        customizationService.get().$promise.then(function (customization) {
            $scope.customizationSettings = customization;
            $scope.saveCustomization = function () {
                $scope.customizationSettings.$save();
                setCustomization($rootScope, $scope.customizationSettings);
            };
        });

        configurationService.get().$promise.then(function(configuration) {
            $scope.configuration = configuration;
            $scope.saveConfiguration = function () {
                $scope.configuration.$save();
            };
        });

        versionService.get().then(function (version) {
            $scope.version = version;
        });

        $scope.import = function () {
            var element = document.getElementById("importFile");
            var file = element.files[0];

            var formData = new FormData();
            formData.append("filename", file.name);
            formData.append("file", file);

            var ajaxRequest = $.ajax({
                cache: false,
                type: 'POST',
                url: '/api/v2/import',
                contentType: false,
                processData: false,
                data: formData
            });

            ajaxRequest.done(function () {
                alert("Import succesful. Please refresh the page.");
            }).fail(function () {
                alert("Import unsuccesful.");
            });
        };

        $scope.export = function () {
            window.open('/api/v2/export', '_blank', '');
        };
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

function setCustomization($rootScope, customization) {
    $rootScope.instanceName = customization.instanceName;
}

