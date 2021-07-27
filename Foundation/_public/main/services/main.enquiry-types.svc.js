
/*
*
*/

(function () {
    'use strict';
    angular.module('app.core').factory('main.enquiryTypes.svc', svc);

    svc.$inject = ['$httpParamSerializer', '$http', '$window', 'core.settings'];

    function svc($httpParamSerializer, $http, $window, settings) {
        return {
            search: search,
            update: update,
            create: create,
            remove: remove
        }

        function search(filter) {
            return $http.get(settings.apiUrl + '/enquiry-types/?' + $httpParamSerializer(filter));
        }
        function create(model) {
            return $http.post(settings.apiUrl + '/enquiry-types/', model);
        }
        function remove(id) {
            return $http.delete(settings.apiUrl + '/enquiry-types/' + id);
        }
        function update(model) {
            return $http.put(settings.apiUrl + '/enquiry-types/' + model.id, model);
        }
       
    }

})();
