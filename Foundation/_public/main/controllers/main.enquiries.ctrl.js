/*
*
*/

(function () {
    'use strict';
    angular.module('app.main').controller('main.enquiries.ctrl', ctrl);

    ctrl.$inject = ['$filter', '$scope', '$state', 'core.settings', 'core.system.svc', 'ui.svc', 'main.enquiries.svc','main.membership.svc'];
    function ctrl($filter,$scope, $state, settings, system, ui, enquiries,membership) {
        if ($state.params.isTemplate)
            ui.page.title = system.resources.views.manage_templates;
        else
            ui.page.title = system.resources.views.manage_enquiries;


        
        var vm = this;
        vm.model = {};
        if ($state.params.item && $state.params.item.id) {
            vm.model = angular.copy($state.params.item);
            ui.page.description = vm.model.subject
                + (vm.model.referenceNumber ? ' ,Ref#[' + vm.model.referenceNumber + ']' : '')
                + (vm.model.revisionNumber ? ', Rev#[' + vm.model.revisionNumber + ']' : '');

        } else if ($state.params.id) {
            ui.overlay.show();
            enquiries.get($state.params.id).then(
                function (response) {
                    $state.params.item = response.data.content;
                    vm.model = angular.copy($state.params.item);
                    ui.page.description = vm.model.subject
                        + (vm.model.referenceNumber ? ' ,Ref#[' + vm.model.referenceNumber + ']' : '')
                        + (vm.model.revisionNumber ? ', Rev#[' + vm.model.revisionNumber + ']' : '');
                },
                function () {
                }
            ).finally(function () {
                ui.overlay.hide();
            });
        }
        else {
            vm.model.isTemplate = $state.params.isTemplate;
        }

        ui.overlay.show();
        membership.current().then(
            function (response) {
                vm.currentMembership = response.data.content;
                if (!$state.params.id && vm.currentMembership.consumption.enquiriesPerMonth >= vm.currentMembership.membershipPlan.maxEnquiriesPerMonth) {
                    if ($state.params.isTemplate)
                        $state.go('main/templates/list');
                    else
                        $state.go('main/enquiries/list');
                }
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