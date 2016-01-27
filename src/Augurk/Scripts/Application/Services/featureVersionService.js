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

angular.module('Augurk').factory('featureVersionService', ['$http', '$q', '$routeParams', '$rootScope', function ($http, $q, $routeParams, $rootScope) {
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