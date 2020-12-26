var personSearchResultGridUrl;

const WILDCARD_ALL_TOKEN = "%";
const ERROR_MESSAGE_AUTO_DISMISS_MILLISECONDS = 5000;

$(function () {

    $("#errorMessageBlock").hide();
    $("#editPerson").attr("disabled", "disabled");
    $("#deletePerson").attr("disabled", "disabled");

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
            'lastNameSearchMask': function () {
                return "";
            },
            'firstNameSearchMask': function () {
                return "";
            },
            'preferredFirstNameSearchMask': function () {
                return "";
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

        // PROGRAMMER'S NOTE:  value set here to avoid web service call on entry to page
        personSearchResultGridUrl = $("#baseWebServiceUrl").val() + "/ws/person/search/";

        var gridPostData = $("#personSearchResultGrid").jqGrid("getGridParam", "postData");

        gridPostData.lastNameSearchMask = $("#lastNameSearchMask").val().toUpperCase() + WILDCARD_ALL_TOKEN;
        gridPostData.firstNameSearchMask = $("#firstNameSearchMask").val().toUpperCase() + WILDCARD_ALL_TOKEN;
        gridPostData.preferredFirstNameSearchMask = $("#preferredFirstNameSearchMask").val().toUpperCase() + WILDCARD_ALL_TOKEN;

        $("#personSearchResultGrid").jqGrid('setGridParam', { url: personSearchResultGridUrl}).trigger("reloadGrid");

    });     // $("#searchForPerson").click(function ()

});     // // document.ready()
