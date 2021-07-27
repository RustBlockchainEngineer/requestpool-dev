
/*
*
*/

(function () {
    'use strict';
    angular.module('app.main').factory('main.responseAttachments.svc', svc);

    svc.$inject = ['$httpParamSerializer', '$http', '$window', 'core.settings'];

    function svc($httpParamSerializer, $http, $window, settings) {
        return {
            get: get
        }

        function get(recipientId) {
            return $http.get(`${settings.apiUrl}/response-attachments/${recipientId}`);
        }
    }

})();
