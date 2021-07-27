/*
*
*/

(function () {
    'use strict';
    angular.module('app.core').factory('core.account.svc', svc);

    svc.$inject = ['$q', '$httpParamSerializer', '$http', '$window', 'core.settings', 'core.system.svc'];

    function svc($q, $httpParamSerializer, $http, $window, settings, system) {
        var _data = {
            authenticated: false,
            username: null,
            accessToken: null,
            refreshToken: null,
            roles: []
        };
        var _account = {
            authenticated: _data.authenticated,
            username: _data.username,
            accessToken: _data.accessToken,
            refreshToken: _data.refreshToken,
            info: _data.info,
            roles: _data.roles,
            //functions
            login: login,
            register: register,
            refreshLogin: refreshLogin,
            requestOtp: requestOtp,
            logout: logout,
            getProfile:getProfile,
            saveProfile: saveProfile,
            removePhoto:removePhoto,
            updatePassword: updatePassword,
            loadStorage: loadStorage,
            updateStorage: updateStorage,
            updateInfo: updateInfo,
            updateLocation: updateLocation,
            has: has
        };

        return _account;

        function updateInfo(info) {
            angular.extend(_data.info, info);
            _account.info = _data.info;
            updateStorage();
        }
        function loadStorage() {
            if (localStorage) {
                angular.copy({}, _data);
                try {
                    _data = angular.copy(angular.fromJson(localStorage.getItem(settings.dataKey)), _data);
                    angular.extend(_account, _data);
                }
                catch (exp) {
                }
            }
        }

        function updateStorage() {
            if (localStorage) {
                localStorage.setItem(settings.dataKey, angular.toJson(_data));
            }
        }

        function setUserData(response) {
            _data.authenticated = true;
            _data.username = response.data.userName;
            _data.accessToken = response.data.access_token;
            _data.refreshToken = response.data.refresh_token;
            _data.info = JSON.parse(response.data.info);
            _data.roles = JSON.parse(response.data.roles);
            for (var i = 0; i < _data.roles.length; i++) {
                _data.roles[i] = _data.roles[i].replace(/[ \t\n]+/g, '').toLowerCase();
            }
            angular.extend(_account, _data);
        }
        function login(model) {
            angular.extend(model, { grant_type: 'password' })
            return $http.post(settings.server + '/Token', $httpParamSerializer(model), {
                headers: { 'Content-Type': 'application/x-www-form-urlencoded' }
            })
			.then(
				function (response) {
				    setUserData(response);
				    updateStorage();
				    return response;
				},
				function (err) {
				    return err;
				}
			).then(function (result) {
			    console.log(result);
			    if (!result.status || result.status >= 300)
			        return $q(function (resolve, reject) {
			            reject(result);
			        });
			    else
			        return $q(function (resolve, reject) {
			            resolve(result);
			        });

			});
        }

        function refreshLogin() {
            var params = { grant_type: 'refresh_token', refresh_token: obj.data.refreshToken };
            return $http.post(settings.server + '/Token', $httpParamSerializer(params), {
                headers: { 'Content-Type': 'application/x-www-form-urlencoded' }
            })
			.then(
				function (response) {
				    setUserData(response);
				    updateStorage();
				    return $q(function (resolve, reject) {
				        resolve(response);
				    });
				},
				function (err) {
				    return $q(function (resolve, reject) {
				        reject(err);
				    });
				}
			);
        }
        

        function has(permissions,strict) {
            if (!permissions || permissions.length == 0)
                return true;
            var result = false;
            var permissionsArr = permissions.replace(/[ \t\n]+/g, '').toLowerCase().split(',');
            if (permissionsArr.length == 1 && permissionsArr[0] == 'authenticated') {
                result = _data && _data.authenticated;
            }
            else if (_data && _data.authenticated && _data.roles) {

                if (!strict && (_data.roles.indexOf('system') > -1 || _data.roles.indexOf('super') > -1)) {
                    result = true;
                }
                else {
                    result = true;
                    for (var i = 0; i < permissionsArr.length; i++) {

                        if (_data.roles.indexOf(permissionsArr[i]) == -1) {
                            result = false;
                            break;
                        }
                    }
                }
            }
            return result;
        }
        function requestOtp(username,purpose) {
            return $http.get(settings.apiUrl + '/account/otp/' + purpose +'/?username=' + username);
        }
        function logout() {
            angular.copy({
                authenticated: false,
                username: '',
                accessToken: '',
                refreshToken: '',
                roles: []
            }, _data);
            angular.extend(_account,_data);
            localStorage.removeItem(settings.dataKey);
            return $q(function (resolve, reject) {
                resolve({ OK: true });
            });
        }

        function getProfile() {
            return $http.get(settings.apiUrl + '/account/profile');
        }

        function saveProfile(model) {
            return $http.put(settings.apiUrl + '/account/profile',model);
        }
        function updatePassword(model) {
            return $http.put(settings.apiUrl + '/account/password', model);
        }
        function register(model) {
            return $http.post(settings.apiUrl + '/account/register', model);
        }

        function removePhoto() {
            return $http.delete(settings.apiUrl + '/account/photo');
        }

        function updateLocation(model) {
            return $http.put(settings.apiUrl + '/account/location', model);
        }
    }



})();