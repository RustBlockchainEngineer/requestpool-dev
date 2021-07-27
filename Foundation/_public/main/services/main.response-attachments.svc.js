
/*
*
*/

(function () {
    'use strict';
    angular.module('app.main').factory('main.responseAttachments.svc', svc);

    svc.$inject = ['$httpParamSerializer', '$http', '$window', 'core.settings'];

    function svc($httpParamSerializer, $http, $window, settings) {
        return {
            get: get,
            update: update,
            create: create,
            remove: remove
        }

        function get(invitationId) {
            return $http.get(`${settings.apiUrl}/response-attachments/${invitationId}`);
        }
        function create(invitationId,model) {
            return $http.post(`${settings.apiUrl}/response-attachments/${invitationId}`, model);
        }
        function remove(invitationId, id) {
            return $http.delete(`${settings.apiUrl}/response-attachments/${invitationId}/${id}`);
        }
        function update(invitationId,id,model) {
            return $http.put(`${settings.apiUrl}/response-attachments/${invitationId}/${id}`, model);
        }
    }

})();
