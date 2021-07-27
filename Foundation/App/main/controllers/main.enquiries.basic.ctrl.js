/*
*
*/

(function () {
    'use strict';
    angular.module('app.main').controller('main.enquiries.basic.ctrl', ctrl);

    ctrl.$inject = ['$filter', '$scope', '$state', 'core.settings', 'core.system.svc', 'ui.svc', 'main.enquiries.svc',
        'main.projects.svc', 'main.enquiryTypes.svc', 'main.clients.svc', 'main.status.svc'];
    function ctrl($filter, $scope, $state, settings, system, ui, enquiries, projects, enquiryTypes, clients,status) {
        var vm = this;
        vm.viewMode = 'view';
        vm.clients = [];
        vm.projects = [];
        vm.model = {};

        if ($state.params.item && $state.params.item.id) {
            vm.model = angular.copy($state.params.item);
        } else if ($state.params.id) {
            ui.overlay.show();
            enquiries.get($state.params.id).then(
                function (response) {
                    $state.params.item = response.data.content;
                    vm.model = angular.copy($state.params.item);
                },
                function (err) {
                }
            ).finally(function () {
                ui.overlay.hide();
            });
        }
        else
        {
            if ($state.params.isTemplate)
                $state.go('main/templates/list');
            else
                $state.go('main/enquiries/list');
        }
        ui.overlay.show();
        enquiryTypes.search().then(
            function (response) {
                vm.enquiryTypes = response.data.content;
            },
            function () {
            }
        ).finally(function () {
            ui.overlay.hide();
        });
        ui.overlay.show();
        status.get().then(
            function (response) {
                vm.status = response.data.content;
            },
            function () {
            }
        ).finally(function () {
            ui.overlay.hide();
        });

        vm.searchClients = function (name) {
            if (!name)
                return;
            clients.search({ name: name }).then(
                function (response) {
                    vm.clients = response.data.content;
                },
                function () {
                }
            );
        }
        vm.searchProjects = function (title) {
            if (!title)
                return;
            projects.search({ title: title, clientId: vm.model.client ? vm.model.client.id : null }).then(
                function (response) {
                    vm.projects = response.data.content;
                },
                function () {
                }
            );
        }

        vm.reloadParent = function () {
            $state.go('main/enquiries', { isTemplate: false, id: vm.model.parent.id, item: null, viewMode: 'view' });
        }
    }
})();