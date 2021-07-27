(function () {
    'use strict';
    angular.module('app.core').directive('stRefresh', dir);

    dir.$inject = ['$rootScope'];

    function dir($rootScope) {
        return {
            require: 'stTable',
            restrict: "A",
            link: function (scope, elem, attr, table) {
                

                $rootScope.$on("refreshtable", function (event, id) {
                    if (attr['id'] == id) {
                        $rootScope.forceRefresh = id;
                        table.pipe(table.tableState());
                    }
                });
            }
        }
    }

})();
