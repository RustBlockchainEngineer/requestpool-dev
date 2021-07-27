
/*
*
*/

(function () {
    'use strict';
    angular.module('app.main').factory('main.itemsProperties.svc', svc);

    svc.$inject = ['$httpParamSerializer', '$http', '$window', 'core.settings'];

    function svc($httpParamSerializer, $http, $window, settings) {
        return {
            search: search
        }

        function search(filter) {
            return $http.get(settings.apiUrl + '/items-properties/search/?' + $httpParamSerializer(filter));
        }
        
       
    }

})();
