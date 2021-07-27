
/*
*
*/

(function () {
    'use strict';
    angular.module('app.main').factory('main.enquiries.svc', svc);

    svc.$inject = ['$httpParamSerializer', '$http', '$window', 'core.settings'];

    function svc($httpParamSerializer, $http, $window, settings) {
        return {
            search: search,
            get: get,
            getInvitationsCount: getInvitationsCount,
            previewProperties: previewProperties,
            preview: preview
        }


        function search(filter) {
            return $http.get(settings.apiUrl + '/enquiries/search/?' + $httpParamSerializer(filter));
        }
        function get(id) {
            return $http.get(settings.apiUrl + '/enquiries/' + id);
        }
        function getInvitationsCount(id) {
            return $http.get(settings.apiUrl + '/enquiries/' + id+'/invitations-count');
        }
        function preview(id) {
            return $http.get(settings.apiUrl + '/enquiries/preview/' + id);
        }
        function previewProperties(id) {
            return $http.get(settings.apiUrl + '/enquiries/preview/properties/' + id);
        }
    }

})();
