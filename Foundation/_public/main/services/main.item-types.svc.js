
/*
*
*/

(function () {
    'use strict';
    angular.module('app.core').factory('main.itemTypes.svc', svc);

    svc.$inject = ['$httpParamSerializer', '$http', '$window', 'core.settings'];

    function svc($httpParamSerializer, $http, $window, settings) {
        return {
            search: search,
            update: update,
            create: create,
            remove: remove
        }

        function search(filter) {
            return $http.get(settings.apiUrl + '/item-types/?' + $httpParamSerializer(filter));
        }
        function create(model) {
            return $http.post(settings.apiUrl + '/item-types/', model);
        }
        function remove(id) {
            return $http.delete(settings.apiUrl + '/item-types/' + id);
        }
        function update(model) {
            return $http.put(settings.apiUrl + '/item-types/' + model.id, model);
        }
       
    }

})();
