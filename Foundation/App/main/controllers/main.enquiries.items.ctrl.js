/*
*
*/

(function () {
    'use strict';
    angular.module('app.main').controller('main.enquiries.items.ctrl', ctrl);

    ctrl.$inject = ['$compile','$timeout', 'hotRegisterer', '$filter', '$scope', '$state', 'core.system.svc', 'core.settings', '$uibModal', 'ui.svc',
        'main.enquiries.svc', 'main.items.svc', 'main.itemTypes.svc','main.itemsProperties.svc','main.membership.svc'];

    function ctrl($compile,$timeout, hotRegisterer, $filter, $scope, $state, system, settings, $uibModal, ui,
        enquiries, items, itemTypes, itemsProperties,membership) {
        ui.page.title = system.resources.views.items;

        var vm = this;
        vm.showSheet = false;
        vm.showPropertyDialog = false;
        vm.enquiryId = null;
        vm.dynamicProperties = [];
        vm.includedProperties = [];
        vm.includedPropertiesIndex = [];
        vm.includedPropertiesModel = [];
        vm.items = [];
        vm.sheetValues = new Array();
        vm.sheetMeta = new Array();


        vm.settings = {
            rowHeaders: true,
            columnHeaderHeight: 35,
            startCols: 0,
            startRows: 0,
            minSpareRows: 4,
            stretchH: 'last',
            colWidths: 150,
            wordWrap: true,
            autoWrapRow: true,
            manualColumnResize: true,
            outsideClickDeselects: false ,
            maxRows: 100,
            colHeaders: function (index) {
                if (index < 0 || !vm.includedProperties || vm.includedProperties.length == 0)
                    return null;
                var html = '';
                if (vm.includedProperties[index].isPublic) {
                    html += '<i class="fa fa-eye " style="display:inline-block;margin:5px;"></i>';
                }
                else {
                    html += '<i class="fa fa-eye-slash " style="display:inline-block;margin:5px;"></i>';
                }
                if (vm.includedProperties[index].isInfoOnly) {
                    html += '<i class="fa fa-info-circle " style="display:inline-block;margin:5px;"></i>';
                }
                else {
                    html += '<i class="fa fa-edit " style="display:inline-block;margin:5px;"></i>';
                }
                html += vm.includedProperties[index].name;
                return html;
            },
            cells: function (row, col, prop) {
                //console.log('[' + row + '][' + col + ']');
                //row = row - 1;
                //col = col - 1;
                if (row < 0 || col < 0 || !vm.includedProperties || vm.includedProperties.length == 0)
                    return {};
                if (vm.sheetMeta[row] == undefined)
                    vm.sheetMeta[row] = new Array();
                if (vm.sheetMeta[row][col] == undefined) {
                    vm.sheetMeta[row][col] = {
                        itemId: null,
                        propertyId: vm.includedProperties[col].propertyId,
                        propertyName: vm.includedProperties[col].name,
                        propertyType: vm.includedProperties[col].propertyType,
                        isInfoOnly: vm.includedProperties[col].isInfoOnly,
                        isApplicable: true, isReadOnly: false, isRequired: false
                    };
                }
                var cellProperties = {
                    uiRegex: vm.sheetMeta[row][col].propertyType.uiRegex,
                    isApplicable: vm.sheetMeta[row][col].isApplicable,
                    isReadOnly: vm.sheetMeta[row][col].isReadOnly,
                    isRequired: vm.sheetMeta[row][col].isRequired,
                    type: vm.sheetMeta[row][col].propertyType.name,
                    renderer: getRendrer(vm.sheetMeta[row][col].propertyType.name),
                    editor: getEditor(vm.sheetMeta[row][col].propertyType.name),
                    allowInvalid:getValidator(row,col,vm.sheetMeta[row][col].propertyType.name),
                    validator: getValidator(row,col,vm.sheetMeta[row][col].propertyType.name)

                };

                return cellProperties;
            },
            data: vm.sheetValues
        };

        function getRendrer(type) {
            var rendrer = 'enhancedTextRenderer';
            switch (type) {
                case 'text': {
                    rendrer =  'enhancedTextRenderer';
                } break;
                case 'numeric': {
                    rendrer = 'enhancedNumericRenderer';
                } break;
                default:{};
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
        function getValidator(row,col,type) {
            var validator = null;
            switch (type) {
                case 'text': {
                    validator = null;
                } break;
                case 'numeric': {
                    if (typeof (vm.sheetValues[row][col]) == 'string' && vm.sheetValues[row][col].startsWith('='))
                    {
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
        
        if ($state.params.item && $state.params.item.id) {
            vm.enquiryId = $state.params.item.id;
            loadDynamicProperties();

        } else if ($state.params.id) {
            ui.overlay.show();
            enquiries.get($state.params.id).then(
                function (response) {
                    $state.params.item = response.data.content;
                    vm.enquiryId = $state.params.item.id;
                    loadDynamicProperties();
                },
                function () {
                }
            ).finally(function () {
                ui.overlay.hide();
            });
        }
        else {
            if ($state.params.isTemplate)
                $state.go('main/templates/list');
            else
                $state.go('main/enquiries/list');
        }
        ui.overlay.show();
        enquiries.getInvitationsCount(vm.enquiryId).then(
            function (response) {
                vm.isSaveEnabled = response.data.content === 0;
            },
            function (err) {
                ui.alerts.http(err);
            }
        ).finally(function () {
            ui.overlay.hide();
        });
        function loadDynamicProperties() {
            ui.overlay.show();
            itemsProperties.search().then(
                function (response) {
                    //remove deleted properties
                    for (var i = 0; i < response.data.content.length; i++) {
                        if (response.data.content[i].isDeleted === true) {
                            response.data.content[i].splice(i, 1);
                        }
                    }
                    vm.dynamicProperties = response.data.content;
                    loadIncludedProperties();
                },
                function (err) {
                    ui.alerts.http(err);
                }
            ).finally(function () {
                ui.overlay.hide();
            });
        }
        function loadIncludedProperties() {
            ui.overlay.show();
            items.includedProperties(vm.enquiryId).then(
                function (response) {
                    vm.includedProperties = response.data.content;
                    vm.includedProperties.sort(function (a, b) {
                        if (a.rank > b.rank) return -1;
                        else if (a.rank < b.rank) return 1;
                        else return 0;
                    });
                    updateIncludedPropertiesIndex();
                    if (vm.includedProperties.length > 0)
                    {
                        loadItems();
                    }
                    else
                    {
                        vm.loadPropertiesDialog();
                    }
                    
                },
                function (err) {
                    ui.alerts.http(err);
                }
            ).finally(function () {
                ui.overlay.hide();
            });
        }

        function updateIncludedPropertiesIndex() {
            vm.includedPropertiesIndex = [];
            for (var i = 0; i < vm.includedProperties.length; i++) {
                vm.includedPropertiesIndex[vm.includedProperties[i].name.toLowerCase().replace(' ', '')] = i;
            }
        }
        vm.loadPropertiesDialog = function () {
            vm.dialogProperties = new Array();
            //console.log(vm.dynamicProperties);
            //console.log(vm.includedProperties);
            for (var i = 0; i < vm.dynamicProperties.length; i++) {
                var property = angular.copy(vm.dynamicProperties[i]);
                property.rank = 0;
                property.isPublic = true;
                for (var j = 0; j < vm.includedProperties.length; j++) {
                    if (vm.includedProperties[j].propertyId == vm.dynamicProperties[i].id) {
                        property.rank = vm.includedProperties[j].rank;
                        property.isPublic = vm.includedProperties[j].isPublic;
                        property.isInfoOnly = vm.includedProperties[j].isInfoOnly;
                        property.isIncluded = true;
                        break;
                    }
                }
                vm.dialogProperties.push(property);
            }
            sortDialogProperties();
            
            // show dialog
            vm.showPropertiesDialog = true;
        }
       
        function sortDialogProperties() {
            vm.dialogProperties.sort(function (a, b) {
                if (a.isIncluded && !b.isIncluded) {
                    return -1;
                }
                else if (b.isIncluded && !a.isIncluded) {
                    return 1;
                }
                else {
                    if (a.rank > b.rank) return -1;
                    else if (a.rank < b.rank) return 1;
                    else return 0;
                }
            });
        }
        function loadItems() {
            ui.overlay.show();
            items.all(vm.enquiryId).then(
                function (response) {
                    vm.items = response.data.content;
                    for (var itemIndex = 0; itemIndex < vm.items.length; itemIndex++) {
                        if (vm.items[itemIndex].properties && vm.items[itemIndex].properties.length > 0) {
                            for (var propertyIndex = 0; propertyIndex < vm.items[itemIndex].properties.length; propertyIndex++) {
                                var value = vm.items[itemIndex].properties[propertyIndex].value;
                                if (typeof value == 'string' && value.startsWith('=')) {
                                    var fieldsArr = value.match(/{[^}]+}/g);
                                    if (fieldsArr && fieldsArr.length > 0) {
                                        for (var i = 0; i < fieldsArr.length; i++) {
                                            for (var j = 0; j < vm.dynamicProperties.length; j++) {
                                                if (vm.dynamicProperties[j].id ==
                                                    fieldsArr[i].trim().substring(1, fieldsArr[i].length - 1)) {
                                                    vm.items[itemIndex].properties[propertyIndex].value =
                                                        vm.items[itemIndex].properties[propertyIndex].value.replace(fieldsArr[i], '{' + vm.dynamicProperties[j].name + '}');
                                                    break;
                                                }
                                            }      
                                        }
                                    }
                                }
                            }
                        }
                    }
                    refresh();
                },
                function () {
                }
            ).finally(function () {
                ui.overlay.hide();
            });
        }

        function refresh() {
            // prepare sheet metadata
            vm.sheetMeta = [];
            if (vm.items.length == 0) {
                vm.sheetValues[0] = new Array();
                for (var propertyIndex = 0; propertyIndex < vm.includedProperties.length; propertyIndex++) {
                    vm.sheetValues[0][propertyIndex] = '';
                }
            }
            else {
                for (var itemIndex = 0; itemIndex < vm.items.length; itemIndex++) {
                    vm.sheetMeta[itemIndex] = new Array();
                    for (var propertyIndex = 0; propertyIndex < vm.includedProperties.length; propertyIndex++) {
                        var property = {
                            propertyId: vm.includedProperties[propertyIndex].propertyId,
                            propertyName: vm.includedProperties[propertyIndex].name,
                            propertyType: vm.includedProperties[propertyIndex].propertyType,
                            isApplicable: true, isReadOnly: false, isRequired: false
                        };
                        for (var i = 0; i < vm.items[itemIndex].properties.length; i++) {
                            if (vm.items[itemIndex].properties[i].propertyId
                                == vm.includedProperties[propertyIndex].propertyId) {
                                property.value = vm.items[itemIndex].properties[i].value;
                                property.isApplicable = vm.items[itemIndex].properties[i].isApplicable;
                                property.isReadOnly = vm.items[itemIndex].properties[i].isReadOnly;
                                property.isRequired = vm.items[itemIndex].properties[i].isRequired;
                                break;
                            }
                        }
                        vm.sheetMeta[itemIndex][propertyIndex] = property;
                    }
                }
                //prepare sheet values
                for (var itemIndex = 0; itemIndex < vm.sheetMeta.length; itemIndex++) {
                    vm.sheetValues[itemIndex] = new Array();
                    for (var propertyIndex = 0; propertyIndex < vm.includedProperties.length; propertyIndex++) {
                        vm.sheetValues[itemIndex][propertyIndex] = vm.sheetMeta[itemIndex][propertyIndex].value;
                    }
                }
            }
            hotRegisterer.getInstance('excelSheet').updateSettings(vm.settings);
            hotRegisterer.getInstance('excelSheet').render();
        }

        vm.renderSheet = function () {
            if (vm.includedProperties.length > 0) {
                vm.showSheet = true;
                $timeout(function () {
                    hotRegisterer.getInstance('excelSheet').render();
                }, 100);
            }
            else {
                vm.loadPropertiesDialog();
            }
            
        }
      
        vm.cancel = function () {

            reset();
        }

        function reset() {
            refresh();
        }


        function enhancedTextRenderer(hotInstance, td, row, column, prop, value, cellProperties) {
            // Optionally include `BaseRenderer` which is responsible for adding/removing CSS classes to/from the table cells.
            Handsontable.renderers.TextRenderer.apply(this, arguments);
            
            if (!cellProperties)
                return;
            td.style.position = 'relative';
            var html = '<div class="enhanced-cell-icons">';
            
            if (vm.includedProperties[column].isInfoOnly)
            {
                html += '<div class="enhanced-cell-icon"><i class="fa fa-info"></i></div>';
            }
            else {
                
                if (cellProperties.isReadOnly) {
                    html += '<div class="enhanced-cell-icon" ' +
                        'ng-click="vm.toggleCellProperty(' + row + ',' + column + ',\'read-only\')"><i class="fa fa-lock"></i></div>';
                    if (!vm.sheetMeta[row][column].isInfoOnly && vm.sheetValues[row][column] && (vm.sheetValues[row][column] + '').trim().startsWith('=')) {
                        var formula = vm.sheetValues[row][column].replace('=', '').replace(/\s/g, '').toLowerCase();
                        var fieldsArr = formula.match(/{[^}]+}/g);//{[0-9a-zA-Z\s]+}
                        if (fieldsArr && fieldsArr.length > 0) {
                            for (var i = 0; i < fieldsArr.length; i++) {
                                var fieldValue = vm.sheetValues[row][vm.includedPropertiesIndex[fieldsArr[i].substring(1, fieldsArr[i].length - 1)]];
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
                                arguments[5] = "!Error: " + vm.sheetValues[row][column].replace('=', '');
                            }
                        }
                        else {
                            arguments[5] = "! " + vm.sheetValues[row][column].replace('=', '');
                        }
                        Handsontable.renderers.TextRenderer.apply(this, arguments);

                        html += '<div class="enhanced-cell-icon active"><i class="fa fa-calculator"></i></div>';

                    }
                }
                else {           
                    html += '<div class="enhanced-cell-icon active" ' +
                        'ng-click="vm.toggleCellProperty(' + row + ',' + column + ',\'read-only\')"><i class="fa fa-edit"></i></div>';
                }
                if (cellProperties.isApplicable) {
                    html += '<div class="enhanced-cell-icon active" ' +
                        'ng-click="vm.toggleCellProperty(' + row + ',' + column + ',\'applicable\')"><i class="fa fa-check-circle"></i></div>';
                }
                else {
                    html += '<div class="enhanced-cell-icon" ' +
                        'ng-click="vm.toggleCellProperty(' + row + ',' + column + ',\'applicable\')"><i class="fa fa-ban"></i></div>';
                }
                if (cellProperties.isRequired) {
                    html += '<div class="enhanced-cell-icon danger" ' +
                        'ng-click="vm.toggleCellProperty(' + row + ',' + column + ',\'required\')"><i class="fa fa-asterisk"></i></div>';
                }
                else {
                    html += '<div class="enhanced-cell-icon" ' +
                        'ng-click="vm.toggleCellProperty(' + row + ',' + column + ',\'required\')"><i class="fa fa-asterisk"></i></div>';
                    
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
            
            if (vm.includedProperties[column].isInfoOnly) {
                html += '<div class="enhanced-cell-icon"><i class="fa fa-info "></i></div>';
            }
            else {

                if (cellProperties.isReadOnly) {
                    html += '<div class="enhanced-cell-icon" ' +
                        'ng-click="vm.toggleCellProperty(' + row + ',' + column + ',\'read-only\')"><i class="fa fa-lock"></i></div>';
                    if (!vm.sheetMeta[row][column].isInfoOnly && vm.sheetValues[row][column] && (vm.sheetValues[row][column] + '').trim().startsWith('=')) {
                        var formula = vm.sheetValues[row][column].replace('=', '').replace(/\s/g, '').toLowerCase();
                        var fieldsArr = formula.match(/{[^}]+}/g);//{[0-9a-zA-Z\s]+}
                        if (fieldsArr && fieldsArr.length > 0) {
                            for (var i = 0; i < fieldsArr.length; i++) {
                                var fieldValue = vm.sheetValues[row][vm.includedPropertiesIndex[fieldsArr[i].substring(1, fieldsArr[i].length - 1)]];
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
                                arguments[5] = "!Error: " + vm.sheetValues[row][column].replace('=', '');
                            }
                        }
                        else {
                            arguments[5] = "! " + vm.sheetValues[row][column].replace('=', '');
                        }
                        Handsontable.renderers.TextRenderer.apply(this, arguments);

                        html += '<div class="enhanced-cell-icon active"><i class="fa fa-calculator"></i></div>';

                    }
                }
                else {
                    html += '<div class="enhanced-cell-icon active" ' +
                        'ng-click="vm.toggleCellProperty(' + row + ',' + column + ',\'read-only\')"><i class="fa fa-edit"></i></div>';
                }
                if (cellProperties.isApplicable) {
                    html += '<div class="enhanced-cell-icon active" ' +
                        'ng-click="vm.toggleCellProperty(' + row + ',' + column + ',\'applicable\')"><i class="fa fa-check-circle"></i></div>';
                }
                else {
                    html += '<div class="enhanced-cell-icon" ' +
                        'ng-click="vm.toggleCellProperty(' + row + ',' + column + ',\'applicable\')"><i class="fa fa-ban"></i></div>';
                }
                if (cellProperties.isRequired) {
                    html += '<div class="enhanced-cell-icon danger" ' +
                        'ng-click="vm.toggleCellProperty(' + row + ',' + column + ',\'required\')"><i class="fa fa-asterisk"></i></div>';
                }
                else {
                    html += '<div class="enhanced-cell-icon" ' +
                        'ng-click="vm.toggleCellProperty(' + row + ',' + column + ',\'required\')"><i class="fa fa-asterisk"></i></div>';

                }
            }
            html += '</div>';
            angular.element(td).append($compile(html)($scope));
            
        }
        var enhancedNumericEditor = Handsontable.editors.NumericEditor.prototype.extend();
        enhancedNumericEditor.prototype.prepare = function (row, column, prop, td, originalValue, cellProperties) {
            Handsontable.editors.NumericEditor.prototype.prepare.apply(this, arguments);
            if (!cellProperties)
                return;
            td.style.textAlign = 'center';
        }
        var enhancedTextEditor = Handsontable.editors.TextEditor.prototype.extend();
        enhancedTextEditor.prototype.prepare = function (row, column, prop, td, originalValue, cellProperties) {
            Handsontable.editors.TextEditor.prototype.prepare.apply(this, arguments);
            if (!cellProperties)
                return;
        }
        // Register an alias
        Handsontable.renderers.registerRenderer('enhancedTextRenderer', enhancedTextRenderer);
        Handsontable.renderers.registerRenderer('enhancedNumericRenderer', enhancedNumericRenderer);
        Handsontable.editors.registerEditor('enhancedTextEditor', enhancedTextEditor);
        Handsontable.editors.registerEditor('enhancedNumericEditor', enhancedNumericEditor);

    }
})();