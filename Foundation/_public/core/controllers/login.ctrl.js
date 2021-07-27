/*
*
*/

(function () {
    'use strict';
    angular.module('app.core').controller('core.login.ctrl', Ctrl);

    Ctrl.$inject = ['$geolocation', '$rootScope', '$scope', '$state', 'core.settings', 'core.account.svc', 'ui.svc'];
    function Ctrl($geolocation, $rootScope, $scope, $state, settings, account, ui) {
        //ui.overlay.show('#main-content');
        var vm = this;
        vm.login = function () {
            ui.overlay.show();
            account.login(vm.model).then(
                function (result) {
                    $geolocation.getCurrentPosition({
                        timeout: 60000,
                        enableHighAccuracy: true
                    }).then(function (position) {
                        vm.currentLocation = { latitude: position.coords.latitude, longitude: position.coords.longitude };
                    }, function (positionError) {
                        vm.currentLocation = {};

                    })
                    .finally(function () {
                        account.updateLocation(vm.currentLocation);
                    });

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

        vm.register = function () {
            ui.overlay.show();
            account.register(vm.model).then(
                function (result) {
                    vm.login();
                },
                function (err) {
                    console.log(err);
                    ui.alerts.add('Error', err.error, 'danger');
                })
                .finally(function () {
                    ui.overlay.hide();
                });
        }

        vm.requestOtp = function (purpose) {
            ui.overlay.show();
            account.requestOtp(vm.model.username, purpose).then(
                function (result) {
                    ui.alerts.add('Success', 'Check Your Email', 'info');
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