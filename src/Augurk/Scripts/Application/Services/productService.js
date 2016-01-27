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

angular.module('Augurk').factory('productService', ['$http', '$q', '$routeParams', '$rootScope', function ($http, $q, $routeParams, $rootScope) {

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
        else if (!$routeParams.productName) {
            $rootScope.$broadcast('noProductInScope');
        }

    });

    return service;
}]);