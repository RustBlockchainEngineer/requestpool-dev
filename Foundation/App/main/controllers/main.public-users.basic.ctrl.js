/*
*
*/

(function () {
    'use strict';
    angular.module('app.main').controller('main.publicUsers.basic.ctrl', ctrl);

    ctrl.$inject = ['$filter', '$scope', '$state', 'core.settings', 'core.system.svc', 'ui.svc', 'main.publicUsers.svc'];
    function ctrl($filter,$scope, $state, settings, system, ui, publicUsers) {
        ui.page.title = system.resources.views.publicUsers_manage;

        var vm = this;
        vm.isFormMode = true;
        vm.model = { };
        $scope.photoFile = {};
        if ($state.params.item && $state.params.item.id) {
            vm.model = angular.copy($state.params.item);
            setMode();
        } else if ($state.params.id) {
            ui.overlay.show();
            publicUsers.get($state.params.id).then(
                function (response) {
                    $state.params.item = response.data.content;
                    vm.model = angular.copy($state.params.item);
                },
                function () {
                }
            ).finally(function () {
                setMode();
                ui.overlay.hide();
            });
        }
        function setMode(){
            if ($state.params.item && $state.params.item.id) {
                if (!$state.params.mode || $state.params.mode == 'view')
                    vm.isFormMode = false;
                else
                    vm.isFormMode = true;
            }
        }
        
        function getCreateModel() {
            return {
                username: vm.model.username,
                phoneNumber: vm.model.phoneNumber,
                name: vm.model.name,
                remarks: vm.model.remarks,
                isActive: vm.model.isActive,
                photoFile: $scope.photoFile.content ? $scope.photoFile : null

            }
        }
        function getUpdateModel() {
            return {
                id: vm.model.id,
                username: vm.model.username,
                phoneNumber: vm.model.phoneNumber,
                name: vm.model.name,
                remarks: vm.model.remarks,
                isActive: vm.model.isActive,
                photoFile: $scope.photoFile.content ? $scope.photoFile : null
            }
        }
        vm.save = function () {
            ui.overlay.show();
            if (vm.model.id) {
                publicUsers.update(getUpdateModel()).then(
                function (response) {
                    $state.go('main/public-users/list');
                },
                function (err) {
                    ui.alerts.http(err);
                }
                ).finally(function () {
                    ui.overlay.hide();
                });

            }
            else {
                publicUsers.create(getCreateModel()).then(
                function (response) {
                    $state.go('main/public-users/list');
                },
                function (err) {
                    ui.alerts.http(err);
                }
                ).finally(function () {
                    ui.overlay.hide();
                });
            }
        }
        vm.cancel = function () {
            if ($state.params.item && $state.params.item.id) {
                vm.model = angular.copy($state.params.item);
                $scope.photoFile = {};
                vm.isFormMode = false;
            }
            else {
                $state.go('main/public-users/list');
            }
        }
        vm.removePhoto = function () {
            ui.overlay.show();
            publicUsers.removePhoto(vm.model.id).then(
                function (response) {
                    vm.model.photo = vm.model.photoUrl = null;
                    $scope.photoFile = {};
                },
                function (err) {
                    ui.alerts.http(err);
                }
            ).finally(function () {
                ui.overlay.hide();
            });
        }
        vm.resetPhoto = function () {
            $scope.photoFile = {};
        }

        vm.updateActivationCode = function () {
            ui.overlay.show();
            publicUsers.updateActivationCode(vm.model.id).then(
                function (response) {
                    vm.model.otp = response.data.content;
                },
                function (err) {
                    ui.alerts.http(err);
                }
            ).finally(function () {
                ui.overlay.hide();
            });
        }
        

        $scope.$watch('photoFile.content', function () {
            if ($scope.photoFile.content)
                vm.model.photoUrl = $scope.photoFile.header + ',' + $scope.photoFile.content;
            else if ($state.params.item)
                vm.model.photoUrl = $state.params.item.photoUrl;
            else
                vm.model.photoUrl = null;
        });

    }
})();