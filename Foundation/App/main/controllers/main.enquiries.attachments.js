/*
*
*/

(function () {
    'use strict';
    angular.module('app.main').controller('main.enquiries.attachmnets.ctrl', ctrl);

    ctrl.$inject = ['$scope', '$state', 'core.settings', 'core.system.svc', 'ui.svc', 'main.enquiryAttachments.svc', 'main.enquiries.svc'];

    function ctrl($scope, $state, settings, system, ui, enquiryAttachments, enquiries) {
        //ui.page.title = system.resources.views.attachments;

        var vm = this;
        vm.search = { isDeleted: false }
        vm.initialModel = { enquiryId: null, title: '', description: '' };

        vm.model = { enquiryId: null, title: '', description: null };
        if ($state.params.item && $state.params.item.id) {
            vm.model.enquiryId = $state.params.item.id;
            vm.initialModel.enquiryId = $state.params.item.id;
            refresh();

        } else if ($state.params.id) {
            ui.overlay.show();
            enquiries.get($state.params.id).then(
                function (response) {
                    $state.params.item = response.data.content;
                    vm.model.enquiryId = $state.params.item.id;
                    vm.initialModel.enquiryId = $state.params.item.id;
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
            enquiryAttachments.get(vm.model.enquiryId).then(
                function (response) {
                    vm.list = response.data.content;
                },
                function () {
                }
            ).finally(function () {
                ui.overlay.hide();
            });
        }

        
        vm.cancel = function () {

            reset();
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


        function reset() {
            vm.model = angular.copy(vm.initialModel);
            vm.form.$setPristine();
            vm.editMode = false;
        }
    }
})();