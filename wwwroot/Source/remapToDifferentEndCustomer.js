var masterCustomerGridUrl;
var endCustomerGridUrl;
var salesOrderEndCustomerGridUrl;
var customerAssignmentUrl;

const SIMILAR_NAMED_CUSTOMER_SEARCH_NUMBER_OF_CHARACTERS = 6;
const ERROR_MESSAGE_AUTO_DISMISS_MILLISECONDS = 5000;
const DISPLAY_FLAG_LATEST_BOOKINGS_DATE = 1;


$(function () {

    $("#errorMessageBlock").hide();
    $("#endCustomerSearchMask").val("");
    $("#endCustomerSearchMask").attr("disabled", "disabled");
    $("#searchForEndCustomer").attr("disabled", "disabled");
    $("#remapToEndCustomer").attr("disabled", "disabled");
    $("#remapToSalesOrderCustomer").attr("disabled", "disabled");

    $("#masterCustomerSearchMask").keypress(function (event) {
        if (event.keyCode === 13) {
            $("#searchForMasterCustomer").click();
        }
    }); 

    $("#endCustomerSearchMask").keypress(function (event) {
        if (event.keyCode === 13) {
            $("#searchForEndCustomer").click();
        }
    }); 

    $("#assignmentTabs").tabs();

    $("#searchForMasterCustomer").click(function () {

        if ($("#masterCustomerSearchMask").val() === "") {

            $("#errorMessageText").html("Master Customer (source) search text must be specified");
            $("#errorMessageBlock").show();

        } else {

            $("#errorMessageBlock").hide();

            $("#selectedMasterCustomerEntryId").val("");

            // PROGRAMMER'S NOTE:  For Source Master Customer, pass in the value of 0 for the current Entry ID - it is used to filter out a search for Master Customers that is to exclude
            //                     a previously-selected entry.  In this case, all are valid.
            masterCustomerGridUrl = $("#baseWebServiceUrl").val() + "/ws/masterCustomer/0/searchNameMask/" + btoa($("#masterCustomerSearchMask").val().toUpperCase());

            if ($("#onlyFromBeginningOfNameMasterCustomer").is(":checked")) {
                // from beginning of name
                masterCustomerGridUrl += "/1/";
            } else {
                // anywhere in name
                masterCustomerGridUrl += "/0/";
            }

            $("#masterCustomerList").jqGrid('setGridParam', { url: masterCustomerGridUrl }).trigger("reloadGrid");

        }

    });     // $("#searchForMasterCustomer").click(function ()

    $("#masterCustomerList").jqGrid({
        url: masterCustomerGridUrl,
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
        height: 150,
        rowNum: 5000,
        colNames: ['Entry ID', 'Corporate Customer HQ Name', 'External System Customer Name', 'Sales Level 1', 'Sales Level 2', 'Sales Level 3', 'Sales Level 4'],
        colModel: [
            {
                name: 'masterCustomerEntryId',
                index: 'masterCustomerEntryId',
                width: 50,
                align: "left",
                key: true,
                hidden: true,
                editable: false
            },
            {
                name: 'endCustomerHeadquartersName',
                index: 'endCustomerHeadquartersName',
                align: "left",
                width: 250,
                editable: false
            },
            {
                name: 'externalSystemCustomerName',
                index: 'externalSystemCustomerName',
                align: "left",
                width: 250,
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
            },
            {
                name: 'salesLevel4',
                index: 'salesLevel4',
                align: "left",
                width: 200,
                editable: false
            }
        ],
        beforeRequest: function () {

            $("#endCustomerSearchMask").val("");
            $("#endCustomerSearchMask").attr("disabled", "disabled");
            $("#searchForEndCustomer").attr("disabled","disabled");

            endCustomerGridUrl = $("#baseWebServiceUrl").val() + "/ws/customer/searchNameMask/" + encodeURI("~~~~~") + "/0/";
            $("#endCustomerList").jqGrid('setGridParam', { url: endCustomerGridUrl }).trigger("reloadGrid");

            salesOrderEndCustomerGridUrl = $("#baseWebServiceUrl").val() + "/ws/masterCustomerEntry/endCustomerMappingForSalesOrders/-1/";
            $("#salesOrderEndCustomerList").jqGrid('setGridParam', { url: salesOrderEndCustomerGridUrl }).trigger("reloadGrid");

        },
        loadComplete: function () {

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

        },       // loadcomplete()
        onSelectRow: function (rowId, status, e) {

            // Only process left mouse-click
            if (!e || e.which === 1) {

                var rowData = $(this).getRowData(rowId);

                $("#selectedMasterCustomerEntryId").val(rowId);
                $("#endCustomerSearchMask").removeAttr("disabled");
                $("#searchForEndCustomer").removeAttr("disabled");

                $("#endCustomerSearchMask").val(rowData.endCustomerHeadquartersName.substring(0, SIMILAR_NAMED_CUSTOMER_SEARCH_NUMBER_OF_CHARACTERS));

                salesOrderEndCustomerGridUrl = $("#baseWebServiceUrl").val() + "/ws/masterCustomerEntry/endCustomerMappingForSalesOrders/" +
                                               $("#selectedMasterCustomerEntryId").val() + "/";
                $("#salesOrderEndCustomerList").jqGrid('setGridParam', { url: salesOrderEndCustomerGridUrl }).trigger("reloadGrid");

            }       // left-click
        }       // onSelectRow event

    });      // $("#masterCustomerList").jqGrid()

    $("#endCustomerList").jqGrid({
        url: endCustomerGridUrl,
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
        height: 200,
        rowNum: 40,
        colNames: ['End Customer ID', 'End Customer Headquarters Name', 'Sales Level 1', 'Sales Level 2', 'Sales Level 3', 'Sales Level 4', 'Sales Level 5', 'Sales Level 6', 'Latest Booking', 'Display Flags'],
        colModel: [
            {
                name: 'customerId',
                index: 'customerId',
                width: 100,
                align: "center",
                key: true,
                editable: false
            },
            {
                name: 'customerName',
                index: 'customerName',
                width: 300,
                editable: false
            },
            {
                name: 'salesLevel1',
                index: 'salesLevel1',
                width: 100,
                editable: false
            },
            {
                name: 'salesLevel2',
                index: 'salesLevel2',
                width: 200,
                editable: false
            },
            {
                name: 'salesLevel3',
                index: 'salesLevel3',
                width: 200,
                editable: false
            },
            {
                name: 'salesLevel4',
                index: 'salesLevel4',
                width: 200,
                editable: false
            },
            {
                name: 'salesLevel5',
                index: 'salesLevel5',
                width: 200,
                editable: false
            },
            {
                name: 'salesLevel6',
                index: 'salesLevel6',
                width: 250,
                editable: false
            },
            {
                name: 'latestBookingsDate',
                index: 'latestBookingsDate',
                formatter: 'date', formatoptions: { srcformat: 'Y-M-d', newformat: 'Y-m-d' },
                width: 100,
                editable: false
            },
            {
                name: 'displayFlags',
                index: 'displayFlags',
                width: 10,
                hidden: true,
                editable: false
            }
        ],
        beforeRequest: function () {

            $("#selectedEndCustomerId").val("");
            $("#remapToSalesOrderCustomer").attr("disabled", "disabled");

        },
        loadComplete: function () {

            var listRows = $(this).getDataIDs();

            for (i = 0; i < listRows.length; i++) {

                var rowData = $(this).getRowData(listRows[i]);

                if (parseInt(rowData.displayFlags, 10) === DISPLAY_FLAG_LATEST_BOOKINGS_DATE) {
                    $("#" + rowData.customerId, $(this)).removeClass("ui-widget-content").addClass("mostRecentBookingsDate");
                }
            }

        },       // loadcomplete()
        onSelectRow: function (rowId, status, e) {

            // Only process left mouse-click
            if (!e || e.which === 1) {

                $("#remapToEndCustomer").removeAttr("disabled");
                $("#selectedEndCustomerId").val(rowId);

            }       // left-click
        }       // onSelectRow event

    });      // $("#endCustomerList").jqGrid()

    $("#searchForEndCustomer").click(function () {

        if ($("#endCustomerSearchMask").val() === "") {

            $("#errorMessageText").html("Search text must be specified");
            $("#errorMessageBlock").show();

        } else
        {

            $("#errorMessageBlock").hide();

            endCustomerGridUrl = $("#baseWebServiceUrl").val() + "/ws/customer/searchNameMask/" + encodeURI($("#endCustomerSearchMask").val().toUpperCase());

            if ($("#onlyFromBeginningOfNameEndCustomer").is(":checked")) {
                // from beginning of name
                endCustomerGridUrl += "/0/";
            } else
            {
                // anywhere in name
                endCustomerGridUrl += "/1/";
            }

            $("#endCustomerList").jqGrid('setGridParam', { url: endCustomerGridUrl }).trigger("reloadGrid");

        }

    });     // $("#searchForEndCustomer").click(function ()

    $("#remapToEndCustomer").click(function () {

        var customerAssignmentUrl = $("#baseWebServiceUrl").val() + "/ws/masterCustomer/" + $("#selectedMasterCustomerEntryId").val() + "/assignNameAndSalesLevels/" +
                                    $("#selectedEndCustomerId").val() + "/";

        $.ajax({
            type: "PUT",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            url: customerAssignmentUrl,
            success: function (employee) {

                masterCustomerGridUrl = $("#baseWebServiceUrl").val() + "/ws/masterCustomer/0/searchNameMask/" + btoa("~~~~~") + "/";
                $("#masterCustomerList").jqGrid('setGridParam', { url: masterCustomerGridUrl }).trigger("reloadGrid");

                $("#masterCustomerSearchMask").val("");
                $("#endCustomerSearchMask").val("");
                $("#remapToEndCustomer").attr("disabled", "disabled");

                $("#masterCustomerSearchMask").focus();

            },
            error: function (jqXHR, textStatus, errorThrown) {

                // PROGRAMMER'S NOTE:  Need to handle case of error encountered during AJAX call

            }
        });      // ajax()

    });     // $("#remapToEndCustomer").click(function ()

    $("#salesOrderEndCustomerList").jqGrid({
        url: salesOrderEndCustomerGridUrl,
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
        height: 200,
        rowNum: 40,
        colNames: ['Booking Entry ID', 'End Customer ID', 'End Customer Headquarters Name', 'Sales Level 1', 'Sales Level 2', 'Sales Level 3', 'Sales Level 4', 'Adjustment Type Code'],
        colModel: [
            {
                name: 'bookingEntryId',
                index: 'bookingEntryId',
                width: 50,
                key: true,
                hidden: true,
                editable: false
            },
            {
                name: 'endCustomerId',
                index: 'endCustomerId',
                width: 100,
                align: "right",
                editable: false
            },
            {
                name: 'endCustomerHeadquartersName',
                index: 'endCustomerHeadquartersName',
                width: 300,
                editable: false
            },
            {
                name: 'salesLevel1',
                index: 'salesLevel1',
                width: 100,
                editable: false
            },
            {
                name: 'salesLevel2',
                index: 'salesLevel2',
                width: 200,
                editable: false
            },
            {
                name: 'salesLevel3',
                index: 'salesLevel3',
                width: 200,
                editable: false
            },
            {
                name: 'salesLevel4',
                index: 'salesLevel4',
                width: 200,
                editable: false
            },
            {
                name: 'bookingsAdjustmentCodeType',
                index: 'bookingsAdjustmentCodeType',
                width: 100,
                editable: false
            }
        ],
        beforeRequest: function () {

            $("#remapToSalesOrderCustomer").attr("disabled", "disabled");
            $("#selectedBookingEntryId").val("");

        },
        loadComplete: function () {

            //var listRows = $(this).getDataIDs();


        },       // loadcomplete()
        onSelectRow: function (rowId, status, e) {

            // Only process left mouse-click
            if (!e || e.which === 1) {

                $("#selectedBookingEntryId").val(rowId);
                $("#remapToSalesOrderCustomer").removeAttr("disabled");

            }       // left-click
        }       // onSelectRow event

    });      // $("#salesOrderEndCustomerList").jqGrid()

    $("#remapToSalesOrderCustomer").click(function () {

        var customerAssignmentUrl = $("#baseWebServiceUrl").val() + "/ws/masterCustomer/" + $("#selectedMasterCustomerEntryId").val() + "/assignNameAndSalesLevelsFromBookingEntryId/" +
                                    $("#selectedBookingEntryId").val() + "/";

        $.ajax({
            type: "PUT",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            url: customerAssignmentUrl,
            success: function (employee) {

                masterCustomerGridUrl = $("#baseWebServiceUrl").val() + "/ws/masterCustomer/0/searchNameMask/" + btoa("~~~~~") + "/";
                $("#masterCustomerList").jqGrid('setGridParam', { url: masterCustomerGridUrl }).trigger("reloadGrid");

                salesOrderEndCustomerGridUrl = $("#baseWebServiceUrl").val() + "/ws/masterCustomerEntry/endCustomerMappingForSalesOrders/-1/";
                $("#salesOrderEndCustomerList").jqGrid('setGridParam', { url: salesOrderEndCustomerGridUrl }).trigger("reloadGrid");

                $("#masterCustomerSearchMask").val("");
                $("#endCustomerSearchMask").val("");
                $("#remapToEndCustomer").attr("disabled", "disabled");
                $("#remapToSalesOrderCustomer").attr("disabled", "disabled");

                $("#masterCustomerSearchMask").focus();

            },
            error: function (jqXHR, textStatus, errorThrown) {

                // PROGRAMMER'S NOTE:  Need to handle case of error encountered during AJAX call

            }
        });      // ajax()

    });     // $("#remapToSalesOrderCustomer").click(function ()

});     // // document.ready()
