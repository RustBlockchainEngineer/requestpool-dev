/*
*
*/

(function () {
    'use strict';
    angular.module('app.ui').factory('ui.svc', svc);

    svc.$inject = ['$uibModal','uiTinymceConfig'];

    function svc($uibModal, uiTinymceConfig) {
        uiTinymceConfig.baseURL = server + '/wwwroot/tinymce';
        var viewsPath = '/app/views/ui';
        var settings = {
            mainHeader: viewsPath + '/layout.main-header.html',
            contentHeader: viewsPath + '/layout.content-header.html',
            sideMenu: viewsPath + '/layout.side-menu.html',
            footer: viewsPath + '/layout.footer.html',
            alerts: viewsPath + '/layout.alerts.html',
            pageSize: 10,
            images: {
                person: '/wwwroot/_admin/images/person.jpg',
                file: '/wwwroot/_admin/images/file.png',
                photo: '/wwwroot/_admin/images/photo.png',
                icon: '/wwwroot/_admin/images/icon.png',
                doc: '/wwwroot/_public/images/doc.png',
                xls: '/wwwroot/_public/images/xls.png',
                pdf: '/wwwroot/_public/images/pdf.png'
            }

        };
        function layout() {
            var _layout = this;
            _layout.getContentHeaderTop = function () {
                if (_svc.page.showToolbar === undefined) {
                    _svc.page.showToolbar = true;
                }
                return (_svc.page.showToolbar === true) ?
                    ($('#main-header').outerHeight(true) + $('#toolbar').outerHeight(true)) : $('#main-header').outerHeight(true);

            }
            _layout.getContentTop = function () {
                if (_svc.page.showToolbar === undefined) {
                    _svc.page.showToolbar = true;
                }
                return (_svc.page.showToolbar === true) ?
                    ($('#content-header-wrapper').outerHeight(true) + $('#toolbar').outerHeight(true)) : $('#content-header-wrapper').outerHeight(true);
            }
            _layout.getContentSlimscrollOptions = function () {
                var tmp = {
                    noWatch: false,
                    height: Math.floor($(window).height()
                        - ($('#main-header').outerHeight(true) || 0)
                        - ($('#content-header').outerHeight(true) || 0)
                        - ($('#main-footer').outerHeight(true) || 0)
                        - ($('#toolbar').outerHeight(true) || 0)) + 'px'
                };
                return tmp;
            }
            _layout.getSidebarSlimscrollOptions = function () {
                return {
                    noWatch: false,
                    height: Math.floor($(window).height()
                        - ($('#main-header').outerHeight(true) || 0)) + 'px'
                };
            }
            _layout.getSideMenuHeight = function () {
                return Math.floor($(window).height() - ($('#main-header').outerHeight(true) || 0)) + 'px'
            }
            _layout.getWindowHeight = function () {
                return $(window).height() + 'px';
            }
            return _layout;
        };
        function alerts() {
            var _alerts = this;
            _alerts.items = []; //{ title:'',description:'',type:''}
            _alerts.add = function (title, description, type) {
                _alerts.items.unshift({ title: title, description: description, type: type });
            }
            _alerts.remove = function (index) {
                _alerts.items.splice(index, 1);
            }
            _alerts.clear = function (index) {
                _alerts.items = [];
            }
            _alerts.http = function (response) {
                if (!response)
                    return;
                if (response.data) {
                    if (response.data.message)
                        _alerts.items.unshift({ description: response.data.message, type: 'danger' });
                    //if (response.data.errors && response.data.error instanceof Array) {
                    //    var errors = '<ul>';
                    //    for (var i = 0; i < response.data.erors.length; i++) {
                    //        errors += '<li>' + response.data.erors[i] + '</li>'
                    //    }
                    //    errors += '</ul>';
                    //    _alerts.items.unshift({ description: errors, type: 'danger' });
                    //}
                }
            }
        }
        function overlay(){
            var _overlay = this;
            _overlay.blockUi = function () {
                var $body = $('body');
                if ($body.css('position') == 'static')
                    $body.css('position', 'relative');
                if ($body.data('overlayCount') == undefined)
                    $body.data('overlayCount', 0);
                $body.data('overlayCount', $body.data('overlayCount') + 1);
                if ($body.data('overlayCount') == 1)
                    $body.append("<div class='overlay'><i class='fa fa-refresh fa-spin'></i></div>");
            }
            _overlay.unblockUi = function () {
                var $body = $('body');
                if ($body.data('overlayCount') == undefined)
                    $body.data('overlayCount', 0);
                $body.data('overlayCount', $body.data('overlayCount') - 1);
                if ($body.data('overlayCount') <= 0)
                    $body.children('.overlay').remove();
            }

            _overlay.show = function (element) {
                if (element == undefined)
                    element = '#main-content';
                var $wrapper;
                if (element instanceof jQuery)
                    $wrapper = element.closest('[data-overlay-wrapper]');
                else
                    $wrapper = $(element).closest('[data-overlay-wrapper]');
                if ($wrapper.length == 0) {
                    _overlay.blockUi();
                    return;
                }
                if ($wrapper.css('position') == 'static')
                    $wrapper.css('position', 'relative');
                if ($wrapper.data('overlayCount') == undefined)
                    $wrapper.data('overlayCount', 0);
                $wrapper.data('overlayCount', $wrapper.data('overlayCount') + 1);
                if ($wrapper.data('overlayCount') == 1)
                    $wrapper.append("<div class='overlay'><i class='fa fa-refresh fa-spin'></i></div>");
            }
            _overlay.hide = function (element) {
                if (element == undefined)
                    element = '#main-content';
                var $wrapper;
                if (element instanceof jQuery)
                    $wrapper = element.closest('[data-overlay-wrapper]');
                else
                    $wrapper = $(element).closest('[data-overlay-wrapper]');
                if ($wrapper.length == 0) {
                    _overlay.unblockUi();
                    return;
                }
                if ($wrapper.data('overlayCount') == undefined)
                    $wrapper.data('overlayCount', 0);
                $wrapper.data('overlayCount', $wrapper.data('overlayCount') - 1);
                if ($wrapper.data('overlayCount') <= 0)
                    $wrapper.children('.overlay').remove();
            }

        }

        function message(options) {
            if (!options) {
                options = { callback: function () { } };
            }
            else if (!options.callback) {
                options.callback = function () { };
            }
            
            $uibModal.open({
                animation: options.animation || false,
                backdrop: options.backdrop || true,
                templateUrl: 'app/views/ui/modal.message.html',
                size: options.size || 'sm',
                controller: ['options', '$scope', function (options, $scope) {
                    $scope.options = options;
                }],
                resolve: {
                    options: function () {
                        return options;
                    }
                }
            });
        }

        function confirm(options) {
            if (!options) {
                options = { callback: function () { }};
            }
            else if (!options.callback) {
                options.callback = function () { };
            }
            $uibModal.open({
                animation: options.animation || false,
                backdrop: options.backdrop || true,
                templateUrl: 'app/views/ui/modal.confirm.html',
                size: options.size || 'sm',
                controller: ['options', '$scope', function (options, $scope) {
                    $scope.options = options;
                }],
                resolve: {
                    options: function () {
                        return options;
                    }
                }
            });
        }
        function confirmDelete(callback,params) {
            confirm({ callback: callback,params:params,message:'هل انت متأكد من الحذف؟',type:'danger' });
        }
        var _svc = {
            settings: settings,
            page: { title: '', description: '', showToolbar: true },
            layout: new layout(),
            alerts: new alerts(),
            overlay: new $.overlay({ defaultElement:'#main-content'}),
            message: message,
            confirm: confirm,
            confirmDelete: confirmDelete,
            tinymceOptions: {
                min: {
                    relative_urls: false,
                    remove_script_host: false,                    
                    height: 250,
                    menubar: false,
                    plugins: [
                        'lists link charmap print preview anchor',
                        'searchreplace visualblocks fullscreen',
                        'table contextmenu paste code'
                    ],
                    toolbar: 'undo redo | styleselect | bold italic | alignleft aligncenter alignright alignjustify | bullist numlist outdent indent'
                }
            }
        }
        return _svc;


    }

})();