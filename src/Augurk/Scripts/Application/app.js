﻿/*
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

var Augurk = angular.module('Augurk', ['ngSanitize', 'ngResource', 'ngRoute', 'ui.select']);

Augurk.config(['$routeProvider', '$locationProvider',
function ($routeProvider, $locationProvider) {
    $locationProvider.html5Mode(false);
    $routeProvider
        .when('/home', {
            templateUrl: 'templates/home.html'
        })
        .when('/configuration', {
            templateUrl: 'templates/configuration.html'
         })
        .when('/home/:productName', {
            templateUrl: 'templates/home.html'
        })
        .when('/home/:productName/versions/:version', {
            templateUrl: 'templates/home.html'
        })
        .when('/feature/:productName/:groupName/:featureName/:version', {
            templateUrl: 'templates/feature.html',
            controller: 'featureController'
        })
        .otherwise({ redirectTo: '/home' });
}])