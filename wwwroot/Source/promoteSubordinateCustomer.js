var subordinateCustomerGridUrl;
var clcBalanceEntryGridUrl;
var promoteSubordinateCustomerUrl;
var promoteCLCBalanceEntriesUrl;
var selectedCLCBalanceEntries = [];

const ERROR_MESSAGE_AUTO_DISMISS_MILLISECONDS = 5000;

$(function () {

    $("#errorMessageBlock").hide();
    $("#noCLCBalanceEntriesToPromote").prop("checked", false);
    $("#noCLCBalanceEntriesToPromote").attr("disabled", "disabled");
    $("#promoteSubordinateCustomer").attr("disabled", "disabled");

    $("#subordinateCustomerSearchMask").keypress(function (event) {
        if (event.keyCode === 13) {
            $("#searchForSubordinateCustomer").click();
        }
    });

    $("#promoteSubordinateCustomer").click(function () {

        // Perform validations
        $("#errorMessageText").html("");
        $("#errorMessageBlock").hide();

        var optionChecked = false;
        var continueProcessing = false;
        var listRows = $("#clcBalanceEntryList").getDataIDs();

        if (listRows.length > 0) {

            for (i = 0; i < listRows.length; i++) {

                var rowData = $("#clcBalanceEntryList").getRowData(listRows[i]);

                if (rowData.marked == "Yes") {
                    optionChecked = true;
                    continueProcessing = true;
                    i = listRows.length;
                }

            }       // for (i = 0; i < listRows.length; i++) {

            if (!continueProcessing) {
                if (!optionChecked) {
                    if (!$("#noCLCBalanceEntriesToPromote").is(":checked")) {
                        $("#errorMessageText").html("Must mark one or more CLC Balance Entries to promote or check the 'No CLC Balance Entries to Promote' checkbox");
                        $("#errorMessageBlock").show();
                    } else {
                        continueProcessing = true;
                    }
                } else {
                    continueProcessing = true;
                }
            }

        } else {
            continueProcessing = true;
        }

        if (continueProcessing) {

            continueProcessing = false;

            promoteSubordinateCustomerUrl = $("#baseWebServiceUrl").val() + "/ws/subordinateCustomerEntry/" + $("#selectedSubordinateCustomerEntryId").val() +
                                            "/promoteToMasterCustomerEntry/" + $("#selectedLinkedMasterCustomerEntryId").val() + "/";

            $.ajax({
                type: "PUT",
                dataType: "json",
                url: promoteSubordinateCustomerUrl,
                contentType: "application/json",
                success: function (customer) {

                    if (selectedCLCBalanceEntries.length > 0) {

                        //clcBalanceEntries/assignToDifferentMasterCustomerEntry/

                        promoteCLCBalanceEntriesUrl = $("#baseWebServiceUrl").val() + "/ws/clcBalanceEntries/assignToDifferentMasterCustomerEntry/" +
                                                      $("#selectedLinkedMasterCustomerEntryId").val() + "/";

                        $.ajax({
                            type: "PUT",
                            url: promoteCLCBalanceEntriesUrl,
                            traditional: true,
                            data: JSON.stringify(selectedCLCBalanceEntries),
                            contentType: "application/json; charset=utf-8",
                            dataType: "json",
                            success: function (returnValue) {

                                continueProcessing = true;
                            },
                            error: function (e) {

                                // PROGRAMMER'S NOTE:  Need to handle case of error encountered during AJAX call

                            }
                        });     // .ajax()

                        if (continueProcessing) {
                            subordinateCustomerGridUrl = $("#baseWebServiceUrl").val() + "/ws/subordinateCustomerEntry/searchByNameMask/" + btoa("~~~~~~~~~") + "/0/";
                            $("#subordinateCustomerList").jqGrid('setGridParam', { url: subordinateCustomerGridUrl }).trigger("reloadGrid");
                        }

                    }       // (selectedCLCBalanceEntries.length > 0)

                },
                error: function (jqXHR, textStatus, errorThrown) {

                    // PROGRAMMER'S NOTE:  Need to handle case of error encountered during AJAX call

                }
            });      // ajax()

        }       // (continueProcessing)

    });

    $("#searchForSubordinateCustomer").click(function () {

        $("#errorMessageText").html("");
        $("#errorMessageBlock").hide();

        if ($("#subordinateCustomerSearchMask").val() === "") {

            $("#errorMessageText").html("Subordinate Customer search text must be specified");
            $("#errorMessageBlock").show();

        } else {

            $("#errorMessageBlock").hide();

            subordinateCustomerGridUrl = $("#baseWebServiceUrl").val() + "/ws/subordinateCustomerEntry/searchByNameMask/" + btoa($("#subordinateCustomerSearchMask").val().toUpperCase());

            if ($("#onlyFromBeginningOfNameSubordinateCustomer").is(":checked")) {
                // from beginning of name
                subordinateCustomerGridUrl += "/1/";
            } else {
                // anywhere in name
                subordinateCustomerGridUrl += "/0/";
            }

            $("#subordinateCustomerList").jqGrid('setGridParam', { url: subordinateCustomerGridUrl }).trigger("reloadGrid");

        }

    });     // $("#searchForSubordinateCustomer").click(function ()

    $("#subordinateCustomerList").jqGrid({
        url: subordinateCustomerGridUrl,
        type: 'GET',
        ajaxGridOptions: { contentType: "application/json", timeout: 60000 },
        datatype: "json",
        serializeGridData: function (postData) {
            return JSON.stringify(postData);
        },
        jsonReader: {
            root: function (obj) { return obj; },
            page: function (obj) { return 1; },
            total: function (obj) { return 1; },
            records: function (obj) { return obj.length; },
            id: '0',
            cell: '',
            repeatitems: false
        },
        scroll: true,
        scrollrows: true,
        height: 250,
        rowNum: 5000,
        colNames: ['Subordinate Entry ID', 'Linked Master Customer Entry ID', 'Master Customer Entry ID', 'Subordinate Customer Name',
                   'Corporate Customer HQ Name', 'End Customer No.', 'Sales Level 1', 'Sales Level 2', 'Sales Level 3'],
        colModel: [

            {
                name: 'subordinateCustomerEntryId',
                index: 'subordinateCustomerEntryId',
                width: 50,
                align: "left",
                key: true,
                hidden: true,
                editable: false
            },
            {
                name: 'linkedMasterCustomerEntryId',
                index: 'linkedMasterCustomerEntryId',
                width: 50,
                align: "left",
                hidden: true,
                editable: false
            },
            {
                name: 'masterCustomerEntryId',
                index: 'masterCustomerEntryId',
                width: 50,
                align: "left",
                hidden: true,
                editable: false
            },
            {
                name: 'subordinateCustomerName',
                index: 'subordinateCustomerName',
                align: "left",
                width: 260,
                editable: false
            },
            {
                name: 'endCustomerHeadquartersName',
                index: 'endCustomerHeadquartersName',
                align: "left",
                width: 260,
                editable: false
            },
            {
                name: 'endCustomerId',
                index: 'endCustomerId',
                align: "left",
                width: 125,
                editable: false
            },
            {
                name: 'salesLevel1',
                index: 'salesLevel1',
                align: "left",
                width: 100,
                editable: false
            },
            {
                name: 'salesLevel2',
                index: 'salesLevel2',
                align: "left",
                width: 200,
                editable: false
            },
            {
                name: 'salesLevel3',
                index: 'salesLevel3',
                align: "left",
                width: 200,
                editable: false
            }
        ],
        beforeRequest: function () {

            $("#selectedSubordinateCustomerEntryId").val("");
            $("#selectedLinkedMasterCustomerEntryId").val("");
            $("#selectedMasterCustomerEntryId").val("");
            $("#promoteSubordinateCustomer").attr("disabled", "disabled");

            clcBalanceEntryGridUrl = $("#baseWebServiceUrl").val() + "/ws/clcBalanceEntry/activeForMasterCustomer/-1/";
            $("#clcBalanceEntryList").jqGrid('setGridParam', { url: clcBalanceEntryGridUrl }).trigger("reloadGrid");

        },
        loadComplete: function () {

            /*
            var listRows = $(this).getDataIDs();

            for (i = 0; i < listRows.length; i++) {

                var rowData = $(this).getRowData(listRows[i]);

                // Example for coloring rows
                //if (rowData.resubmitInstance > 0 && rowData.requisitionStatus === REQUISITION_STATUS_DESCRIPTION_RETURNED_TO_REQUESTOR &&
                //    rowData.returnToRequestorReason === REQUISITION_RETURN_REASON_REVISE_FUNDS) {
                //    $("#" + rowData.requisitionId, $(this)).removeClass("ui-widget-content").addClass("returnedRequisitionIncreaseFunding");
                //    blnReturnedIncreaseFundingExists = true;
                //} 

            }
            */

        },       // loadcomplete()
        onSelectRow: function (rowId, status, e) {

            // Only process left mouse-click
            if (!e || e.which === 1) {

                var rowData = $(this).getRowData(rowId);

                $("#selectedSubordinateCustomerEntryId").val(rowId);
                $("#selectedLinkedMasterCustomerEntryId").val(rowData.linkedMasterCustomerEntryId);
                $("#selectedMasterCustomerEntryId").val(rowData.masterCustomerEntryId);

                $("#promoteSubordinateCustomer").removeAttr("disabled");
                
                clcBalanceEntryGridUrl = $("#baseWebServiceUrl").val() + "/ws/clcBalanceEntry/activeForMasterCustomer/" + rowData.masterCustomerEntryId + "/";
                $("#clcBalanceEntryList").jqGrid('setGridParam', { url: clcBalanceEntryGridUrl }).trigger("reloadGrid");

            }       // left-click
        }       // onSelectRow event

    });      // $("#masterCustomerList").jqGrid()

    $("#clcBalanceEntryList").jqGrid({
        url: clcBalanceEntryGridUrl,
        type: 'GET',
        ajaxGridOptions: { contentType: "application/json", timeout: 60000 },
        datatype: "json",
        serializeGridData: function (postData) {
            return JSON.stringify(postData);
        },
        jsonReader: {
            root: function (obj) { return obj; },
            page: function (obj) { return 1; },
            total: function (obj) { return 1; },
            records: function (obj) { return obj.length; },
            id: '0',
            cell: '',
            repeatitems: false
        },
        scroll: true,
        scrollrows: true,
        height: 250,
        rowNum: 5000,
        colNames: ['Entry ID', 'Sales Order No.', 'Team Captain Name', 'Credits Purchased', 'Current Balance', 'Expiration Date', 'LCMT Customer Name','Select '],
        colModel: [
            {
                name: 'clcBalanceEntryId',
                index: 'clcBalanceEntryId',
                width: 50,
                align: "left",
                key: true,
                hidden: true,
                editable: false
            },
            {
                name: 'salesOrderNumber',
                index: 'salesOrderNumber',
                align: "left",
                width: 150,
                editable: false
            },
            {
                name: 'teamCaptainName',
                index: 'teamCaptainName',
                align: "left",
                width: 200,
                editable: false
            },
            {
                name: 'creditsPurchased',
                index: 'creditsPurchased',
                align: "right",
                width: 110,
                editable: false
            },
            {
                name: 'currentBalance',
                index: 'currentBalance',
                align: "right",
                width: 100,
                editable: false
            },
            {
                name: 'expirationDate',
                index: 'expirationDate',
                formatter: 'date', formatoptions: { srcformat: 'Y-M-d', newformat: 'Y-m-d' },
                align: "right",
                width: 100,
                editable: false
            },
            {
                name: 'lcmtCustomerName',
                index: 'lcmtCustomerName',
                align: "left",
                width: 350,
                editable: false
            },
            {
                name: 'marked',
                index: 'marked',
                align: "center",
                width: 50,
                formatter: "checkbox",
                formatoptions: { disabled: false },
                editable: true
            }

        ],
        beforeRequest: function () {

            selectedCLCBalanceEntries = [];
            $("#noCLCBalanceEntriesToPromote").prop("checked", false);
            $("#noCLCBalanceEntriesToPromote").attr("disabled", "disabled");

        },
        loadComplete: function () {

            var listRows = $(this).getDataIDs();

            if (listRows.length > 0) {
                $("#noCLCBalanceEntriesToPromote").removeAttr("disabled");
            }

        },       // loadcomplete()
        beforeSelectRow: function (rowId, e) {

            var $self = $(this);
            var iCol = $.jgrid.getCellIndex($(e.target).closest("td")[0]);
            var cm = $self.jqGrid("getGridParam", "colModel");
            var localData = $self.jqGrid("getLocalRow", rowId);

            if (cm[iCol].name === "marked" && e.target.tagName.toUpperCase() === "INPUT") {

                if ($(e.target).is(":checked")) {
                    selectedCLCBalanceEntries.push(rowId);

                    $("#noCLCBalanceEntriesToPromote").prop("checked", false);
                    $("#noCLCBalanceEntriesToPromote").attr("disabled", "disabled");

                }
                else
                {

                    const index = selectedCLCBalanceEntries.indexOf(rowId);
                    if (index > -1) {
                        selectedCLCBalanceEntries.splice(index, 1);
                    }

                    if (selectedCLCBalanceEntries.length == 0) {
                        $("#noCLCBalanceEntriesToPromote").removeAttr("disabled");
                    }

                };

            }

            return true; // allow selection
        },
        onSelectRow: function (rowId, status, e) {

            // Only process left mouse-click
            if (!e || e.which === 1) {

                // NOP

            }       // left-click
        }       // onSelectRow event

    });      // $("#clcBalanceEntryList").jqGrid()

});     // // document.ready()
