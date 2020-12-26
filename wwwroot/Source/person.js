var personSearchResultGridUrl;

const WILDCARD_ALL_TOKEN = "%";
const ERROR_MESSAGE_AUTO_DISMISS_MILLISECONDS = 5000;

$(function () {

    $("#errorMessageBlock").hide();
    $("#editPerson").attr("disabled", "disabled");
    $("#deletePerson").attr("disabled", "disabled");

    personSearchResultGridUrl = $("#baseWebServiceUrl").val() + "/ws/person/search/";

    $("#personSearchResultGrid").jqGrid({
        url: personSearchResultGridUrl,
        type: 'GET',
        ajaxGridOptions: {
            contentType: 'application/json; charset=utf-8',
            timeout: 60000,
            dataType: "json",
            type: "POST",
            cach: false
        },
        postData: {
            lastNameSearchMask: function () {
                return $("#lastNameSearchMask").val() + WILDCARD_ALL_TOKEN;
            },
            firstNameSearchMask: function () {
                return $("#firstNameSearchMask").val() + WILDCARD_ALL_TOKEN;
            },
            preferredFirstNameSearchMask: function () {
                return $("#preferredFirstNameSearchMask").val() + WILDCARD_ALL_TOKEN;
            },
        },
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
        height: 200,
        rowNum: 5000,
        colNames: ['Person ID', 'Last Name', 'First Name', 'Preferred First Name','Middle Name', 'Date of Birth'],
        colModel: [
            {
                name: 'personId',
                index: 'personId',
                width: 50,
                align: "left",
                key: true,
                hidden: true,
                editable: false
            },
            {
                name: 'lastName',
                index: 'lastName',
                align: "left",
                width: 200,
                editable: false
            },
            {
                name: 'firstName',
                index: 'firstName',
                align: "left",
                width: 200,
                editable: false
            },
            {
                name: 'preferredFirstName',
                index: 'preferedFirstName',
                align: "left",
                width: 200,
                editable: false
            },
            {
                name: 'middleName',
                index: 'middleName',
                align: "left",
                width: 200,
                editable: false
            },
            {
                name: 'dateOfBirth',
                index: 'dateOfBirth',
                formatter: 'date', formatoptions: { srcformat: 'Y-M-d', newformat: 'Y-m-d' },
                width: 100,
                editable: false
            }
        ],
        beforeRequest: function () {

            $("#editPerson").attr("disabled", "disabled");
            $("#deletePerson").attr("disabled", "disabled");

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

                $("#selectedPersonId").val(rowId);

                $("#editPerson").removeAttr("disabled");
                $("#deletePerson").removeAttr("disabled");

            }       // left-click
        }       // onSelectRow event

    });      // $("#personSearchResultGrid").jqGrid()


    $("#okOnAddEditPersonModal").click(function () {

        $("#addEditPersonModal").modal("hide");

    });



    //$("#masterCustomerSearchMask").keypress(function (event) {
    //    if (event.keyCode === 13) {
    //        $("#searchForMasterCustomer").click();
    //    }
    //}); 



    $("#searchForPerson").click(function () {

        $("#personSearchResultGrid").jqGrid('setGridParam', { url: personSearchResultGridUrl}).trigger("reloadGrid");

    });     // $("#searchForPerson").click(function ()

    //$("#mapToSOCustomerWithCRSalesLevels").click(function () {

    //    var salesOrderEndCustomerList = $("#salesOrderEndCustomerList");
    //    var selectedRowId = salesOrderEndCustomerList.jqGrid("getGridParam", "selrow");
    //    var rowData = $("#salesOrderEndCustomerList").getRowData(selectedRowId);

    //    var parameterSet = new Object();

    //    parameterSet.masterCustomerEntryId = parseInt($("#selectedUnprocessedCustomerEntryId").val());
    //    parameterSet.endCustomerId = parseInt(rowData.endCustomerId);
    //    parameterSet.endCustomerCRPartyId = parseInt(rowData.endCustomerCRPartyId);
    //    parameterSet.endCustomerHeadquartersName = rowData.endCustomerHeadquartersName;
    //    parameterSet.salesLevel1 = $("#cssotAPI_SOSalesLevel1").val();
    //    parameterSet.salesLevel2 = $("#cssotAPI_SOSalesLevel2").val();
    //    parameterSet.salesLevel3 = $("#cssotAPI_SOSalesLevel3").val();
    //    parameterSet.saleslevel4 = $("#cssotAPI_SOSalesLevel4").val();

    //    processFromCustomerSalesLevelParameterSetUrl = $("#baseWebServiceUrl").val() + "/ws/masterCustomer/AssignNameAndSalesLevelsFromParameterSet/";

    //    $.ajax({
    //        type: "PUT",
    //        url: processFromCustomerSalesLevelParameterSetUrl,
    //        traditional: true,
    //        data: JSON.stringify(parameterSet),
    //        contentType: "application/json; charset=utf-8",
    //        dataType: "json",
    //        success: function (returnValue) {

    //            salesOrderEndCustomerGridUrl = $("#baseWebServiceUrl").val() + "/ws/masterCustomerEntry/endCustomerMappingForSalesOrders/-1/";
    //            $("#salesOrderEndCustomerList").jqGrid('setGridParam', { url: salesOrderEndCustomerGridUrl }).trigger("reloadGrid");

    //            $("#mapToSalesOrderCustomer").attr("disabled", "disabled");
    //            $("#mapToSOCustomerWithCRSalesLevels").attr("disabled", "disabled");

    //            $("#unprocessedList").jqGrid().trigger("reloadGrid");

    //        },
    //        error: function (e) {

    //            // PROGRAMMER'S NOTE:  Need to handle case of error encountered during AJAX call

    //        }
    //    });     // .ajax()

    //});         // $("#mapToSOCustomerWithCRSalesLevels").click()

});     // // document.ready()
