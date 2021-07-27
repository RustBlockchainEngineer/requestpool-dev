/*
*
*/

(function () {
    'use strict';
    angular.module('app.main').controller('main.itemsProperties.ctrl', ctrl);
     
    ctrl.$inject = ['$scope', '$state', 'core.system.svc', 'core.settings', '$uibModal', 'ui.svc', 'main.propertyTypes.svc',
        'main.itemsProperties.svc'];

    function ctrl($scope, $state,system, settings,$uibModal, ui,propertyTypes,itemsProperties) {
        ui.page.title = system.resources.views.item_properties;
        
        $scope.search = { isDeleted: false };

        $scope.initialModel = { name: '' ,propertyType:null};

        $scope.model = {};

        $scope.showSearch = false;
        $scope.toggleSearch = function () {
            $scope.showSearch = !$scope.showSearch;
        }

        ui.overlay.show();
        propertyTypes.all().then(
            function (response) {
                $scope.propertyTypes = response.data.content;
            },
            function () {
            }
        ).finally(function () {
            ui.overlay.hide();
        });
        refresh();
        function refresh() {
            ui.overlay.show();
            itemsProperties.search().then(
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