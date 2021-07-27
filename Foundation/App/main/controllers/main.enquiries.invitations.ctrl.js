

/*
*
*/

(function () {
    'use strict';
    angular.module('app.main').controller('main.enquiries.invitations.ctrl', ctrl);

    ctrl.$inject = ['$filter','$rootScope','$scope', '$state', 'core.system.svc', 'core.settings', '$uibModal', 'ui.svc',
        'main.enquiries.svc', 'main.invitations.svc', 'main.contacts.svc', 'main.contactTypes.svc', 'main.membership.svc'];

    function ctrl($filter,$rootScope,$scope, $state, system, settings, $uibModal, ui, enquiries, invitations, contacts, contactTypes,membership) {
        ui.page.title = system.resources.views.invitations;

        var vm = this;
        vm.viewMode = 'list';
        vm.model = {};
        vm.search = { isDeleted: false,enquiryId:-1 };
        vm.showSearch = true;
        vm.toggleSearch = function () {
            vm.showSearch = !vm.showSearch;
        }
        vm.now = new Date().toISOString();
        
        //get enquiry
        if ($state.params.item && $state.params.item.id) {
            vm.model.enquiry = $state.params.item;
            vm.search.enquiryId = vm.model.enquiry.id;

        } else if ($state.params.id) {
            ui.overlay.show();
            enquiries.get($state.params.id).then(
                function (response) {
                    $state.params.item = response.data.content;
                    vm.model.enquiry = $state.params.item;
                    vm.search.enquiryId = vm.model.enquiry.id;
                },
                function () {
                }
            ).finally(function () {
                ui.overlay.hide();
            });
        }
        else {
            if ($state.params.isTemplate)
                $state.go('main/templates/list');
            else
                $state.go('main/enquiries/list');
        }

        vm.view = function (item) {
            vm.model = angular.copy(item);
            vm.model.formattedEndDate = $filter('date')(vm.model.endDate, 'dd-MM-yyyy');
            vm.viewMode = 'view';
        }
        vm.list = function () {
            vm.model = {};
            vm.viewMode = 'list';
            refresh('invitations');

        }
        //ui.overlay.show();
        //contactTypes.search().then(
        //    function (response) {
        //        vm.contactTypes = response.data.content;
        //    },
        //    function (err) {
        //        ui.alerts.http(err);
        //    }
        //).finally(function () {
        //    ui.overlay.hide();
        //});
        //ui.overlay.show();
        //contacts.all().then(
        //    function (response) {
        //        vm.contacts = response.data.content;
        //    },
        //    function (err) {
        //        ui.alerts.http(err);
        //    }
        //).finally(function () {
        //    ui.overlay.hide();
        //});
        
        vm.cancel = function () {
            vm.model = {};
            vm.list();
        }

       
        var lastSearchModel = {};
        vm.onlineSearch = function (tableState) {

            var pagination = tableState.pagination;
            var itemsPerPage = pagination.number || 10;  // Number of entries showed per page.
            var pageNumber = pagination.start ? (pagination.start / itemsPerPage) + 1 : 1;
            var searchModel = { pageNumber: pageNumber, itemsPerPage: itemsPerPage };
            console.log('1');
            angular.extend(searchModel, vm.search);
            if ($rootScope.forceRefresh != 'invitations' && angular.equals(searchModel, lastSearchModel))
                return;
            $rootScope.forceRefresh = null;
            console.log('hi');
            lastSearchModel = angular.copy(searchModel);
            ui.overlay.show();
            invitations.searchOutgoing(searchModel).then(
                function (response) {
                    vm.displayedList = response.data.content;
                    tableState.pagination.numberOfPages = Math.ceil(response.data.totalCount / itemsPerPage);
                },
                function (err) { }
            ).finally(function () { ui.overlay.hide(); });
        }

        function refresh(id) {
            $rootScope.$broadcast('refreshtable',id);
        }
    }
})();