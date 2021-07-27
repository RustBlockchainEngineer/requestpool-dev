/*
*
*/

(function () {
    'use strict';
    angular.module('app.core').controller('core.profile.ctrl', ctrl);

    ctrl.$inject = ['$filter', '$scope', '$state', 'core.settings', 'core.system.svc', 'ui.svc', 'core.account.svc'];
    function ctrl($filter, $scope, $state, settings, system, ui, account) {
        ui.page.title = system.resources.views.profile;

        var vm = this;
        vm.initialModel = {};
        vm.viewMode = 'view';
        vm.model = { };
        $scope.photoFile = {};
        
        
        ui.overlay.show();
        account.getProfile().then(
            function (response) {
                vm.initialModel = response.data.content;
                vm.model = angular.copy(vm.initialModel);
                updatePhotoStyle();
            },
            function () {
            }
        ).finally(function () {
            ui.overlay.hide();
        });

        function updatePhotoStyle(){
            $scope.photoStyle = vm.model.photoUrl ? { 'background-image': 'url(' + vm.model.photoUrl + ')' } : { 'background-image': 'url(' + ui.settings.images.person + ')' };
        }

        function getUpdateModel() {
            return {
                name: vm.model.name,
                phoneNumber: vm.model.phoneNumber,
                profile: vm.model.profile,
                companyName: vm.model.companyName,
                photoFile: $scope.photoFile.content ? $scope.photoFile : null
            }
        }
        vm.save = function () {
            ui.overlay.show();
            account.saveProfile(getUpdateModel()).then(
                function (response) {
                    vm.initialModel = response.data.content;
                    vm.model = angular.copy(vm.initialModel);
                    account.updateInfo(vm.model);
                    vm.viewMode = 'view';
            },
            function (err) {
                ui.alerts.http(err);
            }
            ).finally(function () {
                updatePhotoStyle();
                ui.overlay.hide();
            });

            
        }
        vm.updatePassword = function () {
            ui.overlay.show();
            account.updatePassword(vm.passwordModel).then(
                function (response) {
                    vm.passwordModel = {};
                    vm.viewMode = 'view';
                },
                function (err) {
                    ui.alerts.http(err);
                }
            ).finally(function () {
                ui.overlay.hide();
            });


        }
        vm.requestOtp = function () {
            ui.overlay.show();
            account.requestOtp(vm.model.userName,'login').then(
                function (result) {
                    ui.alerts.add('Success', 'Check Your Email', 'info');
                },
                function (err) {
                    console.log(err);
                    ui.alerts.add('Error', err.error, 'danger');
                })
                .finally(function () {
                    ui.overlay.hide();
                });
        }
        vm.cancel = function () {
            vm.model = angular.copy(vm.initialModel);
            vm.passwordModel = {};
            $scope.photoFile = {};

            vm.viewMode = 'view';
        }   
        vm.removePhoto = function () {
            ui.overlay.show();
            account.removePhoto().then(
                function (response) {
                    vm.initialModel.photo = vm.initialModel.photoUrl = vm.model.photo = vm.model.photoUrl = null;
                    account.updateInfo({ photo: null,photoUrl:null});
                    $scope.photoFile = {};
                },
                function (err) {
                    ui.alerts.http(err);
                }
            ).finally(function () {
                updatePhotoStyle();
                ui.overlay.hide();
            });
        }
        vm.resetPhoto = function () {
            $scope.photoFile = {};
            updatePhotoStyle();
        }

        $scope.$watch('photoFile.content', function () {
            if ($scope.photoFile.content)
                vm.model.photoUrl = $scope.photoFile.header + ',' + $scope.photoFile.content;
            else if ($state.params.item)
                vm.model.photoUrl = $state.params.item.photoUrl;
            else
                vm.model.photoUrl = null;
        });
    }
})();