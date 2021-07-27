/*
*
*/

(function () {
    'use strict';
    angular.module('app.main').controller('main.inbox.invitations.info.ctrl', ctrl);

    ctrl.$inject = ['$filter', '$scope', '$state', 'core.settings', 'core.system.svc', 'ui.svc', 'main.invitations.svc'];
    function ctrl($filter, $scope, $state, settings, system, ui, invitations) {
        ui.page.title = system.resources.views.inbox_invitations;

        var vm = this;
        vm.viewMode = 'view';
        vm.model = {};
        
        if ($state.params.item && $state.params.item.id) {
            vm.model = angular.copy($state.params.item);            
            setMode();
        } else if ($state.params.id) {
            ui.overlay.show();
            invitations.incomingById($state.params.id).then(
                function (response) {
                    $state.params.item = response.data.content;
                    vm.model = angular.copy($state.params.item);                    
                },
                function (err) {
                }
            ).finally(function () {
                setMode();
                ui.overlay.hide();
            });
        }
        function setMode(){
            if ($state.params.item && $state.params.item.id) {
                if ($state.params.mode)
                    vm.viewMode = $state.params.mode;
                else
                    vm.viewMode = "view";
            }
        }
    }
})();