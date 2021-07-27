
/*
*
*/

(function () {
    'use strict';
    angular.module('app.main').factory('main.invitations.svc', svc);

    svc.$inject = ['$httpParamSerializer', '$http', '$window', 'core.settings'];

    function svc($httpParamSerializer, $http, $window, settings) {
        return {
            searchOutgoing: searchOutgoing,
            outgoingById: outgoingById,
            searchIncoming: searchIncoming,
            incomingById: incomingById,
            attachments: attachments

        }

       
        function searchOutgoing(filter) {
            return $http.get(settings.apiUrl + '/invitations/outgoing/search?' + $httpParamSerializer(filter));
        }
        function outgoingById(id) {
            return $http.get(settings.apiUrl + '/invitations/outgoing/' + id);
        }

        function searchIncoming(filter) {
            return $http.get(settings.apiUrl + '/invitations/incoming/search?' + $httpParamSerializer(filter));
        }
        function incomingById(id) {
            return $http.get(settings.apiUrl + '/invitations/incoming/' + id);
        }
        function attachments(id) {
            return $http.get(settings.apiUrl + '/invitations/attachments/' + id);
        }
    }

})();
