
/*
*
*/

(function () {
    'use strict';
    angular.module('app.main').factory('main.propertyTypes.svc', svc);

    svc.$inject = ['$httpParamSerializer', '$http', '$window', 'core.settings'];

    function svc($httpParamSerializer, $http, $window, settings) {
        return {
            all: all
        }

        function all() {
            return $http.get(settings.apiUrl + '/property-types/');
        }
       
    }

})();
