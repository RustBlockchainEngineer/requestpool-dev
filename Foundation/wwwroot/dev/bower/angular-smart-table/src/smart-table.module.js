ng.module('smart-table', []).run(['$templateCache', function ($templateCache) {
    $templateCache.put('template/smart-table/pagination.html',
        '<nav ng-if="numPages && pages.length >= 2"><ul class="pagination">' +
        '<li ng-class="{disabled: currentPage==1}"><a href="javascript: void(0);" ng-click="selectPage(currentPage-1)"><span class="glyphicon glyphicon-chevron-left"></span></a></li>' +
        '<li ng-repeat="page in pages" ng-class="{active: page==currentPage}"><a href="javascript: void(0);" ng-click="selectPage(page)">{{page}}</a></li>' +
        '<li ng-class="{disabled: currentPage==numPages}"><a href="javascript: void(0);" ng-click="selectPage(currentPage+1)"><span class="glyphicon glyphicon-chevron-right"></span></a></li>' +
        '</ul></nav>');
}]);

