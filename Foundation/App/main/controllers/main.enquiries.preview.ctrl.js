/*
*
*/

(function () {
    'use strict';
    angular.module('app.main').controller('main.enquiries.preview.ctrl', ctrl);

    ctrl.$inject = ['$compile','$timeout', 'hotRegisterer', '$filter', '$scope', '$state', 'core.system.svc', 'core.settings', '$uibModal',
        'ui.svc', 'main.enquiries.svc'];

    function ctrl($compile,$timeout, hotRegisterer, $filter, $scope, $state, system, settings, $uibModal, ui, enquiries) {
        ui.page.title = system.resources.views.preview;

        var vm = this;
        vm.showSheet = true;
        vm.isDraftResponse = true;
        vm.isSaveEnabled = true;
        vm.propertiesIndex = [];
        vm.properties = [];
        vm.items = [];
        vm.responses = [];
        vm.sheetValues = new Array();
        vm.sheetMeta = new Array();

        vm.settings = {
            rowHeaders: true,
            columnHeaderHeight: 35,
            stretchH: 'last',
            colWidths: 150,
            wordWrap: true,
            autoWrapRow: true,
            manualColumnResize: true,
            colHeaders: function (index) {
                if (index < 0 || !vm.properties || vm.properties.length == 0)
                    return null;
                return vm.properties[index].name;
            },
            cells: function (row, col, prop) {
                //console.log('[' + row + '][' + col + ']');
                //console.log(vm.sheetMeta[row]);

                if (row < 0 || col < 0 || !vm.properties || vm.properties.length == 0
                    || !vm.sheetMeta[row] || !vm.sheetMeta[row].properties[col])
                    return undefined;

                var cellProperties = {
                    uiRegex: vm.sheetMeta[row].properties[col].propertyType.uiRegex,
                    isApplicable: vm.sheetMeta[row].properties[col].isApplicable,
                    isReadOnly: vm.sheetMeta[row].properties[col].isReadOnly,
                    isRequired: vm.sheetMeta[row].properties[col].isRequired,
                    type: vm.sheetMeta[row].properties[col].propertyType.name,
                    readOnly: vm.properties[col].isInfoOnly || vm.sheetMeta[row].properties[col].isReadOnly
                    || !vm.sheetMeta[row].properties[col].isApplicable,
                    allowEmpty: ! vm.sheetMeta[row].properties[col].isRequired,
                    renderer: getRendrer(vm.sheetMeta[row].properties[col].propertyType.name),
                    editor: getEditor(vm.sheetMeta[row].properties[col].propertyType.name),
                    validator: getValidator(row, col, vm.sheetMeta[row].properties[col].propertyType.name)
                };

                return cellProperties;
            },
            data: vm.sheetValues
        };
        function getRendrer(type) {
            var rendrer = 'enhancedTextRenderer';
            switch (type) {
                case 'text': {
                    rendrer = 'enhancedTextRenderer';
                } break;
                case 'numeric': {
                    rendrer = 'enhancedNumericRenderer';
                } break;
                default: { };
            }
            return rendrer;
        }
        function getEditor(type) {
            var editor = 'enhancedTextEditor';
            switch (type) {
                case 'text': {
                    editor = 'enhancedTextEditor';
                } break;
                case 'numeric': {
                    editor = 'enhancedNumericEditor';
                } break;
                default: { };
            }
            return editor;
        }
        function getValidator(row, col, type) {
            var validator = null;
            switch (type) {
                case 'text': {
                    validator = null;
                } break;
                case 'numeric': {
                    if (typeof vm.sheetValues[row][col] == 'string' && vm.sheetValues[row][col].startsWith('=')) {
                        validator = function (value, callback) {
                            callback(true);
                        }
                    }
                    else
                        validator = 'numeric';
                } break;
                default: { };
            }
            return validator;
        }
        vm.model = {}
        vm.enquiryId = null;
        if ($state.params.item && $state.params.item.id) {
            vm.enquiryId = $state.params.item.id;
            loadProperties();

        } else if ($state.params.id) {
            ui.overlay.show();
            enquiries.get($state.params.id).then(
                function (response) {
                    $state.params.item = response.data.content;
                    vm.enquiryId = $state.params.item.id;
                    loadProperties();
                },
                function () {
                }
            ).finally(function () {
                ui.overlay.hide();
            });
        }

        function loadProperties() {
            ui.overlay.show();
            enquiries.previewProperties(vm.enquiryId).then(
                function (response) {
                    vm.properties = response.data.content;
                    vm.properties.sort(function (a, b) {
                        if (a.rank > b.rank) return -1;
                        else if (a.rank < b.rank) return 1;
                        else return 0;
                    });
                    updatePropertiesIndex();
                    refresh();
                },
                function (err) {
                    ui.alerts.http(err);
                }
            ).finally(function () {
                ui.overlay.hide();
            });
        }
        function updatePropertiesIndex() {
            vm.includedPropertiesIndex = [];
            for (var i = 0; i < vm.properties.length; i++) {
                vm.propertiesIndex[vm.properties[i].name.toLowerCase().replace(' ', '')] = i;
            }
        }
        function refresh() {
            ui.overlay.show();
            enquiries.preview(vm.enquiryId).then(
                function (response) {
                    vm.sheetMeta = [];
                    vm.items = response.data.content.items;
                    for (var itemIndex = 0; itemIndex < vm.items.length; itemIndex++) {
                        if (vm.items[itemIndex].properties && vm.items[itemIndex].properties.length > 0) {
                            for (var propertyIndex = 0; propertyIndex < vm.items[itemIndex].properties.length; propertyIndex++) {
                                var value = vm.items[itemIndex].properties[propertyIndex].value;
                                if (typeof value == 'string' && value.startsWith('=')) {
                                    var fieldsArr = value.match(/{[^}]+}/g);
                                    if (fieldsArr && fieldsArr.length > 0) {
                                        for (var i = 0; i < fieldsArr.length; i++) {
                                            for (var j = 0; j < vm.properties.length; j++) {
                                                if (vm.properties[j].propertyId ==
                                                    fieldsArr[i].trim().substring(1, fieldsArr[i].length - 1)) {
                                                    vm.items[itemIndex].properties[propertyIndex].value =
                                                        vm.items[itemIndex].properties[propertyIndex].value.replace(fieldsArr[i], '{' + vm.properties[j].name + '}');
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    vm.responses = [];

                    if (vm.items.length > 0) {
                        for (var itemIndex = 0; itemIndex < vm.items.length; itemIndex++) {
                            vm.sheetMeta[itemIndex] = {
                                itemId: vm.items[itemIndex].id,
                                properties: new Array()
                            }
                            for (var propertyIndex = 0; propertyIndex < vm.properties.length; propertyIndex++) {
                                var property = {
                                    propertyId: vm.properties[propertyIndex].propertyId,
                                    propertyName: vm.properties[propertyIndex].name,
                                    propertyType: vm.properties[propertyIndex].propertyType,
                                    isApplicable: true, isReadOnly: false, isRequired: false
                                };
                                for (var i = 0; i < vm.items[itemIndex].properties.length; i++) {
                                    if (vm.items[itemIndex].properties[i].propertyId
                                        == vm.properties[propertyIndex].propertyId) {
                                        property.itemId = vm.items[itemIndex].id;
                                        property.value = vm.items[itemIndex].properties[i].value;
                                        property.isApplicable = vm.items[itemIndex].properties[i].isApplicable;
                                        property.isReadOnly = vm.items[itemIndex].properties[i].isReadOnly;
                                        property.isRequired = vm.items[itemIndex].properties[i].isRequired;
                                        break;
                                    }
                                }
                                vm.sheetMeta[itemIndex].properties[propertyIndex] = property;
                            }
                        }
                        //prepare sheet values

                        for (var itemIndex = 0; itemIndex < vm.sheetMeta.length; itemIndex++) {
                            vm.sheetValues[itemIndex] = new Array();
                            //set initial value
                            for (var propertyIndex = 0; propertyIndex < vm.properties.length; propertyIndex++) {
                                vm.sheetValues[itemIndex][propertyIndex] = vm.sheetMeta[itemIndex].properties[propertyIndex].value;
                            }
                        }
                    }
                    hotRegisterer.getInstance('excelSheet').updateSettings(vm.settings);
                    hotRegisterer.getInstance('excelSheet').render();
                },
                function () {
                }
            ).finally(function () {
                ui.overlay.hide();
            });
        }

        vm.renderSheet = function () {
            vm.showSheet = true;
            $timeout(function () {
                hotRegisterer.getInstance('excelSheet').render();
            }, 100);
        }
        vm.checkSave = function () {
            hotRegisterer.getInstance('excelSheet').validateCells(function (isValid) {
                if (isValid) {
                }
            });
           
        }

        vm.cancel = function () {
            reset();
        }

        function reset() {
            refresh();
        }

        var enhancedTextEditor = Handsontable.editors.TextEditor.prototype.extend();
        enhancedTextEditor.prototype.prepare = function (row, column, prop, td, originalValue, cellProperties) {
            Handsontable.editors.TextEditor.prototype.prepare.apply(this, arguments);
            if (!cellProperties)
                return;
            if (vm.sheetValues[row][column] && (vm.sheetValues[row][column] + '').trim().startsWith('=')) {
                cellProperties.readOnly = true;
            }
            else if (cellProperties.isReadOnly || vm.properties[column].isInfoOnly) {
                cellProperties.readOnly = true;
            }
            if (!cellProperties.isApplicable) {
                cellProperties.readOnly = true;
                td.style.backgroundColor = '#ccc';
                td.style.color = '#ccc';
            }
            if (cellProperties.isRequired) {
                cellProperties.allowEmpty = false;
            }
        }
        var enhancedNumericEditor = Handsontable.editors.NumericEditor.prototype.extend();
        enhancedNumericEditor.prototype.prepare = function (row, column, prop, td, originalValue, cellProperties) {
            Handsontable.editors.NumericEditor.prototype.prepare.apply(this, arguments);
            if (!cellProperties)
                return;
            td.style.textAlign = 'center';
            if (vm.sheetValues[row][column] && (vm.sheetValues[row][column] + '').trim().startsWith('=')) {
                cellProperties.readOnly = true;
            }
            else if (cellProperties.isReadOnly || vm.properties[column].isInfoOnly) {
                cellProperties.readOnly = true;
            }
            if (!cellProperties.isApplicable) {
                cellProperties.readOnly = true;
                td.style.backgroundColor = '#ccc';
                td.style.color = '#ccc';
            }
            if (cellProperties.isRequired) {
                cellProperties.allowEmpty = false;
            }
        }
        function enhancedTextRenderer(hotInstance, td, row, column, prop, value, cellProperties) {
            Handsontable.renderers.TextRenderer.apply(this, arguments);
            if (!cellProperties)
                return;
            td.style.position = 'relative';
            var html = '<div class="enhanced-cell-icons">';
            
            if (vm.properties[column].isInfoOnly) {
                html += '<div class="enhanced-cell-icon"><i class="fa fa-info"></i></div>';
            }
            else if (!cellProperties.isApplicable) {
                td.style.backgroundColor = '#ccc';
                td.style.color = '#ccc';
            }
            else {
                if (cellProperties.isReadOnly) {
                    html += '<div class="enhanced-cell-icon"><i class="fa fa-lock"></i></div>';
                    if (vm.sheetValues[row][column] && (vm.sheetValues[row][column] + '').trim().startsWith('=')) {
                        var formula = vm.sheetValues[row][column].replace('=', '').replace(/\s/g, '').toLowerCase();
                        var fieldsArr = formula.match(/{[^}]+}/g);//{[0-9a-zA-Z\s]+}
                        if (fieldsArr && fieldsArr.length > 0) {
                            for (var i = 0; i < fieldsArr.length; i++) {
                                var fieldValue = vm.sheetValues[row][vm.propertiesIndex[fieldsArr[i].substring(1, fieldsArr[i].length - 1)]];
                                formula = formula.replace(fieldsArr[i], fieldValue ? fieldValue : 0);
                            }
                            try {
                                var badOperationsArr = formula.match(/[^0-9\+\-\*\/\(\)]/g);
                                if (badOperationsArr && badOperationsArr.length > 0) {
                                    arguments[5] = "!Error: " + value.replace('=', '');
                                }
                                else {
                                    arguments[5] = eval(formula);
                                }
                            } catch (exp) {
                                arguments[5] = '';
                            }
                        }
                        else {
                            arguments[5] = '';
                        }
                        Handsontable.renderers.TextRenderer.apply(this, arguments);
                        html += '<div class="enhanced-cell-icon active"><i class="fa fa-calculator"></i></div>';

                    }
                }
                else {
                    html += '<div class="enhanced-cell-icon active"><i class="fa fa-edit"></i></div>';
                }
                if (cellProperties.isRequired) {
                    html += '<div class="enhanced-cell-icon danger"><i class="fa fa-asterisk"></i></div>';
                }
                else {
                    html += '<div class="enhanced-cell-icon"><i class="fa fa-asterisk"></i></div>';
                }
            }
            html += '</div>';
            angular.element(td).append($compile(html)($scope));
        }
        function enhancedNumericRenderer(hotInstance, td, row, column, prop, value, cellProperties) {
            
            Handsontable.renderers.NumericRenderer.apply(this, arguments);
            if (!cellProperties)
                return;
            td.style.position = 'relative';
            td.style.textAlign = 'center';
            var html = '<div class="enhanced-cell-icons">';
            
            if (vm.properties[column].isInfoOnly) {
                html += '<div class="enhanced-cell-icon"><i class="fa fa-info"></i></div>';
            }
            else if (!cellProperties.isApplicable) {
                td.style.backgroundColor = '#ccc';
                td.style.color = '#ccc';
            }
            else {
                if (cellProperties.isReadOnly) {
                    html += '<div class="enhanced-cell-icon"><i class="fa fa-lock"></i></div>';
                    if (vm.sheetValues[row][column] && (vm.sheetValues[row][column] + '').trim().startsWith('=')) {
                        var formula = vm.sheetValues[row][column].replace('=', '').replace(/\s/g, '').toLowerCase();
                        var fieldsArr = formula.match(/{[^}]+}/g);//{[0-9a-zA-Z\s]+}
                        if (fieldsArr && fieldsArr.length > 0) {
                            for (var i = 0; i < fieldsArr.length; i++) {
                                var fieldValue = vm.sheetValues[row][vm.propertiesIndex[fieldsArr[i].substring(1, fieldsArr[i].length - 1)]];
                                formula = formula.replace(fieldsArr[i], fieldValue ? fieldValue : 0);                                
                            }
                            try {
                                var badOperationsArr = formula.match(/[^0-9\+\-\*\/\(\)]/g);
                                if (badOperationsArr && badOperationsArr.length > 0) {
                                    arguments[5] = "!Error: " + value.replace('=', '');
                                }
                                else {
                                    arguments[5] = eval(formula);
                                }
                            } catch (exp) {
                                arguments[5] = '';
                            }
                        }
                        else {
                            arguments[5] = '';
                        }
                        Handsontable.renderers.TextRenderer.apply(this, arguments);
                        html += '<div class="enhanced-cell-icon active"><i class="fa fa-calculator"></i></div>';

                    }
                }
                else {
                    html += '<div class="enhanced-cell-icon active"><i class="fa fa-edit"></i></div>';
                }
                if (cellProperties.isRequired) {
                    html += '<div class="enhanced-cell-icon danger"><i class="fa fa-asterisk"></i></div>';
                }
                else {
                    html += '<div class="enhanced-cell-icon"><i class="fa fa-asterisk"></i></div>';
                }
            }
            html += '</div>';
            angular.element(td).append($compile(html)($scope));
        }



        // Register an alias
        Handsontable.renderers.registerRenderer('enhancedTextRenderer', enhancedTextRenderer);
        Handsontable.renderers.registerRenderer('enhancedNumericRenderer', enhancedNumericRenderer);
        Handsontable.editors.registerEditor('enhancedTextEditor', enhancedTextEditor);
        Handsontable.editors.registerEditor('enhancedNumericEditor', enhancedNumericEditor);
    }
})();