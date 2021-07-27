
/*
*
*/

(function () {
    'use strict';
    angular.module('app.main').factory('main.projects.svc', svc);

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
            return $http.get(settings.apiUrl + '/projects/');
        }

        function search(filter) {
            return $http.get(settings.apiUrl + '/projects/search/?' + $httpParamSerializer(filter));
        }
        function get(id) {
            return $http.get(settings.apiUrl + '/projects/' + id);
        }
        function create(model) {
            return $http.post(settings.apiUrl + '/projects/', model);
        }
        function remove(id) {
            return $http.delete(settings.apiUrl + '/projects/' + id);
        }
        function update(model) {
            return $http.put(settings.apiUrl + '/projects/' + model.id, model);
        }
    }

})();
