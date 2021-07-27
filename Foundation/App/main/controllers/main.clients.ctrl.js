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
        vm.viewMode = 'view';
        vm.model = { };

        if ($state.params.item && $state.params.item.id) {
            vm.model = angular.copy($state.params.item);
            ui.page.description = vm.model.name;
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
                ui.overlay.hide();
            });
        }
        else {
            $state.go('main/clients/list');
        }
       
    }
})();