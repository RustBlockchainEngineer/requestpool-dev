/*
*
*/

(function () {
    'use strict';
    angular.module('app.main').controller('main.publicUsers.membership.current.ctrl', ctrl);
     
    ctrl.$inject = ['$filter','$scope', '$state', 'core.system.svc', 'core.settings', '$uibModal', 'ui.svc',
        'main.membership.svc', 'main.publicUsers.svc'];

    function ctrl($filter,$scope, $state, system, settings, $uibModal, ui, membership,publicUsers) {
        ui.page.title = system.resources.views.membership;

        var vm = this;
        
        vm.model = {}
        if ($state.params.item && $state.params.item.id) {
            vm.model.publicUserId = $state.params.item.id;
            refresh();

        } else if ($state.params.id) {
            ui.overlay.show();
            publicUsers.get($state.params.id).then(
                function (response) {
                    $state.params.item = response.data.content;
                    vm.model.publicUserId = $state.params.item.id;
                    refresh();
                },
                function () {
                }
            ).finally(function () {
                ui.overlay.hide();
            });
        }

       

      

        function refresh() {
            ui.overlay.show();
            membership.current(vm.model.publicUserId).then(
                function (response) {
                    vm.current = response.data.content;
                },
                function () {
                }
            ).finally(function () {
                ui.overlay.hide();
            });
        }

      
    }
})();