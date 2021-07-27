
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

        
        vm.showDetails = function (item) {
            $state.go('main/projects', { id: item.id, item: angular.copy(item) });
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