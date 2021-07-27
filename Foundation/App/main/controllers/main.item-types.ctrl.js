/*
*
*/

(function () {
    'use strict';
    angular.module('app.core').controller('main.itemTypes.ctrl', ctrl);
     
    ctrl.$inject = ['$scope', '$state', 'core.system.svc', 'core.settings', '$uibModal', 'ui.svc', 'main.itemTypes.svc'];

    function ctrl($scope, $state,system, settings,$uibModal, ui,itemTypes) {
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
            itemTypes.search().then(
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