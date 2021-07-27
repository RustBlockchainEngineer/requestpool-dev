/*
*
*/

(function () {
    'use strict';
    angular.module('app.core').directive('permissions', dir);

    dir.$inject = ['core.account.svc'];

    function dir(account) {
        return {
            link: function (scope, element, attrs) {
                function toggleVisibility() {
                    if (account.has(attrs.permissions,angular.isDefined(attrs.strict))) {
                        element.show();
                    }
                    else
                        element.hide();
                }
                toggleVisibility();
                scope.$on('authentication', toggleVisibility);
            }
        };
    }

})();
