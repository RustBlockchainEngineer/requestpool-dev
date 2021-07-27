/*
*
*/

(function () {
    'use strict';
    angular.module('app.core').controller('main.status.ctrl', ctrl);
     
    ctrl.$inject = ['$scope', '$state', 'core.system.svc', 'core.settings', '$uibModal', 'ui.svc', 'main.status.svc'];

    function ctrl($scope, $state,system, settings,$uibModal, ui,status) {
        ui.page.title = system.resources.views.status;

        $scope.search = { isDeleted: false };
        $scope.initialModel = {name:'',name_EN:''};

        $scope.model = { color: '#ff0000' };
        $scope.colorPickerOptions = { hue: true, swatch: true, swatchBootstrap: true, swatchOnly: true, format: 'hexString', alpha: false };  
        $scope.showSearch = false;
        $scope.toggleSearch = function () {
            $scope.showSearch = !$scope.showSearch;
        }

        refresh();
        function refresh() {
            ui.overlay.show();
            status.search().then(
                function (response) {
                    $scope.list = response.data.content;
                },
                function () {
                }
            ).finally(function () {
                ui.overlay.hide();
            });
        }

        $scope.edit = function (item) {
            angular.copy(item,$scope.model);
        }

        $scope.remove = function (item) {
            ui.overlay.show();
            status.remove(item.id).then(
                function (response) {
                    var selectedIndex = -1;
                    for (var i = 0; i < $scope.list.length; i++) {
                        if ($scope.list[i].id == item.id) {
                            selectedIndex = i;
                            break;
                        }
                    }
                    if (selectedIndex > -1) {
                        $scope.list.splice(selectedIndex, 1);
                    }
                },
            function () {
                ui.alerts.add('Error', result.error, 'danger');

            })
            .finally(function () {
                ui.overlay.hide();
            });
        }

        $scope.save = function () {
            ui.overlay.show();
            if ($scope.model.id) {
                status.update($scope.model).then(
                function (response) {
                    $scope.model = {};
                    $scope.form.$setPristine();
                },
                function (err) {
                }
                ).finally(function () {
                    ui.overlay.hide();
                    refresh();
                });

            }
            else {
                status.create($scope.model).then(
                function (response) {
                    $scope.model = {};
                    $scope.form.$setPristine();
                },
                function (err) {
                }
                ).finally(function () {
                    refresh();
                    ui.overlay.hide();
                });
            }
        }
        $scope.toggleActive = function (item) {
            ui.overlay.show();
            status.toggleActive(item.id).then(
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
        $scope.setDefault = function (item) {
            ui.overlay.show();
            status.setDefault(item.id).then(
                function (response) {
                    item.isDefault = true;
                    for (var i = 0; i < $scope.list.length; i++) {
                        if ($scope.list[i].id != item.id) {
                            $scope.list[i].isDefault = false;
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
        $scope.cancel = function () {
            angular.copy($scope.initialModel,$scope.model);
            $scope.form.$setPristine();

        }
    }
})();