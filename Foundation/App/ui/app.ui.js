/*
*
*/

(function () {
    'use strict';
    angular.module('app.ui', []).config(config).run(run);

    config.$inject = ['$stateProvider', '$urlRouterProvider', '$httpProvider'];
    run.$inject = ['$rootScope','$state','ui.svc'];

    function config($stateProvider, $urlRouterProvider, $httpProvider) {
    }

    function run($rootScope, $state, ui) {
        $rootScope.ui = ui;
        //before rendering a new state, reset page data such as title, description
        $rootScope.$on('$stateChangeStart', function (event, toState, toParams, fromState, fromParams) {
            ui.page = {};
            ui.alerts.clear();
        });
    }
})();