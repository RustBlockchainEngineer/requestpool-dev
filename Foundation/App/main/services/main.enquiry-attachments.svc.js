
/*
*
*/

(function () {
    'use strict';
    angular.module('app.main').factory('main.enquiryAttachments.svc', svc);

    svc.$inject = ['$httpParamSerializer', '$http', '$window', 'core.settings'];

    function svc($httpParamSerializer, $http, $window, settings) {
        return {
            get: get
        }

        function get(enquiryId) {
            return $http.get(settings.apiUrl + '/enquiries-attachments/' + enquiryId);
        }
    }

})();
