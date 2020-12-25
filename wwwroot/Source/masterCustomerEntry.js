var unprocessedGridUrl;
var masterCustomerGridUrl;
var salesOrderEndCustomerGridUrl;
var customerGridUrl;
var workProcessingQueueListUrl;
var affiliatedCustomerListGridUrl;
var customerSSOTSalesLevelInformationUrl;
var processFromCustomerSalesLevelParameterSetUrl;

var SIMILAR_NAMED_CUSTOMER_SEARCH_NUMBER_OF_CHARACTERS = 6;
const ERROR_MESSAGE_AUTO_DISMISS_MILLISECONDS = 5000;

var MASTER_CUSTOMER_ENTRY_PROCESSING_STATUS_READY_FOR_PROCESSING = "R";
var MASTER_CUSTOMER_ENTRY_PROCESSING_STATUS_PROCESSED = "P";
var MASTER_CUSTOMER_ENTRY_PROCESSING_STATUS_MOVED_TO_SUBORDINATE_TABLE = "S";
var MASTER_CUSTOMER_ENTRY_PROCESSING_STATUS_MOVED_TO_HOLD_QUEUE = "H";

$(function () {

    $.jstree.defaults.core.check_callback = true;

    $("#errorMessageBlock").hide();
    $("#searchForEndCustomer").attr("disabled", "disabled");
    $("#assignNameAndSalesLevels").attr("disabled", "disabled");
    $("#mapToEndCustomerWithCRSalesLevels").attr("disabled", "disabled");
    $("#mapToSalesOrderCustomer").attr("disabled", "disabled");
    $("#mapToSOCustomerWithCRSalesLevels").attr("disabled", "disabled");

    $("#workProcessingQueue").find("option").remove();

    workProcessingQueueListUrl = $("#baseWebServiceUrl").val() + "/ws/workProcessingQueues/";

    affiliatedCustomerListGridUrl = $("#baseWebServiceUrl").val() + "/ws/customer/list/~/~/~/~/~/~/";

    $("#assignmentTabs").tabs({
        heightStyle: "content"
    });

    $("#affiliatedCustomerList").jqGrid({
        url: unprocessedGridUrl,
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
        colNames: ['EndCustomerID', 'Corporate Customer HQ Name', 'Matched At Sales Level'],
        colModel: [
            {
                name: 'customerId',
                index: 'customerId',
                width: 50,
                align: "left",
                key: true,
                hidden: true,
                editable: false
            },
            {
                name: 'customerName',
                index: 'customerName',
                align: "left",
                width: 400,
                editable: false
            },
            {
                name: 'matchedAtSalesLevel',
                index: 'matchedAtSalesLevel',
                align: "left",
                width: 100,
                editable: false
            }
        ],
        beforeRequest: function () {

            $("#selectedAffiliatedEndCustomerID").val(0);
            $("#okOnAccountManagerModal").attr("disabled", "disabled");

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

                $("#selectedEndCustomerId").val(rowId);
                $("#okOnAccountManagerModal").removeAttr("disabled");

            }       // left-click
        }       // onSelectRow event

    });      // $("#unprocessedList").jqGrid()

    $("#okOnAccountManagerModal").click(function () {

        $("#accountManagerModal").modal("hide");
        $("#assignNameAndSalesLevels").click();
        $("#mapToEndCustomerWithCRSalesLevels").click();

    });

    $("#accountManagerInformation").click(function () {

        var getResourceDataUrl = $("#baseWebServiceUrl").val() + "/ws/resource/" + $("#accountManagerCEC_ID").html() + "/";

        $.ajax({
            type: "GET",
            dataType: "json",
            url: getResourceDataUrl,
            contentType: "application/json",
            success: function (employee) {

                $("#employeeName").html(employee.name);
                $("#employeeCEC_ID").html(employee.externalId);
                $("#departmentCode").html(employee.departmentCode);
                $("#departmentName").html(employee.departmentName);

                $("#supervisorTree").jstree("destroy").empty();
                $("#supervisorTree").jstree({
                    'core': {
                        'multiple': false,
                        'animation': 0,
                        'data': {
                            'type': "get",
                            'dataType': "JSON",
                            'url': $("#baseWebServiceUrl").val() + "/ws/resource/" + employee.internalId + "/supervisorNodes/",
                            'data': function (node) {
                                return { 'id': node.id };
                            }
                        }
                    }
                });     // $("#supervisorTree").jstree

                affiliatedCustomerListGridUrl = $("#baseWebServiceUrl").val() + "/ws/customer/list/" +
                                                encodeURI(employee.salesLevel1) + "/" + encodeURI(employee.salesLevel2) + "/" +
                                                encodeURI(employee.salesLevel3) + "/" + encodeURI(employee.salesLevel4) + "/" +
                                                encodeURI(employee.salesLevel5) + "/" + encodeURI(employee.salesLevel6) + "/";
                $("#affiliatedCustomerList").jqGrid('setGridParam', { url: affiliatedCustomerListGridUrl }).trigger("reloadGrid");
    
                $("#accountManagerModal").modal("show");

            },
            error: function (jqXHR, textStatus, errorThrown) {

                // PROGRAMMER'S NOTE:  Need to handle case of error encountered during AJAX call

            }
        });      // ajax()

    });

    // Populate Work Processing Queue list dropdown
    $.ajax({
        type: "GET",
        url: workProcessingQueueListUrl,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (returnValue) {

            var workProcessingQueue = $("#workProcessingQueue");

            returnValue.forEach((element) => {

                var option = $("<option />");
                option.val(element.workProcessingQueueId);
                option.html(element.referenceName);
                workProcessingQueue.append(option);

            });

            // Select first item in list and trigger initial populate
            $("#workProcessingQueue option:eq(0)").prop('selected', true);

            // PROGRAMMER'S NOTE:  If triggering immediately, would routinely fail the refresh call to the web service.
            setTimeout(function () {
                $("#workProcessingQueue").trigger("change");
            }, 100);

        },
        error: function (jqXHR, textStatus, errorThrown) {

            $("#errorMessageBlock").show();
            $("#errorMessageText").html("An error occured during web service call to populate Reason Codes");
            setTimeout(function () {

                $("#errorMessageText").html("");
                $("#errorMessageBlock").hide();

            }, ERROR_MESSAGE_AUTO_DISMISS_MILLISECONDS);

        }
    });        // .ajax()

    $("#markProcessed").attr("disabled", "disabled");
    $("#moveToHoldQueue").attr("disabled", "disabled");
    $("#subordinateToMasterCustomer").attr("disabled", "disabled");
    $("#masterCustomerSearchMask").attr("disabled", "disabled");
    $("assignToBDM").attr("disabled", "disabled");

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

    $("#unprocessedList").jqGrid({
        url: unprocessedGridUrl,
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
        colNames: ['Entry ID', 'Customer Name in L@C System', 'End Customer ID', 'Corporate Customer HQ Name', 'Sales Level 1', 'Sales Level 2', 'Sales Level 3','Account Manager CEC ID'],
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
                name: 'externalSystemCustomerName',
                index: 'externalSystemCustomerName',
                width: 250,
                align: "left",
                editable: false
            },
            {
                name: 'endCustomerId',
                index: 'endCustomerId',
                align: "right",
                width: 70,
                hidden: true,
                editable: false
            },
            {
                name: 'endCustomerHeadquartersName',
                index: 'endCustomerHeadquartersName',
                align: "left",
                width: 225,
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
                width: 125,
                editable: false
            },
            {
                name: 'salesLevel3',
                index: 'salesLevel3',
                align: "left",
                width: 125,
                editable: false
            },
            {
                name: 'accountManagerCEC_ID',
                index: 'accountManagerCEC_ID',
                align: "left",
                width: 50,
                hidden: true,
                editable: false
            }
            
        ],
        beforeRequest: function () {

            $("#markProcessed").attr("disabled", "disabled");
            $("#moveToHoldQueue").attr("disabled", "disabled");
            $("#assignToBDM").attr("disabled", "disabled");
            $("#subordinateToMasterCustomer").attr("disabled", "disabled");
            $("#masterCustomerSearchMask").attr("disabled", "disabled");
            $("#masterCustomerSearchMask").val("");
            $("#searchForMasterCustomer").attr("disabled", "disabled");

            $("#mapToSalesOrderCustomer").attr("disabled", "disabled");
            $("#mapToSOCustomerWithCRSalesLevels").attr("disabled", "disabled");

            $("#accountManagerCEC_ID").html("");
            $("#accountManagerInformation").attr("disabled", "disabled");

            // RJB 2020-08-18
            //masterCustomerGridUrl = $("#baseWebServiceUrl").val() + "/ws/masterCustomer/0/searchByNameMask?searchMask=" + btoa("ZZZZZZZZZZZZZ");       // invalid search value to blank grid
            masterCustomerGridUrl = $("#baseWebServiceUrl").val() + "/ws/masterCustomer/0/searchNameMask/" + btoa("ZZZZZZZZZZZZZ") + "/0/";       // invalid search value to blank grid

            $("#masterCustomerList").jqGrid('setGridParam', { url: masterCustomerGridUrl }).trigger("reloadGrid");
            $("#endCustomerSearchMask").val("");
            $("#searchForEndCustomer").attr("disabled", "disabled");

            customerGridUrl = $("#baseWebServiceUrl").val() + "/ws/customer/searchNameMask/" + encodeURI("~~~~~~~~") + "/1/";      // invalid search value to blank grid
            $("#endCustomerList").jqGrid('setGridParam', { url: customerGridUrl }).trigger("reloadGrid");

            salesOrderEndCustomerGridUrl = $("#baseWebServiceUrl").val() + "/ws/masterCustomerEntry/endCustomerMappingForSalesOrders/-1/";
            $("#salesOrderEndCustomerList").jqGrid('setGridParam', { url: salesOrderEndCustomerGridUrl }).trigger("reloadGrid");

            $("#cssotAPI_ECSalesLevel1").val("");
            $("#cssotAPI_ECSalesLevel2").val("");
            $("#cssotAPI_ECSalesLevel3").val("");
            $("#cssotAPI_ECSalesLevel4").val("");

            $("#cssotAPI_SOSalesLevel1").val("");
            $("#cssotAPI_SOSalesLevel2").val("");
            $("#cssotAPI_SOSalesLevel3").val("");
            $("#cssotAPI_SOSalesLevel4").val("");

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

            // Update item status counts
            itemStatusCountUrl = $("#baseWebServiceUrl").val() + "/ws/workProcessingQueue/" + $("#workProcessingQueue").val() + "/statusCountsForItems/";

            $.ajax({
                type: "GET",
                url: itemStatusCountUrl,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (returnValue) {

                    $("#readyToProcess").html(returnValue.readyToProcess);
                    $("#processed").html(returnValue.processed);
                    $("#subordinated").html(returnValue.subordinated);
                    $("#placedOnHold").html(returnValue.placedOnHold);

                },
                error: function (jqXHR, textStatus, errorThrown) {
                    // NOP
                }
            });        // .ajax()


        },       // loadcomplete()
        onSelectRow: function (rowId, status, e) {

            // Only process left mouse-click
            if (!e || e.which === 1) {

                var rowData = $(this).getRowData(rowId);

                $("#selectedUnprocessedCustomerEntryId").val(rowId);

                $("#accountManagerCEC_ID").html(rowData.accountManagerCEC_ID);

                if ($("#accountManagerCEC_ID").html() !== "") {
                    $("#accountManagerInformation").removeAttr("disabled");
                }

                $("#masterCustomerSearchMask").removeAttr("disabled");
                $("#searchForMasterCustomer").removeAttr("disabled");

                $("#endCustomerSearchMask").val(rowData.externalSystemCustomerName);
                $("#searchForEndCustomer").removeAttr("disabled");

                $("#markProcessed").removeAttr("disabled");
                $("#moveToHoldQueue").removeAttr("disabled");
                $("#assignToBDM").removeAttr("disabled");

                $("#masterCustomerSearchMask").val(rowData.externalSystemCustomerName.substring(0, SIMILAR_NAMED_CUSTOMER_SEARCH_NUMBER_OF_CHARACTERS));

                masterCustomerGridUrl = $("#baseWebServiceUrl").val() + "/ws/masterCustomer/" + rowId + "/searchNameMask/" +
                                        btoa(rowData.externalSystemCustomerName.substring(0, SIMILAR_NAMED_CUSTOMER_SEARCH_NUMBER_OF_CHARACTERS)) + "/1/";
                $("#masterCustomerList").jqGrid('setGridParam', { url: masterCustomerGridUrl }).trigger("reloadGrid");

                customerGridUrl = $("#baseWebServiceUrl").val() + "/ws/customer/searchNameMask/" + encodeURI(rowData.externalSystemCustomerName) + "/1/";
                $("#endCustomerList").jqGrid('setGridParam', { url: customerGridUrl }).trigger("reloadGrid");

                salesOrderEndCustomerGridUrl = $("#baseWebServiceUrl").val() + "/ws/masterCustomerEntry/endCustomerMappingForSalesOrders/" + rowId + "/";
                $("#salesOrderEndCustomerList").jqGrid('setGridParam', { url: salesOrderEndCustomerGridUrl }).trigger("reloadGrid");

            }       // left-click
        }       // onSelectRow event

    });      // $("#unprocessedList").jqGrid()

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
        colNames: ['Entry ID', 'Corporate Customer HQ Name', 'Sales Level 1', 'Sales Level 2', 'Sales Level 3'],
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
                width: 125,
                editable: false
            },
            {
                name: 'salesLevel3',
                index: 'salesLevel3',
                align: "left",
                width: 125,
                editable: false
            }
        ],
        beforeRequest: function () {

            $("#subordinateToMasterCustomer").attr("disabled","disabled");

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
                $("#subordinateToMasterCustomer").removeAttr("disabled");

            }       // left-click
        }       // onSelectRow event

    });      // $("#masterCustomerList").jqGrid()

    $("#searchForMasterCustomer").click(function () {

        // RJB 2020-08-18
        //masterCustomerGridUrl = $("#baseWebServiceUrl").val() + "/ws/masterCustomer/" + $("#selectedUnprocessedCustomerEntryId").val() + "/searchByNameMask?searchMask=" +
        //                        btoa($("#masterCustomerSearchMask").val());
        masterCustomerGridUrl = $("#baseWebServiceUrl").val() + "/ws/masterCustomer/" + $("#selectedUnprocessedCustomerEntryId").val() + "/searchNameMask/" +
                                btoa($("#masterCustomerSearchMask").val());

        // anywhere in name
        masterCustomerGridUrl += "/0/";

        $("#masterCustomerList").jqGrid('setGridParam', { url: masterCustomerGridUrl }).trigger("reloadGrid");

    });     // $("#searchForMasterCustomer").click(function ()

    $("#subordinateToMasterCustomer").click(function () {

        var markSubordinateUrl = $("#baseWebServiceUrl").val() + "/ws/masterCustomer/" + $("#selectedUnprocessedCustomerEntryId").val() + "/subordinateToMasterCustomer/" + 
                                 $("#selectedMasterCustomerEntryId").val() + "/";

        $.ajax({
            type: "PUT",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            url: markSubordinateUrl,
            success: function (result) {

                $("#unprocessedList").jqGrid().trigger("reloadGrid");
                $("#unprocessedList").jqGrid("setSelection", parseInt($("#selectedUnprocessedCustomerEntryId").val()));

            },
            error: function (jqXHR, textStatus, errorThrown) {

                // PROGRAMMER'S NOTE:  Need to handle case of error encountered during AJAX call

            }
        });      // ajax()

    });     // $("#markSubordinate").click(function ()

    $("#markProcessed").click(function () {

        var unprocessedListGrid = $("#unprocessedList");
        var selectedRowId = unprocessedListGrid.jqGrid("getGridParam", "selrow");

        var rowData = $("#unprocessedList").getRowData(selectedRowId);

        if (rowData.endCustomerId == "0") {

            $("#errorMessageText").html("An End Customer and Sales Levels must be assigned to Master Customer Entry");
            $("#errorMessageBlock").show();
            setTimeout(function () {

                $("#errorMessageText").html("");
                $("#errorMessageBlock").hide();

            }, ERROR_MESSAGE_AUTO_DISMISS_MILLISECONDS);

        } else {

            // Any further validations

            var markProcessedUrl = $("#baseWebServiceUrl").val() + "/ws/masterCustomer/" + $("#selectedUnprocessedCustomerEntryId").val() + "/setProcessingStatus/" +
                                    MASTER_CUSTOMER_ENTRY_PROCESSING_STATUS_PROCESSED + "/";

            $.ajax({
                type: "PUT",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                url: markProcessedUrl,
                success: function (result) {

                    $("#subordinateCustomers").find("option").remove();
                    $("#unprocessedList").jqGrid().trigger("reloadGrid");

                },
                error: function (jqXHR, textStatus, errorThrown) {

                    // PROGRAMMER'S NOTE:  Need to handle case of error encountered during AJAX call

                }
            });      // ajax()

        }       // (unprocessedListGrid.jqGrid("getCell", selectedRowId, "endCustomerId") == "")

    });     // $("#markProcessed").click(function ()

    $("#moveToHoldQueue").click(function () {

        var unprocessedListGrid = $("#unprocessedList");
        var selectedRowId = unprocessedListGrid.jqGrid("getGridParam", "selrow");

        var rowData = $("#unprocessedList").getRowData(selectedRowId);

        // Any further validations

        var markProcessedUrl = $("#baseWebServiceUrl").val() + "/ws/masterCustomer/" + $("#selectedUnprocessedCustomerEntryId").val() + "/setProcessingStatus/" +
                               MASTER_CUSTOMER_ENTRY_PROCESSING_STATUS_MOVED_TO_HOLD_QUEUE + "/";

        $.ajax({
            type: "PUT",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            url: markProcessedUrl,
            success: function (result) {

                $("#subordinateCustomers").find("option").remove();
                $("#unprocessedList").jqGrid().trigger("reloadGrid");

            },
            error: function (jqXHR, textStatus, errorThrown) {

                // PROGRAMMER'S NOTE:  Need to handle case of error encountered during AJAX call

            }
        });      // ajax()


    });     // $("#moveToHoldQueue").click(function ()

    /*
    $("#assignToBDM").click(function () {

        var assignToBDMUrl = $("#baseWebServiceUrl").val() + "/ws/masterCustomer/" + $("#selectedUnprocessedCustomerEntryId").val() + "/assignToBDM/" +
                             $("#businessDevelopmentManagers").val() + "/";

        $.ajax({
            type: "PUT",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            url: assignToBDMUrl,
            success: function (result) {

                $("#unprocessedList").jqGrid().trigger("reloadGrid");
                $("#unprocessedList").jqGrid("setSelection", parseInt($("#selectedUnprocessedCustomerEntryId").val()));

            },
            error: function (jqXHR, textStatus, errorThrown) {

                // PROGRAMMER'S NOTE:  Need to handle case of error encountered during AJAX call

            }
        });      // ajax()

    });     // $("#assignToBDM").click(function ()
    */

    $("#endCustomerList").jqGrid({
        url: customerGridUrl,
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
        height: 150,
        rowNum: 5000,
        colNames: ['Customer No.', 'End Customer CR Party ID', 'Customer Name', 'Sales Level 1', 'Sales Level 2', 'Sales Level 3', 'Sales Level 4',
                   'Sales Level 5', 'Sales Level 6', 'Latest Booking'],
        colModel: [
            {
                name: 'customerId',
                index: 'customerId',
                width: 50,
                align: "center",
                key: true,
                hidden: true,
                editable: false
            },
            {
                name: 'endCustomerCRPartyId',
                index: 'endCustomerCRPartyId',
                width: 100,
                hidden: true,
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
                width: 150,
                editable: false
            },
            {
                name: 'salesLevel3',
                index: 'salesLevel3',
                width: 150,
                editable: false
            },
            {
                name: 'salesLevel4',
                index: 'salesLevel4',
                width: 150,
                editable: false
            },
            {
                name: 'salesLevel5',
                index: 'salesLevel5',
                width: 150,
                hidden: true,
                editable: false
            },
            {
                name: 'salesLevel6',
                index: 'salesLevel6',
                width: 150,
                hidden: true,
                editable: false
            },
            {
                name: 'latestBookingsDate',
                index: 'latestBookingsDate',
                formatter: 'date', formatoptions: { srcformat: 'Y-M-d', newformat: 'Y-m-d' },
                width: 100,
                editable: false
            }
        ],
        beforeRequest: function () {

            $("#cssotAPI_ECSalesLevel1").val("");
            $("#cssotAPI_ECSalesLevel2").val("");
            $("#cssotAPI_ECSalesLevel3").val("");
            $("#cssotAPI_ECSalesLevel4").val("");

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

                $("#selectedEndCustomerId").val(rowId);
                $("#assignNameAndSalesLevels").removeAttr("disabled");
                $("#mapToEndCustomerWithCRSalesLevels").removeAttr("disabled");

                var rowData = $(this).getRowData(rowId);

                customerSSOTSalesLevelInformationUrl = $("#baseWebServiceUrl").val() + "/ws-cssot/customerSSOT_APICustomer/" + rowData.endCustomerCRPartyId +
                                                       "/territory/";
                $.ajax({
                    type: "GET",
                    dataType: "json",
                    url: customerSSOTSalesLevelInformationUrl,
                    contentType: "application/json",
                    success: function (salesLevelInformation) {

                        $("#cssotAPI_ECSalesLevel1").val(salesLevelInformation.salesLevel1);
                        $("#cssotAPI_ECSalesLevel2").val(salesLevelInformation.salesLevel2);
                        $("#cssotAPI_ECSalesLevel3").val(salesLevelInformation.salesLevel3);
                        $("#cssotAPI_ECSalesLevel4").val(salesLevelInformation.salesLevel4);

                        $("#mapToEndCustomerWithCRSalesLevels").removeAttr("disabled");

                    },
                    error: function (jqXHR, textStatus, errorThrown) {

                        // PROGRAMMER'S NOTE:  Need to handle case of error encountered during AJAX call

                    }
                });      // ajax()



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

            customerGridUrl = $("#baseWebServiceUrl").val() + "/ws/customer/searchNameMask/" + encodeURI($("#endCustomerSearchMask").val().toUpperCase());

            if ($("#endCustomerOnlyFromBeginningOfName").is(":checked")) {
                // from beginning of name
                customerGridUrl += "/1/";
            } else
            {
                // anywhere in name
                customerGridUrl += "/0/";
            }

            $("#endCustomerList").jqGrid('setGridParam', { url: customerGridUrl }).trigger("reloadGrid");

        }

    });     // $("#searchForEndCustomer").click(function ()

    $("#assignNameAndSalesLevels").click(function () {

        var customerAssignmentUrl = $("#baseWebServiceUrl").val() + "/ws/masterCustomer/" + $("#selectedUnprocessedCustomerEntryId").val() + "/assignNameAndSalesLevels/" +
                                    $("#selectedEndCustomerId").val() + "/";

        $.ajax({
            type: "PUT",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            url: customerAssignmentUrl,
            success: function (employee) {

                $("#unprocessedList").jqGrid().trigger("reloadGrid");
                $("#unprocessedList").jqGrid("setSelection", parseInt($("#selectedUnprocessedCustomerEntryId").val()));

            },
            error: function (jqXHR, textStatus, errorThrown) {

                // PROGRAMMER'S NOTE:  Need to handle case of error encountered during AJAX call

            }
        });      // ajax()

    });     // $("#assignNameAndSalesLevels").click(function ()

    $("#workProcessingQueue").change(function () {

        unprocessedGridUrl = $("#baseWebServiceUrl").val() + "/ws/masterCustomer/" + $(this).val()  + "/unprocessedList/";

        $("#unprocessedList").jqGrid('setGridParam', { url: unprocessedGridUrl }).trigger("reloadGrid");

    });

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
        colNames: ['Booking Entry ID', 'End Cust. ID', 'End Customer ID', 'End Customer Headquarters Name', 'Sales Level 1',
                   'Sales Level 2', 'Sales Level 3', 'Sales Level 4', 'Adj. Type Code'],
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
                name: 'endCustomerCRPartyId',
                index: 'endCustomerCRPartyId',
                width: 100,
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
                width: 250,
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
                width: 180,
                editable: false
            },
            {
                name: 'salesLevel3',
                index: 'salesLevel3',
                width: 180,
                editable: false
            },
            {
                name: 'salesLevel4',
                index: 'salesLevel4',
                width: 180,
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

            $("#mapToSalesOrderCustomer").attr("disabled", "disabled");
            $("#mapToSOCustomerWithCRSalesLevels").attr("disabled", "disabled");
            $("#selectedBookingEntryId").val("");

            $("#cssotAPI_SOSalesLevel1").val("");
            $("#cssotAPI_SOSalesLevel2").val("");
            $("#cssotAPI_SOSalesLevel3").val("");
            $("#cssotAPI_SOSalesLevel4").val("");

        },
        loadComplete: function () {

            //var listRows = $(this).getDataIDs();


        },       // loadcomplete()
        onSelectRow: function (rowId, status, e) {

            // Only process left mouse-click
            if (!e || e.which === 1) {

                var rowData = $(this).getRowData(rowId);

                $("#selectedBookingEntryId").val(rowId);
                $("#mapToSalesOrderCustomer").removeAttr("disabled");

                customerSSOTSalesLevelInformationUrl = $("#baseWebServiceUrl").val() + "/ws-cssot/customerSSOT_APICustomer/" + rowData.endCustomerCRPartyId +
                                                       "/territory/";
                $.ajax({
                    type: "GET",
                    dataType: "json",
                    url: customerSSOTSalesLevelInformationUrl,
                    contentType: "application/json",
                    success: function (salesLevelInformation) {

                        $("#cssotAPI_SOSalesLevel1").val(salesLevelInformation.salesLevel1);
                        $("#cssotAPI_SOSalesLevel2").val(salesLevelInformation.salesLevel2);
                        $("#cssotAPI_SOSalesLevel3").val(salesLevelInformation.salesLevel3);
                        $("#cssotAPI_SOSalesLevel4").val(salesLevelInformation.salesLevel4);

                        $("#mapToSOCustomerWithCRSalesLevels").removeAttr("disabled");

                    },
                    error: function (jqXHR, textStatus, errorThrown) {

                        // PROGRAMMER'S NOTE:  Need to handle case of error encountered during AJAX call

                    }
                });      // ajax()

            }       // left-click
        }       // onSelectRow event

    });      // $("#salesOrderEndCustomerList").jqGrid()


    $("#mapToSalesOrderCustomer").click(function () {

        var customerAssignmentUrl = $("#baseWebServiceUrl").val() + "/ws/masterCustomer/" + $("#selectedUnprocessedCustomerEntryId").val() + "/assignNameAndSalesLevelsFromBookingEntryId/" +
                                    $("#selectedBookingEntryId").val() + "/";

        $.ajax({
            type: "PUT",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            url: customerAssignmentUrl,
            success: function (employee) {

                salesOrderEndCustomerGridUrl = $("#baseWebServiceUrl").val() + "/ws/masterCustomerEntry/endCustomerMappingForSalesOrders/-1/";
                $("#salesOrderEndCustomerList").jqGrid('setGridParam', { url: salesOrderEndCustomerGridUrl }).trigger("reloadGrid");

                $("#mapToSalesOrderCustomer").attr("disabled", "disabled");
                $("#mapToSOCustomerWithCRSalesLevels").attr("disabled", "disabled");

                $("#unprocessedList").jqGrid().trigger("reloadGrid");

            },
            error: function (jqXHR, textStatus, errorThrown) {

                // PROGRAMMER'S NOTE:  Need to handle case of error encountered during AJAX call

            }
        });      // ajax()

    });     // $("#mapToSalesOrderCustomer").click()

    ///

    $("#mapToSOCustomerWithCRSalesLevels").click(function () {

        var salesOrderEndCustomerList = $("#salesOrderEndCustomerList");
        var selectedRowId = salesOrderEndCustomerList.jqGrid("getGridParam", "selrow");
        var rowData = $("#salesOrderEndCustomerList").getRowData(selectedRowId);

        var parameterSet = new Object();

        parameterSet.masterCustomerEntryId = parseInt($("#selectedUnprocessedCustomerEntryId").val());
        parameterSet.endCustomerId = parseInt(rowData.endCustomerId);
        parameterSet.endCustomerCRPartyId = parseInt(rowData.endCustomerCRPartyId);
        parameterSet.endCustomerHeadquartersName = rowData.endCustomerHeadquartersName;
        parameterSet.salesLevel1 = $("#cssotAPI_SOSalesLevel1").val();
        parameterSet.salesLevel2 = $("#cssotAPI_SOSalesLevel2").val();
        parameterSet.salesLevel3 = $("#cssotAPI_SOSalesLevel3").val();
        parameterSet.saleslevel4 = $("#cssotAPI_SOSalesLevel4").val();

        processFromCustomerSalesLevelParameterSetUrl = $("#baseWebServiceUrl").val() + "/ws/masterCustomer/AssignNameAndSalesLevelsFromParameterSet/";

        $.ajax({
            type: "PUT",
            url: processFromCustomerSalesLevelParameterSetUrl,
            traditional: true,
            data: JSON.stringify(parameterSet),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (returnValue) {

                salesOrderEndCustomerGridUrl = $("#baseWebServiceUrl").val() + "/ws/masterCustomerEntry/endCustomerMappingForSalesOrders/-1/";
                $("#salesOrderEndCustomerList").jqGrid('setGridParam', { url: salesOrderEndCustomerGridUrl }).trigger("reloadGrid");

                $("#mapToSalesOrderCustomer").attr("disabled", "disabled");
                $("#mapToSOCustomerWithCRSalesLevels").attr("disabled", "disabled");

                $("#unprocessedList").jqGrid().trigger("reloadGrid");

            },
            error: function (e) {

                // PROGRAMMER'S NOTE:  Need to handle case of error encountered during AJAX call

            }
        });     // .ajax()

    });         // $("#mapToSOCustomerWithCRSalesLevels").click()

    $("#mapToEndCustomerWithCRSalesLevels").click(function () {

        var endCustomerList = $("#endCustomerList");
        var selectedRowId = endCustomerList.jqGrid("getGridParam", "selrow");
        var rowData = $("#endCustomerList").getRowData(selectedRowId);

        var parameterSet = new Object();

        parameterSet.masterCustomerEntryId = parseInt($("#selectedUnprocessedCustomerEntryId").val());
        parameterSet.endCustomerId = parseInt(rowData.customerId);
        parameterSet.endCustomerCRPartyId = parseInt(rowData.endCustomerCRPartyId);
        parameterSet.endCustomerHeadquartersName = rowData.customerName;
        parameterSet.salesLevel1 = $("#cssotAPI_ECSalesLevel1").val();
        parameterSet.salesLevel2 = $("#cssotAPI_ECSalesLevel2").val();
        parameterSet.salesLevel3 = $("#cssotAPI_ECSalesLevel3").val();
        parameterSet.saleslevel4 = $("#cssotAPI_ECSalesLevel4").val();

        processFromCustomerSalesLevelParameterSetUrl = $("#baseWebServiceUrl").val() + "/ws/masterCustomer/AssignNameAndSalesLevelsFromParameterSet/";

        $.ajax({
            type: "PUT",
            url: processFromCustomerSalesLevelParameterSetUrl,
            traditional: true,
            data: JSON.stringify(parameterSet),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (returnValue) {

                salesOrderEndCustomerGridUrl = $("#baseWebServiceUrl").val() + "/ws/masterCustomerEntry/endCustomerMappingForSalesOrders/-1/";
                $("#salesOrderEndCustomerList").jqGrid('setGridParam', { url: salesOrderEndCustomerGridUrl }).trigger("reloadGrid");

                $("#mapToSalesOrderCustomer").attr("disabled", "disabled");
                $("#mapToSOCustomerWithCRSalesLevels").attr("disabled", "disabled");

                $("#unprocessedList").jqGrid().trigger("reloadGrid");

            },
            error: function (e) {

                // PROGRAMMER'S NOTE:  Need to handle case of error encountered during AJAX call

            }
        });     // .ajax()

    });         // $("#mapToEndCustomerWithCRSalesLevels").click()

});     // // document.ready()
