/*
*
*/

(function () {
    'use strict';
    angular.module('app.core').factory('core.authenticationInterceptor', authenticationInterceptor);
    
    authenticationInterceptor.$inject = ['$injector','$q', 'core.settings'];

    function authenticationInterceptor($injector,$q, settings) {

        return {
            request: request,
            requestError: requestError,
            response: response,
            responseError:responseError
        }
        function prepareError(rejection) {
            //console.log(rejection);
            if (rejection.data == undefined)
                rejection.data = {};
            if (rejection.data.message == undefined) {
                if (rejection.status == -1)
                    rejection.data.message = 'Please check you are connected to Internet';
                else
                    rejection.data.message = 'Unknown Error';
            }
            return rejection;
        }
        function request(config) {
            var account = $injector.get('core.account.svc');
            if (account.accessToken !== undefined && account.accessToken !== null) {
                config.headers['Authorization'] = 'Bearer ' + account.accessToken;
            }
            if (settings.culture) {
                config.headers['X-Culture'] = settings.culture;
            }
            if (settings.loginHeader) {
                config.headers[settings.loginHeader] = 'true';
            }
            return config;
        }

        function requestError(rejection) {
            prepareError(rejection);
            return $q.reject(rejection);
        }

        function response(response) {
            return response;
        }

        function responseError(rejection) {
            prepareError(rejection);

            var $http = $injector.get('$http');
            var account = $injector.get('core.account.svc');

            if (rejection.status == 401) {
                if (account.refreshToken !== undefined && account.refreshToken !== null) {
                    return account.refreshLogin().then(
                        function (result) {
                            if(result.OK)
                                return $http(rejection.config);//retry original request
                            else
                                account.logout();
                        },
                        function (err) {
                            account.logout();
                        });
                }
                else {
                    account.logout();
                }
            }
            return $q.reject(rejection);
        }
    }

})();
