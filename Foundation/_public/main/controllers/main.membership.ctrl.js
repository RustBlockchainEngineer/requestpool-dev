/*
*
*/

(function () {
    'use strict';
    angular.module('app.main').controller('main.membership.ctrl', ctrl);

    ctrl.$inject = ['$filter', '$scope', '$state', 'core.settings', 'core.system.svc', 'ui.svc', 'main.membership.svc'];
    function ctrl($filter, $scope, $state, settings, system, ui, membership) {
        ui.page.title = system.resources.views.membership;

        var vm = this;
        vm.model = {};

        ui.overlay.show();
        membership.all().then(
            function (response) {
                vm.list = response.data.content;
            },
            function () {
            }
        ).finally(function () {
            ui.overlay.hide();
        });

        ui.overlay.show();
        membership.current().then(
            function (response) {
                vm.current = response.data.content;
            },
            function () {
            }
        ).finally(function () {
            ui.overlay.hide();
        });

        ui.overlay.show();
        membership.consumption().then(
            function (response) {
                vm.consumption = response.data.content;
            },
            function () {
            }
        ).finally(function () {
            ui.overlay.hide();
        });


        vm.setActiveTab = function (index) {
            vm.activeTab = index;
        }


    }
})();