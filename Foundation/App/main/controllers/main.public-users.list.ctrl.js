/*
*
*/

(function () {
    'use strict';
    angular.module('app.main').controller('main.publicUsers.list.ctrl', ctrl);
     
    ctrl.$inject = ['$scope', '$state', 'core.system.svc', 'core.settings', '$uibModal', 'ui.svc', 'main.publicUsers.svc'];

    function ctrl($scope, $state,system, settings,$uibModal, ui,publicUsers) {
        ui.page.title = system.resources.views.public_users;

        var vm = this;
        vm.search = { isDeleted: false, isCreatedByApp: true };
        vm.showSearch = true;
        vm.toggleSearch = function () {
            vm.showSearch = !vm.showSearch;
        }

        vm.edit = function (item) {
            $state.go('main/public-users', { id: item.id, item: angular.copy(item),mode:'form' });//vm.list[selectedIndex]) 
        }
        vm.showDetails = function (item) {
            $state.go('main/public-users', { id: item.id, item: angular.copy(item) });//vm.list[selectedIndex]) 
        }

        vm.remove = function (item) {
            ui.overlay.show();
            publicUsers.remove(item.id).then(
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
            publicUsers.toggleActive(item.id).then(
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
            publicUsers.search(searchModel).then(
                function (response) {
                    vm.displayedList = response.data.content;
                    tableState.pagination.numberOfPages = Math.ceil(response.data.totalCount / itemsPerPage);
                },
                function (err) { }
            ).finally(function () { ui.overlay.hide(); });
        }

        vm.getPhotoIcon = function (item) {
            var ext = item.photo ? item.photo.split('.').pop().toLowerCase() : '';
            var src = '';
            switch (ext) {
                case 'jpeg':
                case 'jpg':
                case 'png':
                case 'gif':
                case 'bmp': {
                    src = item.photoUrl;
                } break;
                default: {
                    src = ui.settings.images.person;
                }
            }
            return src;
        }
    }
})();