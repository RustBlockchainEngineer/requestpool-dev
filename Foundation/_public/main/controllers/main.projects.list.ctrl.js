
/*
*
*/

(function () {
    'use strict';
    angular.module('app.main').controller('main.projects.list.ctrl', ctrl);
     
    ctrl.$inject = ['$scope', '$state', 'core.system.svc', 'core.settings', '$uibModal', 'ui.svc', 'main.projects.svc'];

    function ctrl($scope, $state,system, settings,$uibModal, ui,projects) {
        ui.page.title = system.resources.views.projects;

        var vm = this;
        vm.search = { isDeleted: false, isCreatedByApp: true };
        vm.showSearch = true;
        vm.toggleSearch = function () {
            vm.showSearch = !vm.showSearch;
        }

        vm.edit = function (item) {
            $state.go('main/projects', { id: item.id, item: angular.copy(item),mode:'form' });
        }
        vm.showDetails = function (item) {
            $state.go('main/projects', { id: item.id, item: angular.copy(item) });
        }

        vm.remove = function (item) {
            ui.overlay.show();
            projects.remove(item.id).then(
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
        vm.addEnquiry = function (item) {
            $state.go('main/enquiries', { projectId: item.id, project: angular.copy(item), mode: 'form' });
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
            projects.search(searchModel).then(
                function (response) {
                    vm.displayedList = response.data.content;
                    tableState.pagination.numberOfPages = Math.ceil(response.data.totalCount / itemsPerPage);
                },
                function (err) { }
            ).finally(function () { ui.overlay.hide(); });
        }
    }
})();