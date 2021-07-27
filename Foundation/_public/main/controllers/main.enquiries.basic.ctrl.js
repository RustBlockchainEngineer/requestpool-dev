/*
*
*/

(function () {
    'use strict';
    angular.module('app.main').controller('main.enquiries.basic.ctrl', ctrl);

    ctrl.$inject = ['$filter', '$scope', '$state', 'core.settings', 'core.system.svc', 'ui.svc', 'main.enquiries.svc',
        'main.projects.svc', 'main.enquiryTypes.svc', 'main.clients.svc', 'main.status.svc'];
    function ctrl($filter, $scope, $state, settings, system, ui, enquiries, projects, enquiryTypes, clients, status) {
        var vm = this;
        vm.viewMode = 'form';
        vm.clients = [];
        vm.projects = [];
        vm.model = {};

        if ($state.params.item && $state.params.item.id) {
            vm.model = angular.copy($state.params.item);
            setMode();
        } else if ($state.params.id) {
            ui.overlay.show();
            enquiries.get($state.params.id).then(
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
        else
        {
            vm.model.isTemplate = $state.params.isTemplate;
        }
        ui.overlay.show();
        enquiryTypes.search().then(
            function (response) {
                vm.enquiryTypes = response.data.content;
            },
            function () {
            }
        ).finally(function () {
            ui.overlay.hide();
        });

        ui.overlay.show();
        status.get().then(
            function (response) {
                vm.status = response.data.content;
            },
            function () {
            }
        ).finally(function () {
            ui.overlay.hide();
        });

        vm.searchClients = function (name) {
            /*if (!name)
                return;*/
            clients.search({ name: name }).then(
                function (response) {
                    vm.clients = response.data.content;
                },
                function () {
                }
            );
        }
        vm.searchProjects = function (title) {
            /*if (!title)
                return;*/
            projects.search({ title: title, clientId: vm.model.client ? vm.model.client.id : null }).then(
                function (response) {
                    vm.projects = response.data.content;
                },
                function () {
                }
            );
        }
        function setMode() {

            if ($state.params.item && $state.params.item.id) {
                if ($state.params.viewMode) {
                    vm.viewMode = $state.params.viewMode;
                }
                else {
                    vm.viewMode = 'view';
                }
            }
            else {
                vm.viewMode = 'form';
            }
        }

        function getCreateModel() {
            return {
                referenceNumber: vm.model.referenceNumber,
                revisionNumber: vm.model.revisionNumber,
                prNumber: vm.model.prNumber,
                boqNumber: vm.model.boqNumber,
                subject: vm.model.subject,
                description: vm.model.description,
                enquiryTypeId: vm.model.enquiryType.id,
                projectId: vm.model.client && vm.model.project?vm.model.project.id:null,
                clientId: vm.model.client?vm.model.client.id:null,
                statusId: vm.model.status ? vm.model.status.id : null,
                remarks: vm.model.remarks,
                isTemplate: vm.model.isTemplate

            }
        }
        function getUpdateModel() {
            return {
                id: vm.model.id,
                referenceNumber: vm.model.referenceNumber,
                revisionNumber: vm.model.revisionNumber,
                prNumber: vm.model.prNumber,
                boqNumber: vm.model.boqNumber,
                subject: vm.model.subject,
                description: vm.model.description,
                enquiryTypeId: vm.model.enquiryType.id,
                projectId: vm.model.client && vm.model.project ? vm.model.project.id : null,
                clientId: vm.model.client ? vm.model.client.id : null,
                statusId: vm.model.status ? vm.model.status.id : null,
                remarks: vm.model.remarks,
                isTemplate: vm.model.isTemplate
            }
        }
        vm.save = function () {
            ui.overlay.show();
            if (vm.model.id) {
                enquiries.update(getUpdateModel()).then(
                    function (response) {
                        $state.params.item = response.data.content;
                        vm.model = angular.copy($state.params.item);
                        
                        vm.viewMode = 'view';
                    },
                    function (err) {
                        ui.alerts.http(err);
                    }
                ).finally(function () {
                    ui.overlay.hide();
                });

            }
            else {
                enquiries.create(getCreateModel()).then(
                    function (response) {
                        $state.params.item = response.data.content;
                        vm.model = angular.copy($state.params.item);
                        vm.reload(vm.model);
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
                vm.viewMode = 'view';
            }
            else {
                if (vm.model.isTemplate)
                    $state.go('main/templates/list', { isTemplate: true });
                else
                    $state.go('main/enquiries/list', { isTemplate: false });

            }
        }
        vm.copy = function (item, model) {
            if (item)
                model.enquiryId = item.id;
            else if (vm.selectedItem)
                model.enquiryId = vm.selectedItem.id;
            else
                return;
            ui.overlay.show();
            enquiries.copy(model).then(
                function (response) {
                    edit(response.data.content);
                },
                function (err) {
                    ui.alerts.http(err);
                })
                .finally(function () {
                    ui.overlay.hide();
                });
        }
        function edit(item) {
            if (item.isTemplate)
                $state.go('main/templates', { isTemplate: true, id: item.id, item: angular.copy(item), viewMode: 'form' });
            else
                $state.go('main/enquiries', { isTemplate: false, id: item.id, item: angular.copy(item), viewMode: 'form' });

        }

        vm.reloadParent = function () {
            $state.go('main/enquiries', { isTemplate: false, id: vm.model.parent.id, item: null, viewMode: 'view' });
        }

        vm.reload = function (item) {
            if ($state.params.isTemplate)
                $state.go('main/templates', { isTemplate: true, id: item.id, item: item, viewMode: 'view' });
            else
                $state.go('main/enquiries', { isTemplate: false, id: item.id, item: item, viewMode: 'view' });
            
        }
    }
})();