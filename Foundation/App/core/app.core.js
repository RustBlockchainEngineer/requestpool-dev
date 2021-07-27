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
                controller: 'core.home.ctrl',
                templateUrl: '/app/views/core/home.html',
                params: {},
                data: { activeMenuItem: 'home' }

            })
            .state('core/profile', {
                url: '/core/profile',
                controller: 'core.profile.ctrl',
                controllerAs: 'vm',
                templateUrl: '/app/views/core/profile.html',
                params: {},
                data: { activeMenuItem: 'profile' }

            })
            .state('core/login', {
                url: '/core/login',
                controller: 'core.login.ctrl',
                templateUrl: '/app/views/core/login.html',
                params: {}
            })
            .state('core/administrators/list', {
                url: '/core/administrators/list',
                controller: 'core.administrators.list.ctrl',
                templateUrl: '/app/views/core/administrators.list.html',
                controllerAs: 'vm',
                params: {},
                permissions: 'admin , manage admins',
                data: { activeMenuItem: 'administrators' }

            })
            .state('core/administrators', {
                url: '/core/administrators/:id',
                controller: 'core.administrators.ctrl',
                templateUrl: '/app/views/core/administrators.html',
                controllerAs: 'vm',
                params: { item: null ,id:null, mode:null},
                permissions: 'admin , manage admins',
                data: { activeMenuItem: 'administrators' }

            });
        

        $httpProvider.interceptors.push('core.authenticationInterceptor');
    }

    function run($rootScope, $state, system, settings, account, ui, resources) {
        $rootScope.account = account;
        $rootScope.settings = settings;
        account.loadStorage();
        resources.load();

        $rootScope.logout = function () {
            ui.overlay.show('body');
            account.logout().then(
            function (result) {
                $rootScope.$broadcast('authentication');
                ui.overlay.hide('body');
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