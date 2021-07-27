/*
*
*/

(function () {
    'use strict';
    angular.module('app.core').controller('core.profile.ctrl', ctrl);

    ctrl.$inject = ['$filter', '$scope', '$state', 'core.settings', 'core.system.svc', 'ui.svc', 'core.account.svc'];
    function ctrl($filter, $scope, $state, settings, system, ui, account) {
        ui.page.title = system.resources.views.profile;

        var vm = this;
        
        vm.updatePassword = function () {
            ui.overlay.show();
            account.updatePassword(vm.passwordModel).then(

                function (response) {
                    vm.passwordModel = {};
                    vm.reset();
                },
                function (err) {
                    ui.alerts.http(err);
                }
            ).finally(function () {
                ui.overlay.hide();
                $state.go('core/home');
            });

        }
        
        vm.reset = function () {
            vm.passwordModel = {};
            vm.passwordForm.$setPristine();
        }   
    }
})();