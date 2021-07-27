/*
*
*/

(function () {
    'use strict';
    angular.module('app.core').controller('main.enquiryTypes.ctrl', ctrl);
     
    ctrl.$inject = ['$scope', '$state', 'core.system.svc', 'core.settings', '$uibModal', 'ui.svc', 'main.enquiryTypes.svc'];

    function ctrl($scope, $state,system, settings,$uibModal, ui,enquiryTypes) {
        ui.page.title = system.resources.views.enquiry_types;
        
        $scope.search = { isDeleted: false };

        $scope.initialModel = { name: '' };

        $scope.model = {};

        $scope.showSearch = false;
        $scope.toggleSearch = function () {
            $scope.showSearch = !$scope.showSearch;
        }

        refresh();
        function refresh() {
            ui.overlay.show();
            enquiryTypes.search().then(
                function (response) {
                    $scope.list = response.data.content;
                },
                function () {
                }
            ).finally(function () {
                ui.overlay.hide();
            });
        }

        

    
    }
})();