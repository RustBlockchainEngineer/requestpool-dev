
/*
*
*/

(function () {
    'use strict';
    angular.module('app.core').factory('main.cities.svc', svc);

    svc.$inject = ['$httpParamSerializer', '$http', '$window', 'core.settings'];

    function svc($httpParamSerializer, $http, $window, settings) {
        return {
            search: search,
            update: update,
            create: create,
            remove: remove,
            toggleActive: toggleActive
        }

        function search(filter) {
            return $http.get(settings.apiUrl + '/cities/?' + $httpParamSerializer(filter));
        }
        function create(model) {
            return $http.post(settings.apiUrl + '/cities/', model);
        }
        function remove(id) {
            return $http.delete(settings.apiUrl + '/cities/' + id);
        }
        function update(model) {
            return $http.put(settings.apiUrl + '/cities/' + model.id, model);
        }
        function toggleActive(id) {
            return $http.post(settings.apiUrl + '/cities/' + id + '/toggleactive');
        }
    }

})();
