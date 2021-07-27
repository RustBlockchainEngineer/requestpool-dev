
/*
*
*/

(function () {
    'use strict';
    angular.module('app.main').factory('main.status.svc', svc);

    svc.$inject = ['$httpParamSerializer', '$http', '$window', 'core.settings'];

    function svc($httpParamSerializer, $http, $window, settings) {
        return {
            get: get
        }

        function get() {
            return $http.get(settings.apiUrl + '/status/');
        }
    }

})();
