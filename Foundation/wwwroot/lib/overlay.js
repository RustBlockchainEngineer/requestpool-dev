(function ($) {
    $.overlay = function (options) {

        var defaults = {
            defaultElement : 'body'
        },
        plugin = this,
        options = $.extend({}, defaults, options);


        plugin.blockUi = function () {
            var $body = $('body');
            if ($body.css('position') == 'static')
                $body.css('position', 'relative');
            if ($body.data('overlayCount') == undefined)
                $body.data('overlayCount', 0);
            $body.data('overlayCount', $body.data('overlayCount') + 1);
            if ($body.data('overlayCount') == 1)
                $body.append("<div class='overlay'><i class='fa fa-refresh fa-spin'></i></div>");
        }
        plugin.unblockUi = function () {
            var $body = $('body');
            if ($body.data('overlayCount') == undefined)
                $body.data('overlayCount', 1);
            $body.data('overlayCount', $body.data('overlayCount') - 1);
            if ($body.data('overlayCount') <= 0)
                $body.children('.overlay').remove();
        }

        plugin.show = function (element) {
            if (element == undefined)
                element = options.defaultElement;
            var $wrapper;
            if (element instanceof jQuery)
                $wrapper = element.closest('[data-overlay-wrapper]');
            else
                $wrapper = $(element).closest('[data-overlay-wrapper]');
            if ($wrapper.length == 0) {
                plugin.blockUi();
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
        plugin.hide = function (element) {
            if (element == undefined)
                element = options.defaultElement;
            var $wrapper;
            if (element instanceof jQuery)
                $wrapper = element.closest('[data-overlay-wrapper]');
            else
                $wrapper = $(element).closest('[data-overlay-wrapper]');
            if ($wrapper.length == 0) {
                plugin.unblockUi();
                return;
            }
            if ($wrapper.data('overlayCount') == undefined)
                $wrapper.data('overlayCount', 1);
            $wrapper.data('overlayCount', $wrapper.data('overlayCount') - 1);
            if ($wrapper.data('overlayCount') <= 0)
                $wrapper.children('.overlay').remove();
        }


    }
}(jQuery));