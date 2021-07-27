
/*
*
*/

(function () {
    'use strict';
    angular.module('app.main').factory('main.items.svc', svc);

    svc.$inject = ['$httpParamSerializer', '$http', '$window', 'core.settings'];

    function svc($httpParamSerializer, $http, $window, settings) {
        return {
            all: all,
            search: search,
            create: create,
            includedProperties: includedProperties,
            updateIncludedProperties: updateIncludedProperties
        }

        function all(enquiryId) {
            return $http.get(settings.apiUrl + '/items/' + enquiryId);
        }

        function search(filter) {
            return $http.get(settings.apiUrl + '/items/search/?' + $httpParamSerializer(filter));
        }
        function create(model) {
            return $http.post(settings.apiUrl + '/items/', model);
        }

        function includedProperties(enquiryId) {
            return $http.get(settings.apiUrl + '/items/included-properties/' + enquiryId);
        }
        function updateIncludedProperties(model) {
            return $http.put(settings.apiUrl + '/items/included-properties/' + model.enquiryId, model);
        }
    }

})();
