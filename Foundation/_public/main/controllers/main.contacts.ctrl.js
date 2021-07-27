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
        vm.isFormMode = true;
        vm.model = { };

        if ($state.params.item && $state.params.item.id) {
            vm.model = angular.copy($state.params.item);
            setMode();
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
        ui.overlay.show();
        contactTypes.search().then(
            function (response) {
                vm.contactTypes = response.data.content;

                if (vm.contactTypes.length > 0) {
                    vm.model.companyName = vm.contactTypes[0].publicUser.companyName;
                }

                vm.selectedContactTypes = new Array();
                
                if (vm.model.contactTypes != undefined) {
                    vm.selectedContactTypes = vm.model.contactTypes;
                }
                vm.dropdownSetting = {
                    scrollable: true,
                    scrollableHeight: '250px'
                }
            },
            function (err) {
                ui.alerts.http(err);
            }
        ).finally(function () {
            ui.overlay.hide();
        });
        function convertContactTypesToString(selectedContactTypes) {
            var result = "";
            for (var i = 0; i < selectedContactTypes.length; i++) {
                if (i == 0) {
                    result += selectedContactTypes[i].id;
                }
                else {
                    result += "," + selectedContactTypes[i].id;
                }
            }
            return result;
        }
        function getCreateModel() {
            return {
                //contactTypeId: vm.model.contactType.id,
                contactTypeId: vm.selectedContactTypes[0].id,
                title: vm.model.title,
                name: vm.model.name,
                phone: vm.model.phone,
                fax: vm.model.fax,
                email: vm.model.email,
                address: vm.model.address,
                companyName: vm.model.companyName,
                profile: vm.model.profile,
                remarks: vm.model.remarks,
                contactTypes: convertContactTypesToString(vm.selectedContactTypes)
            }
        }
        function getUpdateModel() {
            return {
                id: vm.model.id,
                contactTypeId: vm.selectedContactTypes[0].id,
                title: vm.model.title,
                name: vm.model.name,
                phone: vm.model.phone,
                fax: vm.model.fax,
                email: vm.model.email,
                address: vm.model.address,
                companyName: vm.model.companyName,
                profile: vm.model.profile,
                remarks: vm.model.remarks,
                contactTypes: convertContactTypesToString(vm.selectedContactTypes)
            }
        }
        vm.save = function () {
            ui.overlay.show();
            if (vm.model.id) {
                contacts.update(getUpdateModel()).then(
                function (response) {
                    $state.go('main/contacts/list');
                },
                function (err) {
                    ui.alerts.http(err);
                }
                ).finally(function () {
                    ui.overlay.hide();
                });

            }
            else {
                contacts.create(getCreateModel()).then(
                    function (response) {
                        
                    $state.go('main/contacts/list');
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
                $state.go('main/contacts/list');
            }
        }        
    }
})();