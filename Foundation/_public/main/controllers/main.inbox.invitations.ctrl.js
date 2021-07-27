/*
*
*/

(function () {
    'use strict';
    angular.module('app.main').controller('main.inbox.invitations.ctrl', ctrl);

    ctrl.$inject = ['$filter', '$scope', '$state', 'core.settings', 'core.system.svc', 'ui.svc', 'main.invitations.svc'];
    function ctrl($filter, $scope, $state, settings, system, ui, invitations) {
        ui.page.title = system.resources.views.inbox;
        
        var vm = this;
        vm.isNewEntry=true;
        if ($state.params.item && $state.params.item.id) {
            vm.model = $state.params.item;
            ui.page.description = vm.model.subject;
            
        } else if ($state.params.id) {
            ui.overlay.show();
            invitations.incomingById($state.params.id).then(
                function (response) {
                    $state.params.item = response.data.content;
                    vm.model = $state.params.item;
                    ui.page.description = vm.model.subject;
                },
                function () {
                }
            ).finally(function () {
                ui.overlay.hide();
            });
        }

        vm.setActiveTab = function (index) {
            console.log('tab' + index);
            vm.activeTab = index;
        }

        
    }
})();