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
        vm.viewMode = 'view';
        vm.model = { };

        if ($state.params.item && $state.params.item.id) {
            vm.model = angular.copy($state.params.item);
            ui.page.description = vm.model.title;
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
                ui.overlay.hide();
            });
        }
        else {
            $state.go('main/projects/list');
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

        
        
       
       
    }
})();