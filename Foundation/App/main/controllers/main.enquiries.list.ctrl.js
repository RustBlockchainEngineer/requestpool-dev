

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

        
        vm.showDetails = function (item) {
            if (item.isTemplate)
                $state.go('main/templates', {isTemplate:true, id: item.id, item: angular.copy(item), viewMode: 'view' });
            else
                $state.go('main/enquiries', { isTemplate: false, id: item.id, item: angular.copy(item), viewMode: 'view' });

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