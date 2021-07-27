/*
*
*/

(function () {
    'use strict';
    angular.module('app.main', []).config(config).run(run);

    config.$inject = ['$stateProvider', '$urlRouterProvider', '$httpProvider'];
    run.$inject = ['$rootScope', '$state', 'core.settings', 'core.account.svc'];


    function config($stateProvider, $urlRouterProvider, $httpProvider) {
        
        $stateProvider
            .state('main/enquiry-types', {
                url: '/main/enquiry-types',
                controller: 'main.enquiryTypes.ctrl',
                templateUrl: root + '/views/main/enquiry-types.html',
                permissions: 'public_user',
                data: { activeMenuItem:'enquiry-types'}
            })
            .state('main/item-types', {
                url: '/main/item-types',
                controller: 'main.itemTypes.ctrl',
                templateUrl: root + '/views/main/item-types.html',
                permissions: 'public_user',
                data: { activeMenuItem: 'item-types' }
            })
            .state('main/items-properties', {
                url: '/main/items-properties',
                controller: 'main.itemsProperties.ctrl',
                templateUrl: root + '/views/main/items-properties.html',
                permissions: 'public_user',
                data: { activeMenuItem: 'item-properties' }
            })
            .state('main/contact-types', {
                url: '/main/contact-types',
                controller: 'main.contactTypes.ctrl',
                templateUrl: root + '/views/main/contact-types.html',
                permissions: 'public_user',
                data: { activeMenuItem: 'contact-types' }
            })
            .state('main/contacts/list', {
                url: '/main/contacts/list',
                controller: 'main.contacts.list.ctrl',
                templateUrl: root + '/views/main/contacts.list.html',
                controllerAs: 'vm',
                permissions: 'public_user',
                data: { activeMenuItem: 'contacts' }
            })
            .state('main/contacts', {
                url: '/main/contacts/:id',
                controller: 'main.contacts.ctrl',
                templateUrl: root + '/views/main/contacts.html',
                controllerAs: 'vm',
                params: { item: null, mode: null },
                permissions: 'public_user',
                data: { activeMenuItem: 'contacts' }
            })
            .state('main/clients/list', {
                url: '/main/clients/list',
                controller: 'main.clients.list.ctrl',
                templateUrl: root+'/views/main/clients.list.html',
                controllerAs: 'vm',
                permissions: 'public_user',
                data: { activeMenuItem: 'clients' }
            })
            .state('main/clients', {
                url: '/main/clients/:id',
                controller: 'main.clients.ctrl',
                templateUrl: root +'/views/main/clients.html',
                controllerAs: 'vm',
                params: { item: null, mode: null },
                permissions: 'public_user',
                data: { activeMenuItem: 'clients' }
            })
            .state('main/projects/list', {
                url: '/main/projects/list',
                controller: 'main.projects.list.ctrl',
                templateUrl: root +'/views/main/projects.list.html',
                controllerAs: 'vm',
                permissions: 'public_user',
                data: { activeMenuItem: 'projects' }
            })
            .state('main/projects', {
                url: '/main/projects/:id',
                controller: 'main.projects.ctrl',
                templateUrl: root +'/views/main/projects.html',
                controllerAs: 'vm',
                params: { item: null, mode: null },
                permissions: 'public_user',
                data: { activeMenuItem: 'projects' }
            })
            .state('main/enquiries/list', {
                url: '/main/enquiries/list',
                controller: 'main.enquiries.list.ctrl',
                templateUrl: root + '/views/main/enquiries.list.html',
                controllerAs: 'vm',
                params: {isTemplate: false },
                permissions: 'public_user',
                data: { activeMenuItem: 'enquiries' }
            })
            .state('main/enquiries', {
                url: '/main/enquiries/:id?projectId',
                controller: 'main.enquiries.ctrl',
                templateUrl: root + '/views/main/enquiries.html',
                controllerAs: 'vm',
                params: { item: null, viewMode: null, project: null, isTemplate: false},
                permissions: 'public_user',
                data: { activeMenuItem: 'enquiries' }
            })
            .state('main/templates/list', {
                url: '/main/templates/list',
                controller: 'main.enquiries.list.ctrl',
                templateUrl: root + '/views/main/enquiries.list.html',
                controllerAs: 'vm',
                params: { isTemplate: true },
                permissions: 'public_user',
                data: { activeMenuItem: 'templates' }
            })
            .state('main/templates', {
                url: '/main/templates/:id',
                controller: 'main.enquiries.ctrl',
                templateUrl: root + '/views/main/enquiries.html',
                controllerAs: 'vm',
                params: { item: null, viewMode: null, isTemplate: true },
                permissions: 'public_user',
                data: { activeMenuItem: 'templates' }
            })
            .state('main/inbox/invitations/list', {
                url: '/main/inbox/invitations/list/',
                controller: 'main.inbox.invitations.list.ctrl',
                templateUrl: root + '/views/main/inbox.invitations.list.html',
                controllerAs: 'vm',
                permissions: 'public_user',
                data: { activeMenuItem: 'invitations' }
            })
            .state('main/inbox/invitations', {
                url: '/main/inbox/invitations/:id',
                controller: 'main.inbox.invitations.ctrl',
                templateUrl: root + '/views/main/inbox.invitations.html',
                controllerAs: 'vm',
                params: { item: null, mode: null},
                permissions: 'public_user',
                data: { activeMenuItem: 'invitations' }
            })
            .state('main/membership', {
                url: '/main/membership',
                controller: 'main.membership.ctrl',
                templateUrl: root + '/views/main/membership.html',
                controllerAs: 'vm',
                params: { },
                permissions: 'public_user',
                data: { activeMenuItem: 'membership' }
            })
            .state('main/items/list', {
                url: '/main/items/list',
                controller: 'main.items.list.ctrl',
                templateUrl: root + '/views/main/items.list.html',
                controllerAs: 'vm',
                permissions: 'public_user',
                data: { activeMenuItem: 'items' }
            })
            ;
    }

    function run($rootScope, $state, settings, account) {
        $rootScope.$on('$stateChangeStart', function (event, toState, toParams, fromState, fromParams) {
        });
    }


})();