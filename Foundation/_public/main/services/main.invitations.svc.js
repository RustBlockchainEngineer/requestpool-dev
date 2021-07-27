
/*
*
*/

(function () {
    'use strict';
    angular.module('app.main').factory('main.invitations.svc', svc);

    svc.$inject = ['$httpParamSerializer', '$http', '$window', 'core.settings'];

    function svc($httpParamSerializer, $http, $window, settings) {
        return {
            outgoing: outgoing,
            searchOutgoing: searchOutgoing,
            outgoingById: outgoingById,
            incoming: incoming,
            searchIncoming: searchIncoming,
            incomingById: incomingById,
            update: update,
            create: create,
            remove: remove,
            attachments: attachments

        }

        function outgoing(enquiryId) {
            return $http.get(settings.apiUrl + '/invitations/outgoing?enquiryId=' + enquiryId);
        }

        function searchOutgoing(filter) {
            return $http.get(settings.apiUrl + '/invitations/outgoing/search?' + $httpParamSerializer(filter));
        }
        function outgoingById(id) {
            return $http.get(settings.apiUrl + '/invitations/outgoing/' + id);
        }

        function incoming(filter) {
            return $http.get(settings.apiUrl + '/invitations/incoming/?'+ $httpParamSerializer(filter));
        }

        function searchIncoming(filter) {
            return $http.get(settings.apiUrl + '/invitations/incoming/search?' + $httpParamSerializer(filter));
        }
        function incomingById(id) {
            return $http.get(settings.apiUrl + '/invitations/incoming/' + id);
        }
        function create(model) {
            return $http.post(settings.apiUrl + '/invitations/', model);
        }
        function remove(id) {
            return $http.delete(settings.apiUrl + '/invitations/' + id);
        }
        function update(model) {
            return $http.put(settings.apiUrl + '/invitations/' + model.id, model);
        }
        function attachments(id) {
            return $http.get(settings.apiUrl + '/invitations/attachments/' + id);
        }
    }

})();
