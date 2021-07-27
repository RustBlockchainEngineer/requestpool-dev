
/*
*
*/

(function () {
    'use strict';
    angular.module('app.main').factory('main.membership.svc', svc);

    svc.$inject = ['$httpParamSerializer', '$http', '$window', 'core.settings'];

    function svc($httpParamSerializer, $http, $window, settings) {
        return {
            all: all,
            get: get,
            current: current,
            consumption: consumption,
            upgrade: upgrade,
            renew: renew

        }

        function all() {
            return $http.get(settings.apiUrl + '/membership/');
        }
        function get(id) {
            return $http.get(settings.apiUrl + '/membership/' + id);
        }
        function current() {
            return $http.get(settings.apiUrl + '/membership/current');
        }

        function consumption() {
            return $http.get(settings.apiUrl + '/membership/consumption');
        }

        function upgrade(model) {
            return $http.post(settings.apiUrl + '/membership/upgrade', model);
        }
        function renew(model) {
            return $http.post(settings.apiUrl + '/membership/renew', model);
        }

    }

})();
