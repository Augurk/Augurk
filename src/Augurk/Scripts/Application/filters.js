/*
 Copyright 2014, Mark Taling
 
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

var AugurkFilters = angular.module('AugurkFilters', ['AugurkServices']);

// markdown filter
// ---------------
// Runs all input through the Showdown script, effectively applying markdown.
AugurkFilters.filter('markdown', function() {
    return function (input) {
        if (!input) {
            return "";
        }
        var converter = new showdown.Converter({ 'tables': true });               
        var html = converter.makeHtml(input);
        html = html.replace("<table>", '<table class="table table-bordered table-condensed table-striped">');        

        return html;
    };
});

// featurereferences filter
// ------------------------
// Replaces all featurenames between brackets with links to their feature page.
// e.g. [Feature1] will be replaced with <a href="correctLink">Feature1</a>
AugurkFilters.filter('featurereferences', ['$rootScope', function ($rootScope) {
    return function (input) {
        return input.replace(/\[([\w\s]*)\]/gm,
            function (originalContent, featureTitle) {
                if ($rootScope.featureGroups) {
                    var group = $.grep($rootScope.featureGroups, function (group) { return group.name == $rootScope.currentGroupName; })[0];
                    var featureDescriptions = $.grep(group.features, function (feature) { return feature.title == featureTitle; });
                    if (featureDescriptions.length == 1) {
                        return $('<a/>', {
                            html: featureDescriptions[0].title,
                            href: featureDescriptions[0].uri
                        })[0].outerHTML;
                    }
                }
		        return originalContent;
		    }
	    );
    };
}]);

// exampleparameters filter
// ------------------------
// Marks all text found between angle brackets as an example-parameter and marks the whole as an argument.
// e.g. <some text> will be replaced with <span class="argument">&lt;<span class="example-parameter">some text</span>&gt;</span>
AugurkFilters.filter('exampleparameters', function () {
    return function (input) {
        return input.replace(/\<([\w\d\s]*)\>/gm,
		    function (entireString, match) {
		            return $('<span/>', {
		                html: '&lt;' + $('<span/>', {
		                    html: match,
		                    'class': 'example-parameter'
		                })[0].outerHTML + '&gt;',
		                'class': 'argument'
		            })[0].outerHTML;
		    }
	    );
    };
});

// exampleparameters filter
// ------------------------
// Marks all text found between angle brackets as an example-parameter and marks the whole as an argument.
// e.g. <some text> will be replaced with <span class="argument">&lt;<span class="example-parameter">some text</span>&gt;</span>
AugurkFilters.filter('uml', ['$sce', 'uniqueIntegerService', function ($sce, uniqueIntegerService) {
    return function (input) {
        return $sce.trustAsHtml(input.replace(/\<pre><code class=\"UML language-UML\"\>(?:Label\:\W?(.*$))?([\s\S]*?)<\/code><\/pre>/gm,
            function (entireString, title, match) {
                var id = 'uml' + uniqueIntegerService.getNextInteger().toString();
                var result = `<div class="uml">
<canvas id="` + id + `"></canvas>
<script>
var canvas = document.getElementById("` + id + `");
var source = \`` + _.unescape(match) + `\`;
nomnoml.draw(canvas, source);</script></div>`;
                if (title) {
                    result += `<span class="uml-label">` + title + `</span>`;
                }

                return result;
            }
        ));
    };
}]);

function findFeature(featureDescriptions, title) {
    var result = null;

    $.each(featureDescriptions, function (index, featureDescription) {
        if (featureDescription.name == title) {
            result = featureDescription;
            return false; // break;
        }
        
        if (featureDescription.childFeatures) {
            result = findFeature(featureDescription.childFeatures, title);
            if (result) {
                return false; // break;
            }
        }

        return true; //continue
    });

    return result;
}