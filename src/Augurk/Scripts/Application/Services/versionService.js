/*
 Copyright 2016, Mark Taling
 
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

angular.module('Augurk').factory('versionService', ['$http', '$q', function ($http, $q) {

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