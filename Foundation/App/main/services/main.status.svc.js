
/*
*
*/

(function () {
    'use strict';
    angular.module('app.main').factory('main.status.svc', svc);

    svc.$inject = ['$httpParamSerializer', '$http', '$window', 'core.settings'];

    function svc($httpParamSerializer, $http, $window, settings) {
        return {
            search: search,
            get: search,
            update: update,
            create: create,
            remove: remove,
            toggleActive: toggleActive,
            setDefault:setDefault
        }

        function search(filter) {
            return $http.get(settings.apiUrl + '/status/?' + $httpParamSerializer(filter));
        }
        function create(model) {
            return $http.post(settings.apiUrl + '/status/', model);
        }
        function remove(id) {
            return $http.delete(settings.apiUrl + '/status/' + id);
        }
        function update(model) {
            return $http.put(settings.apiUrl + '/status/' + model.id, model);
        }
        function toggleActive(id) {
            return $http.post(settings.apiUrl + '/status/' + id + '/toggleactive');
        }
        function setDefault(id) {
            return $http.post(settings.apiUrl + '/status/' + id + '/setdefault');
        }
    }

})();
