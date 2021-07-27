/*
*
*/
var server = 'http://localhost:55555';
var root = '/_public';
var resources;
(function () {
    'use strict';
    angular.module('app', ['angularjs-dropdown-multiselect','ngSanitize', 'ui.router','ngGeolocation','ngHandsontable', 'ngMap', 'ui.bootstrap', 'ui.tinymce', 'ui.select', 'ui.mask', 'color.picker', 'moment-picker', 'smart-table', 'checklist-model', 'ui.slimscroll', 'adminLTE', 'app.ui', 'app.core', 'app.main']).config(config).run(run);

    config.$inject = ['$stateProvider', '$urlRouterProvider', '$httpProvider'];
    run.$inject = ['$state'];

    function config($stateProvider, $urlRouterProvider, $httpProvider) {
        $urlRouterProvider.otherwise('/core/home');
    }

    function run($state) {

    }


    window.onload = function () {
        var overlay = new $.overlay();
        //overlay.blockUi();
        var dependencies = 1;
        var $http = angular.injector(['ng']).get('$http');
        $http.get(server + '/api/resources', {})
            .then(
            function (response) {
                resources = response.data;
                dependencies--;
                if (dependencies <= 0) {
                    angular.bootstrap(document, ['app']);
                    overlay.unblockUi();
                }

            },
            function (err) {

            }
            );
    }

})();
