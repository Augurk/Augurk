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

var AugurkServices = angular.module('AugurkServices', ['ngResource', 'ngRoute']);

AugurkServices.factory('featureService', ['$resource', function ($resource) {
    // The featurename might contain a period, which webapi only allows if you finish with a slash
    // Since AngularJS doesn't allow for trailing slashes, use a backslash instead
    return $resource('api/v2/products/:productName/groups/:groupName/features/:featureName/versions/:version\\',
                     { productName: '@productName', groupName: '@groupName', featureName: '@featureName', version: '@version' });
}]);

AugurkServices.factory('featureDescriptionService', ['$resource', function ($resource) {

    // The branchname might contain a period, which webapi only allows if you finish with a slash
    // Since AngularJS doesn't allow for trailing slashes, use a backslash instead
    var service = {
        getFeaturesByBranch: function (branch, callback) {
            $resource('api/features/:branchName\\', { branchName: '@branchName' })
                .query({ branchName: branch }, callback);
        },

        getFeaturesByBranchAndTag: function (branch, tag, callback) {
            $resource('api/tags/:branchName/:tag/features', { branchName: '@branchName', tag: '@tag' })
                .query({ branchName: branch, tag: tag }, callback);
        },

        getGroupsByProduct: function (product, callback) {
            $resource('api/v2/products/:productName/groups', { productName: '@productName' })
                .query({ productName: product }, callback);
        }
    };
    
    return service;
}]);

AugurkServices.factory('featureVersionService', ['$http', '$q', '$routeParams', '$rootScope', function ($http, $q, $routeParams, $rootScope) {
    // create the service
    var service = {
        versions: null,
        currentVersion: null
    };

    var versionsPromiseDeferrer = $q.defer();
    $http({ method: 'GET', url: 'api/v2/products/' + $routeParams.productName + '/groups/' + $routeParams.groupName + '/features/' + $routeParams.featureName + '/versions' }).then(function (response) {
        versionsPromiseDeferrer.resolve(response.data);
    });

    service.versions = versionsPromiseDeferrer.promise;

    // set the current version
    if ($routeParams.version) {
        service.currentVersion = $routeParams.version;
    } else {
        versionsPromiseDeferrer.promise.then(function (versions) {
            service.currentVersion = versions[0];
            $rootScope.$broadcast('currentVersionChanged', { version: service.currentVersion });
        });
    }

    // update the version on navigation
    $rootScope.$on('$routeChangeSuccess', function () {
        if ($routeParams.version && $routeParams.version != service.currentVersion) {
            service.currentVersion = $routeParams.version;

            var versionsPromiseDeferrer = $q.defer();
            $http({ method: 'GET', url: 'api/v2/products/' + $routeParams.productName + '/groups/' + $routeParams.groupName + '/features/' + $routeParams.featureName + '/versions' }).then(function (response) {
                versionsPromiseDeferrer.resolve(response.data);
            });

            service.versions = versionsPromiseDeferrer.promise;

            $rootScope.$broadcast('currentVersionChanged', { product: service.currentVersion });
        }
    });

    return service;
}]);

AugurkServices.factory('productService', ['$http', '$q', '$routeParams', '$rootScope', function ($http, $q, $routeParams, $rootScope) {

    // create the service
    var service = {
        products: null,
        currentProduct: null
    };

    // since AngularJS' $resource does not support primitive types, use $http instead.
    var productsPromiseDeferrer = $q.defer();
    $http({ method: 'GET', url: 'api/v2/products' }).then(function (response) {
        productsPromiseDeferrer.resolve(response.data);
    });

    service.products = productsPromiseDeferrer.promise;

    service.getTags = function (productName) {
        var tagsPromiseDeferrer = $q.defer();
        $http({ method: 'GET', url: 'api/v2/products/' + productName + '/tags' }).then(function (response) {
            tagsPromiseDeferrer.resolve(response.data);
        });

        return tagsPromiseDeferrer.promise;
    }

    service.getDescription = function (productName) {
        var descriptionPromiseDeferrer = $q.defer();
        $http({ method: 'GET', url: 'api/v2/products/' + productName + '/description' }).then(function (response) {
            descriptionPromiseDeferrer.resolve(response.data);
        });

        return descriptionPromiseDeferrer.promise;
    }

    // set the current product
    if ($routeParams.productName) {
        service.currentProduct = $routeParams.productName;
    }
    else {
        productsPromiseDeferrer.promise.then(function (products) {
            service.currentProduct = products[0];
            $rootScope.$broadcast('currentProductChanged', { product: service.currentProduct });
        });
    }

    // update the product on navigation
    $rootScope.$on('$routeChangeSuccess', function () {
        if ($routeParams.productName &&
            $routeParams.productName != service.currentProduct) {
            service.currentProduct = $routeParams.productName;
            $rootScope.$broadcast('currentProductChanged', { product: service.currentProduct });
        }
    });

    return service;
}]);