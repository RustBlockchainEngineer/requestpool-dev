/*
*
*/

(function () {
    'use strict';
    angular.module('app.main').controller('main.publicUsers.ctrl', ctrl);

    ctrl.$inject = ['$filter', '$scope', '$state', 'core.settings', 'core.system.svc', 'ui.svc', 'main.publicUsers.svc'];
    function ctrl($filter,$scope, $state, settings, system, ui, publicUsers) {
        ui.page.title = system.resources.views.public_users_manage;
        
        var vm = this;
        vm.isNewEntry=true;
        if ($state.params.item && $state.params.item.id) {
            vm.isNewEntry = false;
        } else if ($state.params.id) {
            ui.overlay.show();
            publicUsers.get($state.params.id).then(
                function (response) {
                    $state.params.item = response.data.content;
                    vm.isNewEntry = false;
                },
                function () {
                }
            ).finally(function () {
                ui.overlay.hide();
            });
        }

        
    }
})();