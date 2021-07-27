
/*
*
*/

(function () {
    'use strict';
    angular.module('app.core').factory('core.administrators.svc', svc);

    svc.$inject = ['$httpParamSerializer', '$http', '$window', 'core.settings'];

    function svc($httpParamSerializer, $http, $window, settings) {
        return {
            all:all,
            search: search,
            get:get,
            update: update,
            create: create,
            remove: remove,
            setPassword: setPassword,
            updatePassword: updatePassword,
            toggleActive: toggleActive,
            getAllRoles: getAllRoles
        }

        function all() {
            return $http.get(settings.apiUrl + '/administrators/');
        }

        function search(filter) {
            return $http.get(settings.apiUrl + '/administrators/search/?' + $httpParamSerializer(filter));
        }
        function get(id) {
            return $http.get(settings.apiUrl + '/administrators/' + id);
        }
        function create(model) {
            return $http.post(settings.apiUrl + '/administrators/', model);
        }
        function remove(id) {
            return $http.delete(settings.apiUrl + '/administrators/' + id);
        }
        function update(model) {
            return $http.put(settings.apiUrl + '/administrators/' + model.id, model);
        }
        function setPassword(model) {
            return $http.post(settings.apiUrl + '/administrators/' + model.id+'/password', model);
        }
        function updatePassword(model) {
            return $http.put(settings.apiUrl + '/administrators/' + model.id + '/password', model);
        }
        function toggleActive(id) {
            return $http.post(settings.apiUrl + '/administrators/' + id + '/toggleactive');
        }
        function getAllRoles() {
            return $http.get(settings.apiUrl + '/roles/');
        }
    }

})();
