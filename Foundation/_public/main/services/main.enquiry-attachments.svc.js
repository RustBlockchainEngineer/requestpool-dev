
/*
*
*/

(function () {
    'use strict';
    angular.module('app.main').factory('main.enquiryAttachments.svc', svc);

    svc.$inject = ['$httpParamSerializer', '$http', '$window', 'core.settings'];

    function svc($httpParamSerializer, $http, $window, settings) {
        return {
            get: get,
            update: update,
            create: create,
            remove: remove

        }

        function get(enquiryId) {
            return $http.get(settings.apiUrl + '/enquiries-attachments/' + enquiryId);
        }
        function create(model) {
            return $http.post(settings.apiUrl + '/enquiries-attachments/', model);
        }
        function remove(id) {
            return $http.delete(settings.apiUrl + '/enquiries-attachments/' + id);
        }
        function update(model) {
            return $http.put(settings.apiUrl + '/enquiries-attachments/' + model.id, model);
        }
    }

})();
