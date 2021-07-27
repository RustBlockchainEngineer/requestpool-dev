

/*
*
*/

(function () {
    'use strict';
    angular.module('app.main').controller('main.enquiries.invitations.ctrl', ctrl);

    ctrl.$inject = ['$filter','$rootScope','$scope', '$state', 'core.system.svc', 'core.settings', '$uibModal', 'ui.svc',
        'main.enquiries.svc', 'main.invitations.svc', 'main.contacts.svc', 'main.contactTypes.svc', 'main.membership.svc'];

    function ctrl($filter,$rootScope,$scope, $state, system, settings, $uibModal, ui, enquiries, invitations, contacts, contactTypes,membership) {
        ui.page.title = system.resources.views.invitations;

        var vm = this;
        vm.viewMode = 'list';
        vm.model = {};
        vm.search = { isDeleted: false,enquiryId:-1 };
        vm.showSearch = true;
        vm.toggleSearch = function () {
            vm.showSearch = !vm.showSearch;
        }
        vm.now = new Date().toISOString();
        
        //get enquiry
        if ($state.params.item && $state.params.item.id) {
            vm.model.enquiry = $state.params.item;
            vm.search.enquiryId = vm.model.enquiry.id;

        } else if ($state.params.id) {
            ui.overlay.show();
            enquiries.get($state.params.id).then(
                function (response) {
                    $state.params.item = response.data.content;
                    vm.model.enquiry = $state.params.item;
                    vm.search.enquiryId = vm.model.enquiry.id;
                },
                function () {
                }
            ).finally(function () {
                ui.overlay.hide();
            });
        }


        ui.overlay.show();
        membership.current().then(
            function (response) {
                vm.currentPlan = response.data.content.membershipPlan;
            },
            function () {
            }
        ).finally(function () {
            ui.overlay.hide();
        });



        vm.edit = function (item) {
            vm.model = {};
            vm.model.enquiry = $state.params.item;
            if (item) {
                vm.model = angular.copy(item);
                vm.model.formattedEndDate = $filter('date')(vm.model.endDate, 'dd-MM-yyyy');
            }
            else {
                vm.model.isDraft = true;
            }
            vm.viewMode = 'form';
        }
        vm.view = function (item) {
            vm.model = angular.copy(item);
            vm.model.formattedEndDate = $filter('date')(vm.model.endDate, 'dd-MM-yyyy');
            vm.viewMode = 'view';
        }
        vm.list = function () {
            vm.model = {};
            vm.viewMode = 'list';
            refresh('invitations');

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
        ui.overlay.show();
        contacts.all().then(
            function (response) {
                vm.contacts = response.data.content;
            },
            function (err) {
                ui.alerts.http(err);
            }
        ).finally(function () {
            ui.overlay.hide();
        });
        function getCreateModel(isDraft) {
            return {
                enquiryId: vm.model.enquiry.id,
                subject: vm.model.subject,
                endDate: vm.model.endDate,
                description: vm.model.description,
                isDraft: isDraft,
                contacts: getContactsIds(vm.model.recipients)
            }
        }
        function getUpdateModel(isDraft) {
            return {
                id: vm.model.id,
                enquiryId: vm.model.enquiry.id,
                subject: vm.model.subject,
                endDate: vm.model.endDate,
                description: vm.model.description,
                isDraft: isDraft,
                contacts: getContactsIds(vm.model.recipients)
            }
        }
        function getContactsIds(recipients) {
            var ids = new Array();
            if (recipients != null) {
                try {
                    for (var i = 0; i < recipients.length; i++) {
                        ids.push(recipients[i].contactId);
                    }

                } catch (exp) { }
            }
            return ids;
        }
        vm.save = function (isDraft) {
            ui.overlay.show();
            if (vm.model.id) {
                invitations.update(getUpdateModel(isDraft)).then(
                    function (response) {
                        vm.view(response.data.content)
                    },
                    function (err) {
                        ui.alerts.http(err);
                    }
                ).finally(function () {
                    ui.overlay.hide();
                });

            }
            else {
                invitations.create(getCreateModel(isDraft)).then(
                    function (response) {
                        vm.view(response.data.content)
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
            vm.model = {};
            vm.list();
        }

        vm.remove = function (item) {
            ui.overlay.show();
            invitations.remove(item.id).then(
                function (response) {
                    var selectedIndex = -1;
                    for (var i = 0; i < vm.list.length; i++) {
                        if (vm.displayedList[i].id == item.id) {
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
        vm.addContact = function (item) {
            if (!vm.model.recipients) 
                vm.model.recipients = new Array();
            if (vm.model.recipients.length < vm.currentPlan.maxContactsPerInvitation) 
                vm.model.recipients.push({ contactId: item.id, name: item.name, email: item.email, phone: item.phone });
            else {
                ui.message({ type: 'danger', title: 'Error', message: 'Your current plan allow max of ' + vm.currentPlan.maxContactsPerInvitation + ' contact(s)' })
            }
            
        }
        vm.removeContact = function (item) {
            if (vm.model.recipients) {
                var selectedIndex = -1;
                for (var i = 0; i < vm.model.recipients.length; i++) {
                    if (vm.model.recipients[i].id == item.id) {
                        selectedIndex = i;
                        break;
                    }
                }
                if (selectedIndex > -1) {
                    vm.model.recipients.splice(selectedIndex, 1);
                }
            }
        }
        vm.showAddContact = function (item) {
            if (!vm.model.recipients)
                return true;
            var show = true;
            for (var i = 0; i < vm.model.recipients.length; i++) {
                if (vm.model.recipients[i].contactId == item.id) {
                    show = false;
                    break;
                }
            }
            return show;
        }
        var lastSearchModel = {};
        vm.onlineSearch = function (tableState) {

            var pagination = tableState.pagination;
            var itemsPerPage = pagination.number || 10;  // Number of entries showed per page.
            var pageNumber = pagination.start ? (pagination.start / itemsPerPage) + 1 : 1;
            var searchModel = { pageNumber: pageNumber, itemsPerPage: itemsPerPage };
            console.log('1');
            angular.extend(searchModel, vm.search);
            if ($rootScope.forceRefresh != 'invitations' && angular.equals(searchModel, lastSearchModel))
                return;
            $rootScope.forceRefresh = null;
            console.log('hi');
            lastSearchModel = angular.copy(searchModel);
            ui.overlay.show();
            invitations.searchOutgoing(searchModel).then(
                function (response) {
                    vm.displayedList = response.data.content;
                    tableState.pagination.numberOfPages = Math.ceil(response.data.totalCount / itemsPerPage);
                },
                function (err) { }
            ).finally(function () { ui.overlay.hide(); });
        }

        function refresh(id) {
            $rootScope.$broadcast('refreshtable',id);
        }
    }
})();