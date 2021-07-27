/*
*
*/

(function () {
    'use strict';
    angular.module('app.main').controller('main.contacts.ctrl', ctrl);

    ctrl.$inject = ['$filter', '$scope', '$state', 'core.settings', 'core.system.svc', 'ui.svc', 'main.contacts.svc',
    'main.contactTypes.svc'];
    function ctrl($filter, $scope, $state, settings, system, ui, contacts, contactTypes) {
        ui.page.title = system.resources.views.contacts_manage;

        var vm = this;
        vm.viewMode = 'view';
        vm.model = { };

        if ($state.params.item && $state.params.item.id) {
            vm.model = angular.copy($state.params.item);
        } else if ($state.params.id) {
            ui.overlay.show();
            contacts.get($state.params.id).then(
                function (response) {
                    $state.params.item = response.data.content;
                    vm.model = angular.copy($state.params.item);
                },
                function () {
                }
            ).finally(function () {
                ui.overlay.hide();
            });
        }
        else {
            $state.go('main/contacts/list');
        }
       
       
        ui.overlay.show();
        contactTypes.search().then(
            function (response) {
                vm.contactTypes = response.data.content;
            },
            function (err) {
                ui.alerts.http(err);
            }
        ).finally(function () {
            ui.overlay.hide();
        });
             
    }
})();