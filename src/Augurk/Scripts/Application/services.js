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

AugurkServices.factory('featureDescriptionService', ['$resource', '$http', '$q', function ($resource, $http, $q) {

    var service = {};

    service.getFeaturesByProductVersionAndTag = function (productName, version, tag) {
        var tagPromiseDeferrer = $q.defer();
        $http({ method: 'GET', url: 'api/v2/products/' + productName + '/versions/' + version + '/tags/' + tag + '/features' }).then(function (response) {
            tagPromiseDeferrer.resolve(response.data);
        });

        return tagPromiseDeferrer.promise;
    }
    
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
            $rootScope.$broadcast('currentVersionChanged', { product: $routeParams.productName, version: service.currentVersion });
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

            $rootScope.$broadcast('currentVersionChanged', { product: $routeParams.productName, version: service.currentVersion });
        }
    });

    return service;
}]);

AugurkServices.factory('productService', ['$http', '$q', '$routeParams', '$rootScope', function ($http, $q, $routeParams, $rootScope) {

    // create the service
    var service = {
        currentProduct: null
    };

    // since AngularJS' $resource does not support primitive types, use $http instead.
    var productsPromiseDeferrer = $q.defer();
    $http({ method: 'GET', url: 'api/v2/products' }).then(function (response) {
        productsPromiseDeferrer.resolve(response.data);
    });

    service.products = productsPromiseDeferrer.promise;

    service.getVersions = function (productName) {
        var versionsPromiseDeferrer = $q.defer();
        $http({ method: 'GET', url: 'api/v2/products/' + productName + '/versions' }).then(function (response) {
            versionsPromiseDeferrer.resolve(response.data);
        });

        return versionsPromiseDeferrer.promise;
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

AugurkServices.factory('versionService', ['$http', '$q', '$routeParams', '$rootScope', function ($http, $q, $routeParams, $rootScope) {

    // create the service
    var service = {};

    service.getTagsForVersion = function (productName, version) {
        var tagsPromiseDeferrer = $q.defer();
        $http({ method: 'GET', url: 'api/v2/products/' + productName + '/versions/' + version + '/tags' }).then(function (response) {
            tagsPromiseDeferrer.resolve(response.data);
        });

        return tagsPromiseDeferrer.promise;
    }

    service.getGroupsByProductAndVersion = function (productName, version) {
        var groupsPromiseDeferrer = $q.defer();
        $http({ method: 'GET', url: 'api/v2/products/' + productName + '/versions/' + version + '/groups' }).then(function (response) {
            groupsPromiseDeferrer.resolve(response.data);
        });

        return groupsPromiseDeferrer.promise;
    }

    return service;
}]);