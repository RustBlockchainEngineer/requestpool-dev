/*
*
*/

(function () {
    'use strict';
    angular.module('app.core').controller('core.login.ctrl', Ctrl);

    Ctrl.$inject = ['$rootScope', '$scope', '$state', 'core.settings', 'core.account.svc', 'ui.svc'];
    function Ctrl($rootScope, $scope, $state, settings, account, ui) {

        $scope.login = function () {
            ui.overlay.show();
            account.login($scope.model).then(
            function (result) {
                $rootScope.$broadcast('authentication');
                $state.go('core/home');
            },
            function (err) {
                console.log(err);
                ui.alerts.add('Error', err.error, 'danger');
            })
            .finally(function () {
                ui.overlay.hide();
            });
        }
    }
})();