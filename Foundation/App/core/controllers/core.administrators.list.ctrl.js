/*
*
*/

(function () {
    'use strict';
    angular.module('app.core').controller('core.administrators.list.ctrl', ctrl);
     
    ctrl.$inject = ['$scope', '$state', 'core.system.svc', 'core.settings', '$uibModal', 'ui.svc', 'core.administrators.svc'];

    function ctrl($scope, $state,system, settings,$uibModal, ui,administrators) {
        ui.page.title = system.resources.views.administrators;

        var vm = this;

        vm.search = { isDeleted: false };
        vm.showSearch = true;
        vm.toggleSearch = function () {
            vm.showSearch = !vm.showSearch;
        }


        ui.overlay.show();
        administrators.search().then(
            function (response) {
                vm.list = response.data.content;
            },
            function (err) {
                ui.alerts.http(err);
            }
        ).finally(function () {
            ui.overlay.hide();
        });


        vm.edit = function (item) {
            $state.go('core/administrators', { id: item.id, item: angular.copy(item),mode:'form'});
        }
        vm.showDetails = function (item) {
            $state.go('core/administrators', { id: item.id, item: angular.copy(item) });

        }

        vm.remove = function (item) {
            ui.overlay.show();
            administrators.remove(item.id).then(
                function (response) {
                    var selectedIndex = -1;
                    for (var i = 0; i < vm.list.length; i++) {
                        if (vm.list[i].id == item.id) {
                            selectedIndex = i;
                            break;
                        }
                    }
                    if (selectedIndex > -1) {
                        vm.list.splice(selectedIndex, 1);
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
            administrators.toggleActive(item.id).then(
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