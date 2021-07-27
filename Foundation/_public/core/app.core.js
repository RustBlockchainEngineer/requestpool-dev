/*
*
*/

(function () {
    'use strict';
    angular.module('app.core', []).config(config).run(run);

    config.$inject = ['$stateProvider', '$urlRouterProvider', '$httpProvider'];
    run.$inject = ['$rootScope','$state','core.system.svc','core.settings','core.account.svc','ui.svc','core.resources.svc'];

    function config($stateProvider, $urlRouterProvider, $httpProvider) {

        $stateProvider
            .state('core/home', {
                url: '/core/home',
                controller: 'core.homeCtrl',
                templateUrl: root+'/views/core/home.html',
                params: {},
                data: { activeMenuItem: 'home' }

            })
            .state('core/login', {
                url: '/core/login',
                controller: 'core.login.ctrl',
                templateUrl: root +'/views/core/login.html',
                params: {},
                controllerAs: 'vm'

            })
            .state('core/profile', {
                url: '/core/profile',
                controller: 'core.profile.ctrl',
                templateUrl: root + '/views/core/profile.html',
                params: {},
                controllerAs: 'vm',
                permissions: 'public_user',
                data: { activeMenuItem: 'profile' }

            })
           ;
        

        $httpProvider.interceptors.push('core.authenticationInterceptor');
    }

    function run($rootScope, $state, system, settings, account, ui, resources) {
        //$rootScope.regex = function (str) {
        //    return new RegExp(str);
        //}
        $rootScope.account = account;
        $rootScope.settings = settings;
        account.loadStorage();
        resources.load();
        //validation.load();

        $rootScope.logout = function () {
            ui.overlay.blockUi();
            account.logout().then(
            function (result) {
                $rootScope.$broadcast('authentication');
                ui.overlay.unblockUi();
                $state.go('core/login');
            });
        }
        $rootScope.$on('$stateChangeStart', function (event, toState, toParams, fromState, fromParams) {
            if (toState.name != 'core/login' && !account.authenticated) {
                event.preventDefault();
                $state.go('core/login');
            }
            else if(toState.name == 'core/login' && account.authenticated)
            {
                event.preventDefault();
                $state.go('core/home');
            }
            if (toState.permissions && !account.has(toState.permissions,toState.strict)) {
                event.preventDefault();
                $state.go('core/home');
            }
        });
        $rootScope.$on('$stateChangeSuccess', function (event, toState, toParams, fromState, fromParams) {
            if (toState.data)
                $rootScope.activeMenuItem = toState.data.activeMenuItem;
        });
    }


})();