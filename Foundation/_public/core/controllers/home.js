/*
*
*/

(function () {
    'use strict';
    angular.module('app.core').controller('core.homeCtrl', Ctrl);
     
    Ctrl.$inject = ['$scope', '$state', 'core.settings', 'core.system.svc','ui.svc'];
    function Ctrl($scope, $state, settings,system,ui) {
        ui.page.title = system.resources.views.home;

//        ui.page = { title: 'Home', description: 'description', showToolbar: false };
        //ui.alerts.add('hi there', 'hi description', 'danger');
        //ui.alerts.add('warning', 'warning description', 'warning');

        $scope.showOverlay = function ($event) {
            ui.overlay.show('.alerts');
        }
        $scope.hideOverlay = function ($event) {
            ui.overlay.hide('.alerts');
        }

        //ui.message({type:'success'});
        //ui.confirmDelete(function () { alert('done')}
        //);

        $scope.remove = function (item) {
            alert(item);
        }
    }
})();