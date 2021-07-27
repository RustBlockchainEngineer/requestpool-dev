
/*
*
*/

(function () {
    'use strict';
    angular.module('app.main').factory('main.publicUsers.svc', svc);

    svc.$inject = ['$httpParamSerializer', '$http', '$window', 'core.settings'];

    function svc($httpParamSerializer, $http, $window, settings) {
        return {
            all:all,
            search: search,
            get: get,
            update: update,
            create: create,
            remove: remove,
            removePhoto: removePhoto,
            toggleActive: toggleActive,
            updateActivationCode: updateActivationCode
        }

        function all() {
            return $http.get(settings.apiUrl + '/publicusers/');
        }

        function search(filter) {
            return $http.get(settings.apiUrl + '/publicusers/search/?' + $httpParamSerializer(filter));
        }
        function get(id) {
            return $http.get(settings.apiUrl + '/publicusers/'+id);
        }
       
        function create(model) {
            return $http.post(settings.apiUrl + '/publicusers/', model);
        }
        function remove(id) {
            return $http.delete(settings.apiUrl + '/publicusers/' + id);
        }
        function removePhoto(id) {
            return $http.delete(settings.apiUrl + '/publicusers/' + id+'/photo');
        }
        function update(model) {
            return $http.put(settings.apiUrl + '/publicusers/' + model.id, model);
        }
        function toggleActive(id) {
            return $http.post(settings.apiUrl + '/publicusers/' + id + '/toggleactive');
        }
        function updateActivationCode(id) {
            return $http.put(settings.apiUrl + '/publicusers/' + id + '/update-code');
        }
    }

})();
