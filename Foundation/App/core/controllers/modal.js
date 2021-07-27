/*
*
*/

(function () {
    'use strict';
    angular.module('app.core').controller('core.modalCtrl', Ctrl);
     
    Ctrl.$inject = ['$scope', '$state', 'core.settings', 'options', 'core.modal'];
    function Ctrl($scope, $state, settings, options, modal) {
        $scope.modalOptions = options;
        $scope.close = function () {
            modal.close();
        }
    }
})();