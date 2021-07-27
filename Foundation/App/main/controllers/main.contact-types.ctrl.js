
/*
*
*/

(function () {
    'use strict';
    angular.module('app.core').controller('main.contactTypes.ctrl', ctrl);
     
    ctrl.$inject = ['$scope', '$state', 'core.system.svc', 'core.settings', '$uibModal', 'ui.svc', 'main.contactTypes.svc'];

    function ctrl($scope, $state,system, settings,$uibModal, ui,contactTypes) {
        ui.page.title = system.resources.views.contact_categories;
        
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
            contactTypes.search().then(
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