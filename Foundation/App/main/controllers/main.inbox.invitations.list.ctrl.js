/*
*
*/

(function () {
    'use strict';
    angular.module('app.main').controller('main.inbox.invitations.list.ctrl', ctrl);
     
    ctrl.$inject = ['$rootScope','$scope', '$state', 'core.system.svc', 'core.settings', '$uibModal', 'ui.svc', 'main.invitations.svc'];

    function ctrl($rootScope,$scope, $state, system, settings, $uibModal, ui, invitations) {
        ui.page.title = system.resources.views.inbox;

        var vm = this;
        vm.search = { isDeleted:false};
        vm.showSearch = true;
        vm.toggleSearch = function () {
            vm.showSearch = !vm.showSearch;
        }

        vm.showDetails = function (item) {
            $state.go('main/inbox/invitations', { id: item.recipientId, item: angular.copy(item) });
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
            invitations.searchIncoming(searchModel).then(
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
    }
})();