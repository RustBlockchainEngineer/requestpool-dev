/*
*
*/

(function () {
    'use strict';
    angular.module('app.core').directive('loggedin', dir);

    dir.$inject = ['core.account.svc'];

    function dir(account) {
        return {
            link: function (scope, element, attrs) {
                function toggleVisibility() {
                    if (account.authenticated) {
                        element.show();
                    }
                    else {
                        element.hide();
                    }
                }
                toggleVisibility();
                scope.$on('authentication', toggleVisibility);
            }
        };
    }

})();
