/*
*
*/

(function () {
    'use strict';
    angular.module('app.main').controller('main.itemsProperties.ctrl', ctrl);
     
    ctrl.$inject = ['$scope', '$state', 'core.system.svc', 'core.settings', '$uibModal', 'ui.svc', 'main.propertyTypes.svc',
        'main.itemsProperties.svc'];

    function ctrl($scope, $state,system, settings,$uibModal, ui,propertyTypes,itemsProperties) {
        ui.page.title = system.resources.views.item_properties;
        
        $scope.search = { isDeleted: false };

        $scope.initialModel = { name: '' ,propertyType:null};

        $scope.model = {};

        $scope.showSearch = false;
        $scope.toggleSearch = function () {
            $scope.showSearch = !$scope.showSearch;
        }

        ui.overlay.show();
        propertyTypes.all().then(
            function (response) {
                $scope.propertyTypes = response.data.content;
            },
            function () {
            }
        ).finally(function () {
            ui.overlay.hide();
        });
        refresh();
        function refresh() {
            ui.overlay.show();
            itemsProperties.search().then(
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
            
            $scope.model = angular.copy(item);
            $scope.model.propertyTypeId = item.propertyType.id;
        }

        $scope.remove = function (item) {
            ui.overlay.show();
            itemTypes.remove(item.id).then(
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
            $scope.model.propertyTypeId = $scope.model.propertyType.id;
            if ($scope.model.id) {
                itemsProperties.update($scope.model).then(
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
                itemsProperties.create($scope.model).then(
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
        $scope.cancel = function () {
            angular.copy($scope.initialModel,$scope.model);
            $scope.form.$setPristine();

        }
    }
})();