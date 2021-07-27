
/*
*
*/

(function () {
    'use strict';
    angular.module('app.main').factory('main.enquiries.svc', svc);

    svc.$inject = ['$httpParamSerializer', '$http', '$window', 'core.settings'];

    function svc($httpParamSerializer, $http, $window, settings) {
        return {
            all: all,
            search: search,
            get: get,
            getInvitationsCount: getInvitationsCount,
            update: update,
            create: create,
            remove: remove,
            copy: copy,
            previewProperties: previewProperties,
            preview: preview
        }

        function all(isTemplate) {
            return $http.get(settings.apiUrl + '/enquiries/' + ((isTemplate !== undefined) ? isTemplate : ''));
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
        function create(model) {
            return $http.post(settings.apiUrl + '/enquiries/', model);
        }
        function remove(id) {
            return $http.delete(settings.apiUrl + '/enquiries/' + id);
        }
        function update(model) {
            return $http.put(settings.apiUrl + '/enquiries/' + model.id, model);
        }
        
        function copy(model) {
            return $http.post(settings.apiUrl + '/enquiries/copy/'+model.enquiryId, model);
        }
        function preview(id) {
            return $http.get(settings.apiUrl + '/enquiries/preview/' + id);
        }
        function previewProperties(id) {
            return $http.get(settings.apiUrl + '/enquiries/preview/properties/' + id);
        }
    }

})();
