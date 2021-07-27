
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
            search: search,
            get: get,
            getByUser: getByUser,
            update: update,
            create: create,
            remove: remove,
            current:current

        }

        function all() {
            return $http.get(settings.apiUrl + '/membership/');
        }

        function search(filter) {
            return $http.get(settings.apiUrl + '/membership/search/?' + $httpParamSerializer(filter));
        }
        function get(id) {
            return $http.get(settings.apiUrl + '/membership/' + id);
        }
        function getByUser(id) {
            return $http.get(settings.apiUrl + '/membership/user/' + id);
        }
        function create(model) {
            return $http.post(settings.apiUrl + '/membership/', model);
        }
        function remove(id) {
            return $http.delete(settings.apiUrl + '/membership/' + id);
        }
        function update(model) {
            return $http.put(settings.apiUrl + '/membership/' + model.id, model);
        }

        function current(id) {
            return $http.get(settings.apiUrl + '/membership/current/' + id);
        }
    }

})();
