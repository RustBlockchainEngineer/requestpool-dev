/*
*
*/

(function () {
    'use strict';
    angular.module('app.core').controller('core.administrators.ctrl', ctrl);

    ctrl.$inject = ['$scope', '$state', 'core.settings','core.system.svc', 'ui.svc', 'core.administrators.svc'];
    function ctrl($scope, $state, settings,system, ui, administrators) {
        ui.page.title = system.resources.views.administrators_manage;

        var vm = this;
        vm.isFormMode = true;
        vm.model = {};

        vm.model = { applicationUser: {} };
        if ($state.params.item && $state.params.item.id) {
            vm.model = $state.params.item;
            setMode();
        }
        else if ($state.params.id) {
            ui.overlay.show();
            administrators.get($state.params.id).then(
                function (response) {
                    $state.params.item = response.data.content;
                    vm.model = response.data.content;
                },
                function () {
                }
            ).finally(function () {
                setMode();
                ui.overlay.hide();
            });
        }
        ui.overlay.show();
        administrators.getAllRoles().then(
            function (response) {
                vm.roles = response.data.content;
            },
            function (err) {
                ui.alerts.http(err);
            }
        ).finally(function () {
            ui.overlay.hide();
        }); 

        function setMode() {
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
                email: vm.model.email,
                phoneNumber: vm.model.phoneNumber,
                password: vm.model.password,
                confirmPassword: vm.model.confirmPassword,
                roles: vm.model.roles,
                name: vm.model.name,
                isActive: vm.model.isActive
            }
        }
        function getUpdateModel() {
            return {
                id: vm.model.id,
                username: vm.model.username,
                email: vm.model.email,
                phoneNumber: vm.model.phoneNumber,
                isUpdatePassword: vm.model.isUpdatePassword,
                password: vm.model.password,
                confirmPassword: vm.model.confirmPassword,
                roles: vm.model.roles,
                name: vm.model.name,
                isActive: vm.model.isActive
            }
        }
        vm.save = function () {
            ui.overlay.show();
            if (vm.model.id) {
                administrators.update(getUpdateModel()).then(
                function (response) {
                    $state.go('core/administrators/list');
                },
                function (err) {
                    ui.alerts.http(err);
                }
                ).finally(function () {
                    ui.overlay.hide();
                });

            }
            else {
                administrators.create(getCreateModel()).then(
                function (response) {
                    $state.go('core/administrators/list');
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
                vm.isFormMode = false;
            }
            else {
                $state.go('core/administrators/list');
            }
            
        }
    }
})();