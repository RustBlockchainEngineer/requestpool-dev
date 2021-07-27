/*
*
*/

(function () {
    'use strict';
    angular.module('app.core').factory('core.settings', settings);

    settings.$inject = [];

    function settings() {
        //var server = 'http://localhost:55555'
        return {
            server: server,
            apiUrl: server + '/api',
            dataKey: 'data',
            culture: 'ar',
            googleMapsApi: 'https://maps.googleapis.com/maps/api/js?key=AIzaSyAMoWSumz3-2qH-5w4rcYeRj8OW9Hl_-7A',
            loginHeader:'X-Public'
        }
    }

})();