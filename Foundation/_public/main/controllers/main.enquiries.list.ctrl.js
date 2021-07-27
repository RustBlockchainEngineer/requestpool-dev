

/*
*
*/

(function () {
    'use strict';
    angular.module('app.main').controller('main.enquiries.list.ctrl', ctrl);

    ctrl.$inject = ['$rootScope', '$scope', '$state', 'core.system.svc', 'core.settings', '$uibModal', 'ui.svc', 'main.enquiries.svc'
        , 'main.membership.svc'];

    function ctrl($rootScope, $scope, $state, system, settings, $uibModal, ui, enquiries, membership) {
        if ($state.params.isTemplate)
            ui.page.title = system.resources.views.templates;
        else
            ui.page.title = system.resources.views.enquiries;


        var vm = this;
        vm.search = { isDeleted: false, isCreatedByApp: true, isTemplate: false };
        vm.isTemplate = $state.params.isTemplate;
        vm.search.isTemplate = vm.isTemplate;
        vm.showSearch = true;
        vm.toggleSearch = function () {
            vm.showSearch = !vm.showSearch;
        }

        ui.overlay.show();
        membership.current().then(
            function (response) {
                vm.disableAdd = false;
                vm.currentMembership = response.data.content;
                if (vm.currentMembership.consumption.enquiriesPerMonth >= vm.currentMembership.membershipPlan.maxEnquiriesPerMonth)
                {
                    vm.disableAdd = true;
                }
            },
            function () {
            }
        ).finally(function () {
            ui.overlay.hide();
        });

        vm.edit = function (item) {
            if (item.isTemplate)
                $state.go('main/templates', {isTemplate:true, id: item.id, item: angular.copy(item), viewMode: 'form' });
            else
                $state.go('main/enquiries', {isTemplate:false, id: item.id, item: angular.copy(item), viewMode: 'form' });

        }
        vm.showDetails = function (item) {
            if (item.isTemplate)
                $state.go('main/templates', {isTemplate:true, id: item.id, item: angular.copy(item), viewMode: 'view' });
            else
                $state.go('main/enquiries', { isTemplate: false, id: item.id, item: angular.copy(item), viewMode: 'view' });

        }

        vm.remove = function (item) {
            ui.overlay.show();
            enquiries.remove(item.id).then(
                function (response) {
                    var selectedIndex = -1;
                    for (var i = 0; i < vm.displayedList.length; i++) {
                        if (vm.displayedList[i].id == item.id) {
                            selectedIndex = i;
                            break;
                        }
                    }
                    if (selectedIndex > -1) {
                        vm.displayedList.splice(selectedIndex, 1);
                    }
                },
                function (err) {
                    ui.alerts.http(err);
                })
                .finally(function () {
                    ui.overlay.hide();
                });
        }

        var lastSearchModel = {};
        vm.onlineSearch = function (tableState) {

            var pagination = tableState.pagination;
            var itemsPerPage = pagination.number || 10;  // Number of entries showed per page.
            var pageNumber = pagination.start ? (pagination.start / itemsPerPage) + 1 : 1;
            var searchModel = { pageNumber: pageNumber, itemsPerPage: itemsPerPage };
            angular.extend(searchModel, vm.search);
            if (angular.equals(searchModel, lastSearchModel))
                return;
            lastSearchModel = angular.copy(searchModel);
            ui.overlay.show();
            enquiries.search(searchModel).then(
                function (response) {
                    vm.displayedList = response.data.content;
                    tableState.pagination.numberOfPages = Math.ceil(response.data.totalCount / itemsPerPage);
                },
                function (err) { }
            ).finally(function () { ui.overlay.hide(); });
        }

        vm.refresh = function (id) {
            $rootScope.$broadcast('refreshtable', id);
        }
        vm.copy = function (item, model) {
            if (item)
                model.enquiryId = item.id;
            else if (vm.selectedItem)
                model.enquiryId = vm.selectedItem.id;
            else
                return;
            ui.overlay.show();
            enquiries.copy(model).then(
                function (response) {
                    vm.edit(response.data.content);
                },
                function (err) {
                    ui.alerts.http(err);
                })
                .finally(function () {
                    ui.overlay.hide();
                });
        }
        vm.toggleSelection = function (item, index) {
            if (vm.selectedIndex == index)
            {
                vm.selectedIndex = null;
                vm.selectedItem = null;
            }
            else
            {
                vm.selectedIndex = index;
                vm.selectedItem = item;
            }
        }

    }
})();