
/*
*
*/

(function () {
    'use strict';
    angular.module('app.core').factory('main.countries.svc', svc);

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
            return $http.get(settings.apiUrl + '/countries/?' + $httpParamSerializer(filter));
        }
        function create(model) {
            return $http.post(settings.apiUrl + '/countries/', model);
        }
        function remove(id) {
            return $http.delete(settings.apiUrl + '/countries/' + id);
        }
        function update(model) {
            return $http.put(settings.apiUrl + '/countries/' + model.id, model);
        }
        function toggleActive(id) {
            return $http.post(settings.apiUrl + '/countries/' + id + '/toggleactive');
        }
    }

})();
