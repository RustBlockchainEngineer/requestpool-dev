
/*
*
*/

(function () {
    'use strict';
    angular.module('app.main').factory('main.membershipPlans.svc', svc);

    svc.$inject = ['$httpParamSerializer', '$http', '$window', 'core.settings'];

    function svc($httpParamSerializer, $http, $window, settings) {
        return {
            all:all,
            search: search,
            get: get,
            update: update,
            create: create,
            remove: remove,
            toggleActive: toggleActive,
            togglePublic: togglePublic,
            setDefault: setDefault,
            setDefaultDowngrade: setDefaultDowngrade


        }

        function all() {
            return $http.get(settings.apiUrl + '/membership-plans/');
        }

        function search(filter) {
            return $http.get(settings.apiUrl + '/membership-plans/search/?' + $httpParamSerializer(filter));
        }
        function get(id) {
            return $http.get(settings.apiUrl + '/membership-plans/'+id);
        }
            function create(model) {
                return $http.post(settings.apiUrl + '/membership-plans/', model);
            }
        function remove(id) {
            return $http.delete(settings.apiUrl + '/membership-plans/' + id);
            }
        function update(model) {
            return $http.put(settings.apiUrl + '/membership-plans/' + model.id, model);
        }
        function toggleActive(id) {
            return $http.post(settings.apiUrl + '/membership-plans/' + id + '/toggleactive');
        }
        function togglePublic(id) {
            return $http.post(settings.apiUrl + '/membership-plans/' + id + '/togglepublic');
        }

        function setDefault(id) {
            return $http.post(settings.apiUrl + '/membership-plans/' + id + '/setdefault');
        }
        function setDefaultDowngrade(id) {
            return $http.post(settings.apiUrl + '/membership-plans/' + id + '/setdefaultdowngrade');
        }
    }

})();
