/*
*
*/

(function () {
    'use strict';
    angular.module('app.main').controller('main.projects.ctrl', ctrl);

    ctrl.$inject = ['$filter', '$scope', '$state', 'core.settings', 'core.system.svc', 'ui.svc', 'main.projects.svc','main.clients.svc'];
    function ctrl($filter, $scope, $state, settings, system, ui, projects,clients) {
        ui.page.title = system.resources.views.projects_manage;

        var vm = this;
        vm.isFormMode = true;
        vm.model = { };

        if ($state.params.item && $state.params.item.id) {
            vm.model = angular.copy($state.params.item);
            ui.page.description = vm.model.title;
            setMode();
        } else if ($state.params.id) {
            ui.overlay.show();
            projects.get($state.params.id).then(
                function (response) {
                    $state.params.item = response.data.content;
                    vm.model = angular.copy($state.params.item);
                    ui.page.description = vm.model.title;
                },
                function () {
                }
            ).finally(function () {
                setMode();
                ui.overlay.hide();
            });
        }
        ui.overlay.show();
        clients.search().then(
            function (response) {
                vm.clients = response.data.content;
            },
            function () {
            }
        ).finally(function () {
            ui.overlay.hide();
        });

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
                clientId: vm.model.client.id,
                title: vm.model.title,
                code: vm.model.code,
                description: vm.model.description,
                remarks: vm.model.remarks
            }
        }
        function getUpdateModel() {
            return {
                id: vm.model.id,
                clientId: vm.model.client.id,
                title: vm.model.title,
                code: vm.model.code,
                description: vm.model.description,
                remarks: vm.model.remarks
            }
        }
        vm.save = function () {
            ui.overlay.show();
            if (vm.model.id) {
                projects.update(getUpdateModel()).then(
                function (response) {
                    $state.go('main/projects/list');
                },
                function (err) {
                    ui.alerts.http(err);
                }
                ).finally(function () {
                    ui.overlay.hide();
                });

            }
            else {
                projects.create(getCreateModel()).then(
                function (response) {
                    $state.go('main/projects/list');
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
                //vm.model.clientId = vm.model.client.id;
                vm.isFormMode = false;
                

            }
            else {
                $state.go('main/projects/list');
            }
        }        
        vm.addEnquiry = function () {
            $state.go('main/enquiries', { projectId: vm.model.id, project: angular.copy(vm.model), mode: 'form' });
        }
    }
})();