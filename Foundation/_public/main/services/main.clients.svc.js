
/*
*
*/

(function () {
    'use strict';
    angular.module('app.main').factory('main.clients.svc', svc);

    svc.$inject = ['$httpParamSerializer', '$http', '$window', 'core.settings'];

    function svc($httpParamSerializer, $http, $window, settings) {
        return {
            all: all,
            search: search,
            get: get,
            update: update,
            create: create,
            remove: remove

        }

        function all() {
            return $http.get(settings.apiUrl + '/clients/');
        }

        function search(filter) {
            return $http.get(settings.apiUrl + '/clients/search/?' + $httpParamSerializer(filter));
        }
        function get(id) {
            return $http.get(settings.apiUrl + '/clients/' + id);
        }
        function create(model) {
            return $http.post(settings.apiUrl + '/clients/', model);
        }
        function remove(id) {
            return $http.delete(settings.apiUrl + '/clients/' + id);
        }
        function update(model) {
            return $http.put(settings.apiUrl + '/clients/' + model.id, model);
        }
    }

})();
