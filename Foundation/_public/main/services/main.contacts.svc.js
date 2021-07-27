
/*
*
*/

(function () {
    'use strict';
    angular.module('app.main').factory('main.contacts.svc', svc);

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
            return $http.get(settings.apiUrl + '/contacts/');
        }

        function search(filter) {
            return $http.get(settings.apiUrl + '/contacts/search/?' + $httpParamSerializer(filter));
        }
        function get(id) {
            return $http.get(settings.apiUrl + '/contacts/' + id);
        }
        function create(model) {
            return $http.post(settings.apiUrl + '/contacts/', model);
        }
        function remove(id) {
            return $http.delete(settings.apiUrl + '/contacts/' + id);
        }
        function update(model) {
            return $http.put(settings.apiUrl + '/contacts/' + model.id, model);
        }
    }

})();
