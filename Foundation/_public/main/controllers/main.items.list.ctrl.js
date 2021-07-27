

/*
*
*/

(function () {
    'use strict';
    angular.module('app.main').controller('main.items.list.ctrl', ctrl);

    ctrl.$inject = ['$rootScope', '$scope', '$state', 'core.system.svc', 'core.settings', '$uibModal', 'ui.svc', 'main.items.svc'
        , 'main.itemsProperties.svc'];

    function ctrl($rootScope, $scope, $state, system, settings, $uibModal, ui, items, itemsProperties) {
        ui.page.title = system.resources.views.items;

        var vm = this;
        vm.search = { isDeleted: false };
        vm.showSearch = true;
        vm.toggleSearch = function () {
            vm.showSearch = !vm.showSearch;
        }

        ui.overlay.show();
        itemsProperties.search().then(
            function (response) {
                //remove deleted properties
                for (var i = 0; i < response.data.content.length; i++) {
                    if (response.data.content[i].isDeleted === true) {
                        response.data.content[i].splice(i, 1);
                    }
                }
                vm.dynamicProperties = response.data.content;
            },
            function (err) {
                ui.alerts.http(err);
            }
        ).finally(function () {
            ui.overlay.hide();
        });
        
        vm.showDetails = function (item) {
            if (item.enquiry.isTemplate)
                $state.go('main/templates', { isTemplate: true, id: item.enquiry.id, item: null, viewMode: 'view' });
            else
                $state.go('main/enquiries', { isTemplate: false, id: item.enquiry.id, item: null, viewMode: 'view' });

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
            items.search(searchModel).then(
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