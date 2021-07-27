
/*
*
*/

(function () {
    'use strict';
    angular.module('app.main').factory('main.itemsProperties.svc', svc);

    svc.$inject = ['$httpParamSerializer', '$http', '$window', 'core.settings'];

    function svc($httpParamSerializer, $http, $window, settings) {
        return {
            search: search,
            update: update,
            create: create,
            remove: remove
        }

        function search(filter) {
            return $http.get(settings.apiUrl + '/items-properties/?' + $httpParamSerializer(filter));
        }
        function create(model) {
            return $http.post(settings.apiUrl + '/items-properties/', model);
        }
        function remove(id) {
            return $http.delete(settings.apiUrl + '/items-properties/' + id);
        }
        function update(model) {
            return $http.put(settings.apiUrl + '/items-properties/' + model.id, model);
        }
       
    }

})();
