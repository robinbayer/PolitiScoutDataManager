var personSearchResultGridUrl;

const SCREEN_MODE_ADD = "Add";
const SCREEN_MODE_EDIT = "Edit";

const WILDCARD_ALL_TOKEN = "%";
const ERROR_MESSAGE_AUTO_DISMISS_MILLISECONDS = 5000;

$(function () {

    $("#errorMessageBlock").hide();
    $("#dialogErrorMessageBlock").hide();
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

        // Perform validations
        $("#dialogErrorMessageBlock").hide();

        if ($("#lastName").val() == "") {
            $("#dialogErrorMessageText").html("A Last Name must be specified.");
            $("#lastName").focus();
            $("#dialogErrorMessageBlock").show();
            return;
        }

        if ($("#firstName").val() == "") {
            $("#dialogErrorMessageText").html("A First Name must be specified.");
            $("#firstName").focus();
            $("#dialogErrorMessageBlock").show();
            return;
        }

        if ($("#preferredFirstName").val() == "") {
            $("#dialogErrorMessageText").html("A Preferred First Name must be specified.");
            $("#preferredFirstName").focus();
            $("#dialogErrorMessageBlock").show();
            return;
        }

        var person = new Object();

        person.lastName = $("#lastName").val();
        person.firstName = $("#firstName").val();
        person.preferredFirstName = $("#preferredFirstName").val();

        if ($("#middleName").val() != "") {
            person.middleName = $("#middleName").val();
        }

        if ($("#generationSuffix").val() != "") {
            person.generationSuffix = $("#generationSuffix").val();
        }

        if ($("#dateOfBirth").val() != "") {
            person.dateOfBirth = new Date($("#dateOfBirth").val());
        }

        if ($("#screenMode").val() == SCREEN_MODE_ADD) {

            var addPersonInformationUrl = $("#baseWebServiceUrl").val() + "/ws/person/";

            $.ajax({
                type: "POST",
                url: addPersonInformationUrl,
                traditional: true,
                data: JSON.stringify(person),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (returnValue) {

                    // Reload Search Results grid with added or edited record
                    var gridPostData = $("#personSearchResultGrid").jqGrid("getGridParam", "postData");

                    $("#lastNameSearchMask").val($("#lastName").val());
                    $("#firstNameSearchMask").val($("#firstName").val());
                    $("#preferredFirstNameSearchMask").val($("#preferredFirstName").val());

                    gridPostData.lastNameSearchMask = $("#lastName").val().toUpperCase();
                    gridPostData.firstNameSearchMask = $("#firstName").val().toUpperCase();
                    gridPostData.preferredFirstNameSearchMask = $("#preferredFirstName").val().toUpperCase();

                    $("#personSearchResultGrid").jqGrid('setGridParam', { url: personSearchResultGridUrl }).trigger("reloadGrid");

                },
                error: function (e) {

                    $("#errorMessageBlock").show();
                    $("#errorMessageText").html("An error occured during web service call to update Person information");
                    setTimeout(function () {

                        $("#errorMessageText").html("");
                        $("#errorMessageBlock").hide();

                    }, ERROR_MESSAGE_AUTO_DISMISS_MILLISECONDS);

                }
            });     // .ajax()


        } else {

            person.personId = parseInt($("#selectedPersonId").val());

            // Edit mode
            var updatePersonInformationUrl = $("#baseWebServiceUrl").val() + "/ws/person/";

            $.ajax({
                type: "PUT",
                url: updatePersonInformationUrl,
                traditional: true,
                data: JSON.stringify(person),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (returnValue) {

                    // Reload Search Results grid with added or edited record
                    var gridPostData = $("#personSearchResultGrid").jqGrid("getGridParam", "postData");

                    $("#lastNameSearchMask").val($("#lastName").val());
                    $("#firstNameSearchMask").val($("#firstName").val());
                    $("#preferredFirstNameSearchMask").val($("#preferredFirstName").val());

                    gridPostData.lastNameSearchMask = $("#lastName").val().toUpperCase();
                    gridPostData.firstNameSearchMask = $("#firstName").val().toUpperCase();
                    gridPostData.preferredFirstNameSearchMask = $("#preferredFirstName").val().toUpperCase();

                    $("#personSearchResultGrid").jqGrid('setGridParam', { url: personSearchResultGridUrl }).trigger("reloadGrid");

                },
                error: function (e) {

                    $("#errorMessageBlock").show();
                    $("#errorMessageText").html("An error occured during web service call to update Person information");
                    setTimeout(function () {

                        $("#errorMessageText").html("");
                        $("#errorMessageBlock").hide();

                    }, ERROR_MESSAGE_AUTO_DISMISS_MILLISECONDS);

                }
            });     // .ajax()

        }


        $("#addEditPersonModal").modal("hide");

        if ($("#screenMode").val() == SCREEN_MODE_ADD) {
            $("#addPerson").focus();
        }

    });

    $("#lastNameSearchMask").keypress(function (event) {
        if (event.keyCode === 13) {
            $("#searchForPerson").click();
        }
    }); 

    $("#firstNameSearchMask").keypress(function (event) {
        if (event.keyCode === 13) {
            $("#searchForPerson").click();
        }
    }); 

    $("#preferredFirstNameSearchMask").keypress(function (event) {
        if (event.keyCode === 13) {
            $("#searchForPerson").click();
        }
    }); 

    $("#searchForPerson").click(function () {

        // PROGRAMMER'S NOTE:  value set here to avoid web service call on entry to page
        personSearchResultGridUrl = $("#baseWebServiceUrl").val() + "/ws/person/search/";

        var gridPostData = $("#personSearchResultGrid").jqGrid("getGridParam", "postData");

        gridPostData.lastNameSearchMask = $("#lastNameSearchMask").val().toUpperCase() + WILDCARD_ALL_TOKEN;
        gridPostData.firstNameSearchMask = $("#firstNameSearchMask").val().toUpperCase() + WILDCARD_ALL_TOKEN;
        gridPostData.preferredFirstNameSearchMask = $("#preferredFirstNameSearchMask").val().toUpperCase() + WILDCARD_ALL_TOKEN;

        $("#personSearchResultGrid").jqGrid('setGridParam', { url: personSearchResultGridUrl}).trigger("reloadGrid");

    });     // $("#searchForPerson").click()

    $("#addPerson").click(function () {

        $("#screenMode").val(SCREEN_MODE_ADD);
        $("#addEditPersonModalLabel").text(SCREEN_MODE_ADD + " Person record");

        $("#lastName").val("");
        $("#firstName").val("");
        $("#middleName").val("");
        $("#preferredFirstName").val("");
        $("#generationSuffix").val("");
        $("#dateOfBirth").val("");

        $("#addEditPersonModal").modal("show");

    });     // $("#addPerson").click()

    $("#editPerson").click(function () {

        var personInformationUrl = $("#baseWebServiceUrl").val() + "/ws/person/" + $("#selectedPersonId").val() + "/";

        $.ajax({
            type: "GET",
            url: personInformationUrl,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (returnValue) {

                // Load existing values from database into controls
                $("#lastName").val(returnValue.lastName);
                $("#firstName").val(returnValue.firstName);
                $("#middleName").val(returnValue.middleName);
                $("#preferredFirstName").val(returnValue.preferredFirstName);
                $("#generationSuffix").val(returnValue.generationSuffix);

                if (returnValue.dateOfBirth != "0001-01-01T00:00:00") {
                    // Format date of birth to YYYY-MM-DD
                    $("#dateOfBirth").val(returnValue.dateOfBirth.substring(0, 10));
                }

                $("#screenMode").val(SCREEN_MODE_EDIT);
                $("#addEditPersonModalLabel").text(SCREEN_MODE_EDIT + " Person record");

                $("#addEditPersonModal").modal("show");

            },
            error: function (jqXHR, textStatus, errorThrown) {

                $("#errorMessageBlock").show();
                $("#errorMessageText").html("An error occured during web service call to populate Person information");
                setTimeout(function () {

                    $("#errorMessageText").html("");
                    $("#errorMessageBlock").hide();

                }, ERROR_MESSAGE_AUTO_DISMISS_MILLISECONDS);

            }
        });        // .ajax()

    });     // $("editPerson").click()

    $("#firstName").focusout(function () {
        if ($("#screenMode").val() == SCREEN_MODE_ADD) {
            if ($("#preferredFirstName").val() == "") {
                $("#preferredFirstName").val($("#firstName").val());
            }
        }
    })

    $("#addEditPersonModal").on("shown.bs.modal", function () {
        $("#lastName").focus();
    })

    $("#deletePerson").click(function () {
        $("#confirmDeletePersonModal").modal("show");
    });     // $("#deletePerson").click()


    $("#onDialogDelete").click(function () {

        var deletePersonInformationUrl = $("#baseWebServiceUrl").val() + "/ws/person/" + $("#selectedPersonId").val() + "/";

        $.ajax({
            type: "DELETE",
            url: deletePersonInformationUrl,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (returnValue) {

                $("#personSearchResultGrid").jqGrid('setGridParam', { url: personSearchResultGridUrl }).trigger("reloadGrid");

            },
            error: function (jqXHR, textStatus, errorThrown) {

                $("#errorMessageBlock").show();
                $("#errorMessageText").html("An error occured during web service call to delete Person information");
                setTimeout(function () {

                    $("#errorMessageText").html("");
                    $("#errorMessageBlock").hide();

                }, ERROR_MESSAGE_AUTO_DISMISS_MILLISECONDS);

            }
        });        // .ajax()

    });     // $("#onDialogDelete").click()


});     // // document.ready()
