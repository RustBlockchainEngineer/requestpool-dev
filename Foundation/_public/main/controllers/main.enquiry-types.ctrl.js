﻿/*
*
*/

(function () {
    'use strict';
    angular.module('app.core').controller('main.enquiryTypes.ctrl', ctrl);
     
    ctrl.$inject = ['$scope', '$state', 'core.system.svc', 'core.settings', '$uibModal', 'ui.svc', 'main.enquiryTypes.svc'];

    function ctrl($scope, $state,system, settings,$uibModal, ui,enquiryTypes) {
        ui.page.title = system.resources.views.enquiry_types;
        
        $scope.search = { isDeleted: false };

        $scope.initialModel = { name: '' };

        $scope.model = {};

        $scope.showSearch = false;
        $scope.toggleSearch = function () {
            $scope.showSearch = !$scope.showSearch;
        }

        refresh();
        function refresh() {
            ui.overlay.show();
            enquiryTypes.search().then(
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
        }

        $scope.remove = function (item) {
            ui.overlay.show();
            enquiryTypes.remove(item.id).then(
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
                enquiryTypes.update($scope.model).then(
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
                enquiryTypes.create($scope.model).then(
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