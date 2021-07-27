
/*
*
*/

(function () {
    'use strict';
    angular.module('app.main').factory('main.responses.svc', svc);

    svc.$inject = ['$httpParamSerializer', '$http', '$window', 'core.settings'];

    function svc($httpParamSerializer, $http, $window, settings) {
        return {
            incoming: incoming,
            outgoing: outgoing,
            properties: properties,
            downloadAttachments: downloadAttachments

        }

        function incoming(enquiryId) {
            return $http.get(settings.apiUrl + '/responses/incoming?enquiryId=' + enquiryId);
        }
        
        function outgoing(invitationId, publicUserId) {
            return $http.get(settings.apiUrl + '/responses/outgoing/' + invitationId + '/' + publicUserId);
        }
        
        function properties(invitationId) {
            return $http.get(settings.apiUrl + '/responses/properties/' + invitationId);
        }

        function downloadAttachments(recipients) {            
            return $http.get(`${settings.apiUrl}/responses/attachments?recipients=${recipients.map(i => i.toString()).join(',')}`,
                { responseType: 'arraybuffer' });
        }
    }

})();
