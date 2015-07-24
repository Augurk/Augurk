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
    return $resource('api/features/:branchName/:groupName/:featureName\\',
                     { branchName: '@branchName', groupName: '@groupName', featureName: '@featureName' });
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
        }
    };
    
    return service;
}]);

AugurkServices.factory('branchService', ['$http', '$q', '$routeParams', '$rootScope', function ($http, $q, $routeParams, $rootScope) {

    // create the service
    var service = {
        branches: null,
        currentBranch: null
    };

    // since AngularJS' $resource does not support primitive types, use $http instead.
    var branchesPromiseDeferrer = $q.defer();
    $http({ method: 'GET', url: 'api/branches' }).then(function (response) {
        branchesPromiseDeferrer.resolve(response.data);
    });

    service.branches = branchesPromiseDeferrer.promise;

    service.getTags = function (branch) {
        var tagsPromiseDeferrer = $q.defer();
        $http({ method: 'GET', url: 'api/tags/' + branch }).then(function (response) {
            tagsPromiseDeferrer.resolve(response.data);
        });

        return tagsPromiseDeferrer.promise;
    }

    // set the current branch
    if ($routeParams.branchName) {
        service.currentBranch = $routeParams.branchName;
    }
    else {
        branchesPromiseDeferrer.promise.then(function (branches) {
            service.currentBranch = branches[0];
            $rootScope.$broadcast('currentBranchChanged', { branch: service.currentBranch });
        });
    }

    // update the branch on navigation
    $rootScope.$on('$routeChangeSuccess', function () {
        if ($routeParams.branchName &&
            $routeParams.branchName != service.currentBranch) {
            service.currentBranch = $routeParams.branchName;
            $rootScope.$broadcast('currentBranchChanged', { branch: service.currentBranch });
        }
    });

    return service;
}]);