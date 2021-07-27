/*
*
*/

(function () {
    'use strict';
    angular.module('app.main').controller('main.membershipPlans.list.ctrl', ctrl);
     
    ctrl.$inject = ['$scope', '$state', 'core.system.svc', 'core.settings', '$uibModal', 'ui.svc',
        'main.membershipPlans.svc'];

    function ctrl($scope, $state, system, settings, $uibModal, ui, membershipPlans) {
        ui.page.title = system.resources.views.membership_plans;

        var vm = this;
        vm.search = { isDeleted: false };
        vm.showSearch = true;
        vm.toggleSearch = function () {
            vm.showSearch = !vm.showSearch;
        }
        
        //ui.overlay.show();
        //ads.search().then(
        //    function (response) {
        //        vm.list = response.data.content;
        //    },
        //    function (err) {
        //        ui.alerts.http(err);
        //    }
        //).finally(function () {
        //    ui.overlay.hide();
        //});


        vm.edit = function (item) {
            $state.go('main/membership-plans', { id: item.id, item: angular.copy(item),mode:'form' });//vm.list[selectedIndex]) 
        }
        vm.showDetails = function (item) {
            $state.go('main/membership-plans', { id: item.id, item: angular.copy(item) });//vm.list[selectedIndex]) 
        }

        vm.remove = function (item) {
            ui.overlay.show();
            membershipPlans.remove(item.id).then(
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

        vm.toggleActive = function (item) {
            ui.overlay.show();
            membershipPlans.toggleActive(item.id).then(
                function (response) {
                    item.isActive = !item.isActive;
                },
            function (err) {
                ui.alerts.http(err);
            })
            .finally(function () {
                ui.overlay.hide();
            });
        }

        vm.togglePublic = function (item) {
            ui.overlay.show();
            membershipPlans.togglePublic(item.id).then(
                function (response) {
                    item.isPublic = !item.isPublic;
                },
                function (err) {
                    ui.alerts.http(err);
                })
                .finally(function () {
                    ui.overlay.hide();
                });
        }
        vm.setDefault = function (item) {
            ui.overlay.show();
            membershipPlans.setDefault(item.id).then(
                function (response) {
                    item.isDefault = true;
                    for (var i = 0; i < vm.displayedList.length; i++) {
                        if (vm.displayedList[i].id != item.id) {
                            vm.displayedList[i].isDefault = false;
                        }
                    }
                },
                function (err) {
                    ui.alerts.http(err);
                })
                .finally(function () {
                    ui.overlay.hide();
                });
        }
        vm.setDefaultDowngrade = function (item) {
            ui.overlay.show();
            membershipPlans.setDefaultDowngrade(item.id).then(
                function (response) {
                    item.isDefaultDowngrade = true;
                    for (var i = 0; i < vm.displayedList.length; i++) {
                        if (vm.displayedList[i].id != item.id) {
                            vm.displayedList[i].isDefaultDowngrade = false;
                        }
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
            //tableState.search.predicateObject;
            //tableState.sort.predicate;
            var searchModel = { pageNumber: pageNumber, itemsPerPage: itemsPerPage };
            angular.extend(searchModel, vm.search);
            if (angular.equals(searchModel, lastSearchModel))
                return;
            lastSearchModel = angular.copy(searchModel);
            ui.overlay.show();
            membershipPlans.search(searchModel).then(
                function (response) {
                    vm.displayedList = response.data.content;
                    tableState.pagination.numberOfPages = Math.ceil(response.data.totalCount / itemsPerPage);
                },
                function (err) { }
            ).finally(function () { ui.overlay.hide(); });
        }
    }
})();