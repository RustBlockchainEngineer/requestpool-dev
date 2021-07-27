/*
*
*/

(function () {
    'use strict';
    angular.module('app.main').controller('main.clients.ctrl', ctrl);

    ctrl.$inject = ['$filter', '$scope', '$state', 'core.settings', 'core.system.svc', 'ui.svc', 'main.clients.svc'];
    function ctrl($filter, $scope, $state, settings, system, ui, clients) {
        ui.page.title = system.resources.views.clients_manage;

        var vm = this;
        vm.isFormMode = true;
        vm.model = { };

        if ($state.params.item && $state.params.item.id) {
            vm.model = angular.copy($state.params.item);
            ui.page.description = vm.model.name;
            setMode();
        } else if ($state.params.id) {
            ui.overlay.show();
            clients.get($state.params.id).then(
                function (response) {
                    $state.params.item = response.data.content;                    
                    vm.model = angular.copy($state.params.item);
                    ui.page.description = vm.model.name;
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
                title: vm.model.title,
                name: vm.model.name,
                phone: vm.model.phone,
                fax: vm.model.fax,
                email: vm.model.email,
                address: vm.model.address,
                website: vm.model.website,
                profile: vm.model.profile,
                remarks: vm.model.remarks
            }
        }
        function getUpdateModel() {
            return {
                id: vm.model.id,
                title: vm.model.title,
                name: vm.model.name,
                phone: vm.model.phone,
                fax: vm.model.fax,
                email: vm.model.email,
                address: vm.model.address,
                website: vm.model.website,
                profile: vm.model.profile,
                remarks: vm.model.remarks
            }
        }
        vm.save = function () {
            ui.overlay.show();
            if (vm.model.id) {
                clients.update(getUpdateModel()).then(
                function (response) {
                    $state.go('main/clients/list');
                },
                function (err) {
                    ui.alerts.http(err);
                }
                ).finally(function () {
                    ui.overlay.hide();
                });

            }
            else {
                clients.create(getCreateModel()).then(
                function (response) {
                    $state.go('main/clients/list');
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
                $state.go('main/clients/list');
            }
        }        
    }
})();