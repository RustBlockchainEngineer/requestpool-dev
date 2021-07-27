/*
*
*/

(function () {
    'use strict';
    angular.module('app.main').controller('main.enquiries.responses.ctrl', ctrl);

    ctrl.$inject = ['hotRegisterer', '$filter', '$scope', '$state', 'core.system.svc', 'core.settings', '$uibModal', 'ui.svc',
        'main.enquiries.svc', 'main.items.svc', 'main.responses.svc'];

    function ctrl(hotRegisterer, $filter, $scope, $state, system, settings, $uibModal, ui,
        enquiries, items, responses) {

        ui.page.title = system.resources.views.responses;
        // need: enquiry items, enquiry responses, item properties(with dynamic properties)
        var vm = this;
        vm.viewMode = 'list';
        vm.properties = [];// get from server
        vm.infoProperties = [];
        vm.responseProperties = [];
        vm.propertiesRow = new Array();
        vm.propertyResponses = new Array();
        vm.recipientResponsesToCompare = [];
        vm.headerRow = [];
        vm.footerRow = [];
        vm.itemRows = [[]];
        vm.aggregates = [];


        vm.model = { enquiryId: null }
        if ($state.params.item && $state.params.item.id) {
            vm.model.enquiryId = $state.params.item.id;
            vm.enquiry = $state.params.item;
            loadProperties();

        } else if ($state.params.id) {
            ui.overlay.show();
            enquiries.get($state.params.id).then(
                function (response) {
                    $state.params.item = response.data.content;
                    vm.enquiry = $state.params.item;
                    vm.model.enquiryId = $state.params.item.id;
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
            items.includedProperties(vm.model.enquiryId).then(
                function (response) {
                    vm.properties = response.data.content;
                    for (var i = 0; i < vm.properties.length; i++) {
                        if (vm.properties[i].isInfoOnly) {
                            vm.infoProperties.push(vm.properties[i]);
                        }
                        else {
                            vm.responseProperties.push(vm.properties[i]);
                        }
                    }
                    vm.infoProperties.sort(function (a, b) {
                        if (a.rank > b.rank) return -1;
                        else if (a.rank < b.rank) return 1;
                        else return 0;
                    });
                    vm.responseProperties.sort(function (a, b) {
                        if (a.rank > b.rank) return -1;
                        else if (a.rank < b.rank) return 1;
                        else return 0;
                    });
                    refresh();
                },
                function (err) {
                    ui.alerts.http(err);
                }
            ).finally(function () {
                ui.overlay.hide();
            });
        }
        function refresh() {
            ui.overlay.show();
            items.all(vm.model.enquiryId).then(
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
                },
                function () {
                }
            ).finally(function () {
                ui.overlay.hide();
            });

            ui.overlay.show();
            responses.incoming(vm.model.enquiryId).then(
                function (response) {
                    vm.recipientsResponses = response.data.content;
                },
                function () {
                }
            ).finally(function () {
                ui.overlay.hide();
            });
        }

        function loadTableRows() {
            vm.headerRow = [];
            for (var propertyIndex = 0; propertyIndex < vm.infoProperties.length; propertyIndex++) {
                vm.headerRow.push({
                    name: vm.infoProperties[propertyIndex].name
                });
            }
            for (var recipientIndex = 0; recipientIndex < vm.recipientResponsesToCompare.length; recipientIndex++) {
                for (var propertyIndex = 0; propertyIndex < vm.responseProperties.length; propertyIndex++) {
                    vm.headerRow.push({
                        name: vm.responseProperties[propertyIndex].name
                    });
                }
            }
            vm.itemRows = [[]];
            for (var i = 0; i < vm.items.length; i++) {
                vm.itemRows[i] = [];
                for (var propertyIndex = 0; propertyIndex < vm.infoProperties.length; propertyIndex++) {
                    vm.itemRows[i].push({
                        value: vm.items[i].infoProperties[vm.infoProperties[propertyIndex].propertyId],
                        propertyId: vm.infoProperties[propertyIndex].propertyId,
                        isInfoOnly: true
                    });
                }
                for (var recipientIndex = 0; recipientIndex < vm.recipientResponsesToCompare.length; recipientIndex++) {
                    for (var propertyIndex = 0; propertyIndex < vm.responseProperties.length; propertyIndex++) {
                        vm.itemRows[i].push({
                            value: vm.items[i].responseProperties[vm.recipientResponsesToCompare[recipientIndex].id][vm.responseProperties[propertyIndex].propertyId],
                            propertyId: vm.responseProperties[propertyIndex].propertyId,
                            isInfoOnly: false,
                            recipientId: vm.recipientResponsesToCompare[recipientIndex].id
                        });
                    }
                }
            }
            updateFooter();
        }

        function updateFooter() {
            vm.footerRow = [];
            if (vm.itemRows.length > 0) {
                for (var i = 0; i < vm.itemRows[0].length; i++) {
                    vm.footerRow.push({
                        propertyId: vm.itemRows[0][i].propertyId,
                        isInfoOnly: vm.itemRows[0][i].isInfoOnly,
                        recipientId: vm.itemRows[0][i].recipientId,
                        value: getAggregateValue(vm.itemRows[0][i].propertyId, vm.itemRows[0][i].recipientId),
                        aggregateFunction: vm.aggregates[vm.itemRows[0][i].propertyId]
                    });
                }
            }
        }
        function getAggregateValue(propertyId, recipientId) {
            var result = null, count = 0;

            for (var rowIndex = 0; rowIndex < vm.itemRows.length; rowIndex++) {
                for (var cellIndex = 0; cellIndex < vm.itemRows[rowIndex].length; cellIndex++) {
                    if (vm.itemRows[rowIndex][cellIndex].propertyId == propertyId
                        && vm.itemRows[rowIndex][cellIndex].recipientId == recipientId) {
                        try {
                            var tmp = parseInt(vm.itemRows[rowIndex][cellIndex].value);
                            switch (vm.aggregates[propertyId]) {
                                case 'Sum':
                                case 'Average': {
                                    result += tmp;
                                } break;
                                case 'Max': {
                                    if (result == null)
                                        result = tmp;
                                    else if (result < tmp)
                                        result = tmp;
                                } break;
                                case 'Min': {
                                    if (result == null)
                                        result = tmp;
                                    if (result > tmp)
                                        result = tmp;
                                } break;
                                default: { }
                            }
                            count++;
                        } catch (exp) { }
                    }
                }
            }

            if (isNaN(result)) {
                result = null;
            }
            else if (vm.aggregates[propertyId] == 'Average' && count > 0) {
                result = result / count;
            }
            return result;
        }
        vm.updateAggregate = function (propertyId, aggregateFunction) {
            vm.aggregates[propertyId] = aggregateFunction;
            updateFooter();
        }

        function loadResponsesToCompare() {
            for (var itemIndex = 0; itemIndex < vm.items.length; itemIndex++) {
                vm.items[itemIndex].infoProperties = new Array();
                for (var infoPropertyIndex = 0; infoPropertyIndex < vm.infoProperties.length; infoPropertyIndex++) {
                    for (var itemPropertyIndex = 0; itemPropertyIndex < vm.items[itemIndex].properties.length; itemPropertyIndex++) {
                        if (vm.items[itemIndex].properties[itemPropertyIndex].propertyId == vm.infoProperties[infoPropertyIndex].propertyId) {

                            vm.items[itemIndex].infoProperties[vm.infoProperties[infoPropertyIndex].propertyId]
                                = vm.items[itemIndex].properties[itemPropertyIndex].value;
                            break;
                        }
                    }
                }

                vm.items[itemIndex].responseProperties = [[]];

                for (var recipientIndex = 0; recipientIndex < vm.recipientResponsesToCompare.length; recipientIndex++) {
                    vm.items[itemIndex].responseProperties[vm.recipientResponsesToCompare[recipientIndex].id] = new Array();
                    var itemResponse = getItemResponse(vm.recipientResponsesToCompare[recipientIndex], vm.items[itemIndex].id);
                    if (!itemResponse.properties) {
                        itemResponse.properties = new Array();
                    }
                    for (var responsePropertyIndex = 0; responsePropertyIndex < vm.responseProperties.length; responsePropertyIndex++) {
                        var isFormula = false;
                        var value = '';
                        for (var itemPropertyIndex = 0; itemPropertyIndex < vm.items[itemIndex].properties.length; itemPropertyIndex++) {
                            if (vm.items[itemIndex].properties[itemPropertyIndex].propertyId == vm.responseProperties[responsePropertyIndex].propertyId) {
                                value = vm.items[itemIndex].properties[itemPropertyIndex].value;
                                var isReadOnly = vm.items[itemIndex].properties[itemPropertyIndex].isReadOnly;
                                if (isReadOnly && typeof value == 'string' && value.trim().startsWith('=')) {
                                    isFormula = true;
                                    var formula = value.replace('=', '').replace(/\s/g, '').toLowerCase();
                                    var fieldsArr = formula.match(/{[^}]+}/g);//{[0-9a-zA-Z\s]+}
                                    if (fieldsArr && fieldsArr.length > 0) {
                                        for (var i = 0; i < fieldsArr.length; i++) {
                                            var isInfoOnlyFound = false;
                                            // search info only properties
                                            for (var infoPropertyIndex = 0; infoPropertyIndex < vm.infoProperties.length; infoPropertyIndex++) {
                                                if (vm.infoProperties[infoPropertyIndex].name.toLowerCase().replace(/\s/g, '') ==
                                                    fieldsArr[i].substring(1, fieldsArr[i].length - 1)) {
                                                    for (var itemPropertyIndex = 0; itemPropertyIndex < vm.items[itemIndex].properties.length; itemPropertyIndex++) {
                                                        if (vm.items[itemIndex].properties[itemPropertyIndex].propertyId == vm.infoProperties[infoPropertyIndex].propertyId) {
                                                            var fieldValue = vm.items[itemIndex].properties[itemPropertyIndex].value;
                                                            formula = formula.replace(fieldsArr[i], fieldValue ? fieldValue : 0);
                                                            isInfoOnlyFound = true;
                                                            break;
                                                        }
                                                    }
                                                    break;
                                                }
                                            }

                                            if (!isInfoOnlyFound) {
                                                //replace property name with value from recipient responses
                                                for (var itemResponsePropertyIndex = 0; itemResponsePropertyIndex < itemResponse.properties.length; itemResponsePropertyIndex++) {
                                                    if (itemResponse.properties[itemResponsePropertyIndex].name.toLowerCase().replace(/\s/g, '') ==
                                                        fieldsArr[i].substring(1, fieldsArr[i].length - 1)) {
                                                        var fieldValue = itemResponse.properties[itemResponsePropertyIndex].value;
                                                        formula = formula.replace(fieldsArr[i], fieldValue ? fieldValue : 0);
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                        try {
                                            var badOperationsArr = formula.match(/[^0-9\+\-\*\/\(\)]/g);
                                            if (badOperationsArr && badOperationsArr.length > 0) {
                                                value = "!Error: " + value.replace('=', '');
                                            }
                                            else {
                                                value = eval(formula);
                                            }
                                        } catch (exp) {
                                            value = "!Error: " + value.replace('=', '');
                                        }
                                    }
                                    else {
                                        value = "! " + value.replace('=', '');
                                    }
                                }
                                break;

                            }
                        }
                        if (isFormula) {
                            vm.items[itemIndex].responseProperties[vm.recipientResponsesToCompare[recipientIndex].id][vm.responseProperties[responsePropertyIndex].propertyId]
                                = value;
                        }
                        else {
                            vm.items[itemIndex].responseProperties[vm.recipientResponsesToCompare[recipientIndex].id][vm.responseProperties[responsePropertyIndex].propertyId]
                                = getPropertyResponse(itemResponse, vm.responseProperties[responsePropertyIndex].propertyId);
                        }
                    }
                }
            }
            loadTableRows();
        }

        function getItemResponse(recipientResponse, itemId) {
            var item = {};
            for (var i = 0; i < recipientResponse.itemsResponse.length; i++) {
                if (recipientResponse.itemsResponse[i].itemId == itemId) {
                    item = recipientResponse.itemsResponse[i];
                    break;
                }
            }
            return item;
        }
        function getPropertyResponse(itemResponse, propertyId) {
            var response = null;
            for (var p = 0; p < itemResponse.properties.length; p++) {
                if (itemResponse.properties[p].propertyId == propertyId) {
                    response = itemResponse.properties[p].value;
                    break;
                }
            }
            return response;
        }

        vm.cancel = function () {

            reset();
        }

        function reset() {
            refresh();
        }

        vm.updateCompareList = function (item) {
            var index = -1;
            for (var i = 0; i < vm.recipientResponsesToCompare.length; i++) {
                if (vm.recipientResponsesToCompare[i].id == item.id) {
                    index = i;
                    break;
                }
            }
            if (index != -1) {
                vm.recipientResponsesToCompare.splice(index, 1);
            }
            else {
                vm.recipientResponsesToCompare.push(angular.copy(item));
            }
        }

        vm.compare = function () {
            loadResponsesToCompare();
            vm.viewMode = 'compare';
        }

        vm.exportToExcel = function (tableId) {
            var wb = XLSX.utils.table_to_book(document.getElementById(tableId), { sheet: "Sheet1" });
            var wbout = XLSX.write(wb, { bookType: 'xlsx', bookSST: true, type: 'binary' });
            var fname = $state.params.item.subject + '.xlsx';
            try {
                saveAs(new Blob([s2ab(wbout)], { type: "application/octet-stream" }), fname);
            } catch (e) {

            }
        }
        function s2ab(s) {
            if (typeof ArrayBuffer !== 'undefined') {
                var buf = new ArrayBuffer(s.length);
                var view = new Uint8Array(buf);
                for (var i = 0; i != s.length; ++i) view[i] = s.charCodeAt(i) & 0xFF;
                return buf;
            } else {
                var buf = new Array(s.length);
                for (var i = 0; i != s.length; ++i) buf[i] = s.charCodeAt(i) & 0xFF;
                return buf;
            }
        }

        vm.downloadAttachments = function () {
            let recipientsIds = [];
            for (var i = 0; i < vm.recipientResponsesToCompare.length; i++) {
                recipientsIds.push(vm.recipientResponsesToCompare[i].id);
            }
            ui.overlay.show();
            responses.downloadAttachments(recipientsIds).then(
                function (response) {
                    //console.log(response);
                    var file = new Blob([response.data], { type: 'application/zip' });
                    saveAs(file, 'attachments.zip');
                },
                function (err) {
                    ui.alerts.http(err);
                }
            ).finally(function () {
                ui.overlay.hide();
            });
        }

        vm.selectAll = function () {
            vm.recipientResponsesToCompare = [];
            if (vm.allSelected) {
                for (var i = 0; i < vm.displayedList.length; i++) {
                    vm.recipientResponsesToCompare.push(angular.copy(vm.displayedList[i]));
                    vm.displayedList[i].checked = true;
                }
            }
            else {
                
                for (var i = 0; i < vm.displayedList.length; i++) {
                    vm.displayedList[i].checked = false;
                }
            }
        }

        vm.view = function (item) {
            vm.recipientResponsesToCompare = [];
            for (var i = 0; i < vm.displayedList.length; i++) {
                vm.displayedList[i].checked = false;
            }
            item.checked = true;
            vm.updateCompareList(item);
            vm.compare();
        }
    }
})();