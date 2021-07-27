/*
*
*/

(function () {
    'use strict';
    angular.module('app.core').directive('fileBind', dir);

    dir.$inject = [];

    function dir() {
        return {
            scope: {
                fileBind: "="                
            },
            link: function (scope, element, attributes) {
                element.bind("change", function (changeEvent) {
                    var reader = new FileReader();
                    var file = changeEvent.target.files[0];

                    if (file instanceof Blob && checkType(file) && checkSize(file)) {
                        reader.onload = function (evt) {
                            scope.$apply(function () {
                                scope.fileBind.content = evt.target.result.substring(evt.target.result.indexOf(',') + 1);
                                scope.fileBind.originalFileName = file.name;
                                scope.fileBind.size = file.size;
                                scope.fileBind.type = file.type;
                                scope.fileBind.header = evt.target.result.substring(0, evt.target.result.indexOf(','));
                            });
                        }
                        reader.readAsDataURL(file);
                    } else {
                        scope.$apply(function () {
                            scope.fileBind.content = null;
                        });
                        element.val(null);
                    }
                    /*
                    scope.$apply(function () {
                        scope.fileBind = changeEvent.target.files[0];
                        // or all selected files:
                        // scope.fileread = changeEvent.target.files;
                    });
                    */
                });
                function checkType(file) {
                    var type = '.'+ file.name.split('.').pop().toLowerCase();
                    var result = false;
                    if (attributes['accept']) {
                        var types = attributes['accept'].split(',');
                        for (var i = 0; i < types.length; i++) {
                            if (types[i].trim().toLowerCase() == type.trim().toLowerCase()) {
                                result = true;
                                break;
                            }
                        }
                    }
                    else {
                        result = true;

                    }


                    return result;
                }
                function checkSize(file) {
                    var result = true;
                    if (attributes['maxSize']) {
                        try {
                            console.log(file.size);
                            var maxSize = parseInt(attributes['maxSize']);
                            console.log(maxSize);

                            result = (file.size / 1024) <= maxSize;
                        } catch (exp) { }
                    }
                    return result;
                }
                scope.$watch('fileBind', function (model) {
                    if (!model.content) {
                        element.val(null);
                    }
                });
            }
        }
    }

})();
