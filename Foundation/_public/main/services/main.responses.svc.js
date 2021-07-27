
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
            submitResponse: submitResponse,
            properties: properties,
            downloadAttachments: downloadAttachments


        }

        function incoming(enquiryId) {
            return $http.get(settings.apiUrl + '/responses/incoming?enquiryId=' + enquiryId);
        }
        
        function outgoing(invitationId) {
            return $http.get(settings.apiUrl + '/responses/outgoing?invitationId=' + invitationId);
        }

        function submitResponse(model) {
            return $http.post(settings.apiUrl + '/responses/' + model.invitationId, model);
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
