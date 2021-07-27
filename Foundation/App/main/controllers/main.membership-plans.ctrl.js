/*
*
*/

(function () {
    'use strict';
    angular.module('app.main').controller('main.membershipPlans.ctrl', ctrl);

    ctrl.$inject = ['$filter', '$scope', '$state', 'core.settings', 'core.system.svc', 'ui.svc', 'main.membershipPlans.svc'];
    function ctrl($filter, $scope, $state, settings, system, ui, membershipPlans) {
        ui.page.title = system.resources.views.membership_plans_manage;

        var vm = this;
        vm.isFormMode = true;
        vm.model = { };
        vm.photoFile = {};
        vm.photoFile_EN = {};

        if ($state.params.item && $state.params.item.id) {
            vm.model = angular.copy($state.params.item);
            
            

            setMode();
        } else if ($state.params.id) {
            ui.overlay.show();
            membershipPlans.get($state.params.id).then(
                function (response) {
                    $state.params.item = response.data.content;
                    vm.model = angular.copy($state.params.item);
                    vm.formattedStartDate = $filter('date')(vm.model.startDate, 'dd-MM-yyyy');
                    vm.formattedEndDate = $filter('date')(vm.model.endDate, 'dd-MM-yyyy');
                },
                function () {
                }
            ).finally(function () {
                setMode();
                ui.overlay.hide();
            });
        }
       
        function setMode(){
            if ($state.params.item && $state.params.item.id) {
                if (!$state.params.mode || $state.params.mode == 'view')
                    vm.isFormMode = false;
                else
                    vm.isFormMode = true;
            }
        }
        
        function getCreateModel() {
            return {
                title: vm.model.title,
                description: vm.model.description,
                maxEnquiriesPerMonth: vm.model.maxEnquiriesPerMonth,
                maxItemsPerEnquiry: vm.model.maxItemsPerEnquiry,
                maxInvitationsPerMonth: vm.model.maxInvitationsPerMonth,
                maxContactsPerInvitation: vm.model.maxContactsPerInvitation,
                maxItemFields: vm.model.maxItemFields,
                costPerMonth: vm.model.costPerMonth,
                costPerYear: vm.model.costPerYear,
                remarks: vm.model.remarks,
                isActive: vm.model.isActive,
                isPublic: vm.model.isPublic


            }
        }
        function getUpdateModel() {
            return {
                id: vm.model.id,
                title: vm.model.title,
                description: vm.model.description,
                maxEnquiriesPerMonth: vm.model.maxEnquiriesPerMonth,
                maxItemsPerEnquiry: vm.model.maxItemsPerEnquiry,
                maxInvitationsPerMonth: vm.model.maxInvitationsPerMonth,
                maxContactsPerInvitation: vm.model.maxContactsPerInvitation,
                maxItemFields: vm.model.maxItemFields,
                costPerMonth: vm.model.costPerMonth,
                costPerYear: vm.model.costPerYear,
                remarks: vm.model.remarks,
                isActive: vm.model.isActive,
                isPublic: vm.model.isPublic
            }
        }
        vm.save = function () {
            ui.overlay.show();
            if (vm.model.id) {
                membershipPlans.update(getUpdateModel()).then(
                function (response) {
                    $state.go('main/membership-plans/list');
                },
                function (err) {
                    ui.alerts.http(err);
                }
                ).finally(function () {
                    ui.overlay.hide();
                });

            }
            else {
                membershipPlans.create(getCreateModel()).then(
                function (response) {
                    $state.go('main/membership-plans/list');
                },
                function (err) {
                    ui.alerts.http(err);
                }
                ).finally(function () {
                    ui.overlay.hide();
                });
            }
        }
        vm.cancel = function () {
            if ($state.params.item && $state.params.item.id) {
                vm.model = angular.copy($state.params.item);
                vm.isFormMode = false;
                

            }
            else {
                $state.go('main/membership-plans/list');
            }
        }        
    }
})();