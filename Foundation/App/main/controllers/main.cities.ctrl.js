﻿/*
*
*/

(function () {
    'use strict';
    angular.module('app.core').controller('main.cities.ctrl', ctrl);
     
    ctrl.$inject = ['$scope', '$state', 'core.system.svc', 'core.settings', '$uibModal', 'ui.svc', 'main.cities.svc','main.countries.svc'];

    function ctrl($scope, $state,system, settings,$uibModal, ui,cities,countries) {
        ui.page.title = system.resources.views.cities;
        
        $scope.search = { isDeleted: false };

        $scope.initialModel = { name: '', countryId: '' };

        $scope.model = {};

        $scope.showSearch = false;
        $scope.toggleSearch = function () {
            $scope.showSearch = !$scope.showSearch;
        }
        ui.overlay.show();
        countries.search().then(
            function (response) {
                $scope.countries = response.data.content;
            },
            function () {
            }
        ).finally(function () {
            ui.overlay.hide();
        });
        refresh();
        function refresh() {
            ui.overlay.show();
            cities.search().then(
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
            
            angular.copy(item, $scope.model);
            $scope.model.countryId = item.country.id;
        }

        $scope.remove = function (item) {
            ui.overlay.show();
            cities.remove(item.id).then(
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
                cities.update($scope.model).then(
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
                cities.create($scope.model).then(
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
            cities.toggleActive(item.id).then(
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
        $scope.cancel = function () {
            angular.copy($scope.initialModel,$scope.model);
            $scope.form.$setPristine();

        }
    }
})();