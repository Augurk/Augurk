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

angular.module('Augurk').controller('menuController', ['$rootScope', '$scope', '$routeParams', 'featureDescriptionService', 'productService',
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

        if (productService.currentProduct) {
            createMenu(productService.currentProduct);
        }

        $rootScope.$on('currentProductChanged', function (event, data) {
            createMenu(data.product);
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

