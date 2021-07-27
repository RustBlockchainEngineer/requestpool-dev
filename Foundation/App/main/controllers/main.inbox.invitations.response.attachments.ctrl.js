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
        if ($state.params.item && $state.params.item.id) {
            vm.recipientId = $state.params.item.id;
            refresh();

        } else if ($state.params.id) {
            ui.overlay.show();
            invitations.incomingById($state.params.id).then(
                function (response) {
                    $state.params.item = response.data.content;
                    vm.recipientId = $state.params.item.id;
                    refresh();
                },
                function () {
                }
            ).finally(function () {
                ui.overlay.hide();
            });
        }
       
        function refresh() {
            ui.overlay.show();
            responseAttachments.get(vm.recipientId).then(
                function (response) {
                    vm.list = response.data.content;
                },
                function () {
                }
            ).finally(function () {
                ui.overlay.hide();
            });
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