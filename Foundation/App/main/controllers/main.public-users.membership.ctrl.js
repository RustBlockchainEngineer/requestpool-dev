/*
*
*/

(function () {
    'use strict';
    angular.module('app.main').controller('main.publicUsers.membership.ctrl', ctrl);
     
    ctrl.$inject = ['$filter','$scope', '$state', 'core.system.svc', 'core.settings', '$uibModal', 'ui.svc',
        'main.membership.svc', 'main.publicUsers.svc', 'main.membershipPlans.svc'];

    function ctrl($filter,$scope, $state, system, settings, $uibModal, ui, membership,publicUsers,membershipPlans) {
        ui.page.title = system.resources.views.membership;

        var vm = this;
        vm.initialModel = {
            startDate: null, endDate: null, membershipPlanId: null, downgradeToMembershipPlanId:null,
            formattedStartDate: null, formattedEndDate: null
        };
        vm.model = angular.copy(vm.initialModel);
        if ($state.params.item && $state.params.item.id) {
            vm.model.publicUserId = $state.params.item.id;
            vm.initialModel.publicUserId = $state.params.item.id;
            refresh();

        } else if ($state.params.id) {
            ui.overlay.show();
            publicUsers.get($state.params.id).then(
                function (response) {
                    $state.params.item = response.data.content;
                    vm.model.publicUserId = $state.params.item.id;
                    vm.initialModel.publicUserId = $state.params.item.id;
                    refresh();
                },
                function () {
                }
            ).finally(function () {
                ui.overlay.hide();
            });
        }
        vm.search = { isDeleted: false };
        vm.showSearch = true;

        ui.overlay.show();
        membershipPlans.all().then(
            function (response) {
                vm.membershipPlans = response.data.content;
                if (!vm.model.id && vm.membershipPlans.length > 0) {
                    vm.model.membershipPlan = { id: vm.membershipPlans[0].id };
                }
            },
            function (err) {
                ui.alerts.http(err);
            }
        ).finally(function () {
            ui.overlay.hide();
        });

        function refresh() {
            ui.overlay.show();
            membership.getByUser(vm.model.publicUserId).then(
                function (response) {
                    vm.list = response.data.content;
                },
                function () {
                }
            ).finally(function () {
                ui.overlay.hide();
            });
        }

        vm.edit = function (item) {
            vm.editMode = true;
            vm.model = angular.copy(item);
            vm.model.publicUserId = item.publicUser.id;
            vm.model.membershipPlanId = item.membershipPlan.id;
            vm.model.downgradeToMembershipPlanId = item.downgradeToMembershipPlan.id;
            vm.model.formattedStartDate = $filter('date')(vm.model.startDate, 'dd-MM-yyyy');
            vm.model.formattedEndDate = $filter('date')(vm.model.endDate, 'dd-MM-yyyy');

        }
        vm.remove = function (item) {
            ui.overlay.show();
            membership.remove(item.id).then(
                function (response) {
                    var selectedIndex = -1;
                    for (var i = 0; i < vm.list.length; i++) {
                        if (vm.list[i].id == item.id) {
                            selectedIndex = i;
                            break;
                        }
                    }
                    if (selectedIndex > -1) {
                        vm.list.splice(selectedIndex, 1);
                    }
                },
                function (err) {
                    ui.alerts.http(err);

                })
                .finally(function () {
                    ui.overlay.hide();
                });
        }

        vm.save = function () {
            ui.overlay.show();
            if (vm.model.id) {
                membership.update(vm.model).then(
                    function (response) {
                        reset();
                    },
                    function (err) {
                        ui.alerts.http(err);
                    }
                ).finally(function () {
                    ui.overlay.hide();
                    refresh();
                });

            }
            else {
                membership.create(vm.model).then(
                    function (response) {
                        reset();
                    },
                    function (err) {
                        ui.alerts.http(err);
                    }
                ).finally(function () {
                    refresh();
                    ui.overlay.hide();
                });
            }
        }


        vm.cancel = function () {

            reset();
        }

        function reset() {
            vm.model = angular.copy(vm.initialModel);
            vm.model.publicUserId = $state.params.item.id;
            $scope.form.$setPristine();
            vm.editMode = false;
        }

        

    }
})();