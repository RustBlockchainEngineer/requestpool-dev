/*
*
*/

(function () {
    'use strict';
    angular.module('app.core').factory('core.resources.svc', svc);

    svc.$inject = ['$rootScope','$q', '$httpParamSerializer', '$http', '$window', 'core.settings', 'core.system.svc'];

    function svc($rootScope,$q, $httpParamSerializer, $http, $window, settings, system) {
//        var translations = {}
        var _svc = {
            load: load
        };

        return _svc;

        function load() {
            system.resources = $rootScope.resources = resources;
            for (var key in resources.validation.common) {
                if (resources.validation.common.hasOwnProperty(key)) {
                    resources.validation.common[key] = new RegExp(resources.validation.common[key]);
                }
            }
            for (var key in resources.validation.model) {
                if (resources.validation.model.hasOwnProperty(key)) {
                    resources.validation.model[key] = new RegExp(resources.validation.model[key]);
                }
            }
   //         return $http.get(settings.apiUrl + '/translations', {})
			//.then(
			//	function (response) {
			//	    system.translation = response.data;
			//	    return $q(function (resolve, reject) {
			//	        resolve(response);
			//	    });
			//	},
			//	function (err) {
			//	    return $q(function (resolve, reject) {
			//	        reject(err);
			//	    });
			//	}
			//);
        }
    }
})();