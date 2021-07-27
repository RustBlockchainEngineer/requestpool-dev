/*
*
*/

(function () {
    'use strict';
    angular.module('app.core').factory('core.modal', svc);

    svc.$inject = ['$uibModal'];

    function svc($uibModal) {
        var modal;

        return {
            show: show,
            close: close
        };

        function show(options) {
            close();

            modal = $uibModal.open({
                animation: false,
                backdrop: true,
                templateUrl: 'app/views/core/modal.alert.html',
                controller: 'core.modalCtrl',
                size: 'sm',
                resolve: {
                    options: options
                }
            });
        }
        function close() {
            try {
                modal.close();
                modal = null;
            } catch (exp) { }
        }
    }

})();