/*
*
*/

(function () {
    'use strict';
    angular.module('app.main').controller('main.inbox.invitations.response.attachmnets.ctrl', ctrl);

    ctrl.$inject = ['$scope', '$state', 'core.settings', 'core.system.svc', 'ui.svc', 'main.responseAttachments.svc', 'main.invitations.svc'];

    function ctrl($scope, $state, settings, system, ui, responseAttachments, invitations) {
        //ui.page.title = system.resources.views.attachments;

        var vm = this;
        vm.isSaveEnabled = false;
        vm.editMode = false;
        vm.search = { isDeleted: false }
        vm.initialModel = { invitationId: null, title: '', description: '' };

        vm.model = { invitationId: null, title: '', description: null };
        if ($state.params.item && $state.params.item.id) {
            vm.model.invitationId = $state.params.item.id;
            vm.initialModel.invitationId = $state.params.item.id;
            vm.isSaveEnabled = $state.params.item.isSaveEnabled;
            refresh();

        } else if ($state.params.id) {
            ui.overlay.show();
            invitations.incomingById($state.params.id).then(
                function (response) {
                    $state.params.item = response.data.content;
                    vm.model.invitationId = $state.params.item.id;
                    vm.initialModel.invitationId = $state.params.item.id;
                    vm.isSaveEnabled = $state.params.item.isSaveEnabled;
                    refresh();

                },
                function () {
                }
            ).finally(function () {
                ui.overlay.hide();
            });
        }



        vm.showSearch = false;
        vm.toggleSearch = function () {
            vm.showSearch = !vm.showSearch;
        }

        function refresh() {
            ui.overlay.show();
            responseAttachments.get(vm.model.invitationId).then(
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
            vm.model.invitationId = vm.initialModel.invitationId;
        }

        vm.remove = function (item) {
            ui.overlay.show();
            responseAttachments.remove(vm.model.invitationId,item.id).then(
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
                function () {
                    ui.alerts.add('Error', result.error, 'danger');

                })
                .finally(function () {
                    reset();
                    ui.overlay.hide();
                });
        }

        vm.save = function () {
            ui.overlay.show();

            if (vm.model.id) {
                responseAttachments.update(vm.model.invitationId,vm.model.id, vm.model).then(
                    function (response) {
                        reset();
                    },
                    function (err) {
                    }
                ).finally(function () {
                    reset();
                    ui.overlay.hide();
                    refresh();
                });

            }
            else {
                responseAttachments.create(vm.model.invitationId,vm.model).then(
                    function (response) {
                        reset();
                    },
                    function (err) {
                    }
                ).finally(function () {
                    reset();
                    refresh();
                    ui.overlay.hide();
                });
            }
        }


        vm.cancel = function (form) {
            reset(form);
        }

        vm.getFileIcon = function (item) {
            var ext = item.filename.split('.').pop().toLowerCase();
            var src = '';
            switch (ext) {
                case 'jpeg':
                case 'jpg':
                case 'png':
                case 'gif':
                case 'bmp': {
                    src = item.url;
                } break;
                case 'xls':
                case 'xlsx':
                    {
                    src = ui.settings.images.xls;
                } break;
                case 'doc':
                case 'docx':{
                    src = ui.settings.images.doc;
                } break;
                case 'pdf': {
                    src = ui.settings.images.pdf;
                } break;
                case 'zip': {
                    src = ui.settings.images.zip;
                } break;

                default: {
                    src = ui.settings.images.file;
                }
            }
            return src;
        }

        function reset(form) {
            vm.model = angular.copy(vm.initialModel);
            if (vm.form) {
                vm.form.$setPristine();
            }
            vm.editMode = false;
        }
    }
})();