
/*
*
*/

(function () {
    'use strict';
    angular.module('app.core').factory('main.enquiryTypes.svc', svc);

    svc.$inject = ['$httpParamSerializer', '$http', '$window', 'core.settings'];

    function svc($httpParamSerializer, $http, $window, settings) {
        return {
            search: search
        }

        function search(filter) {
            return $http.get(settings.apiUrl + '/enquiry-types/search/?' + $httpParamSerializer(filter));
        }
       
       
    }

})();
