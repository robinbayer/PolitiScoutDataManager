﻿var territoryLevelListUrl;
var territoryListUrl;
var resultOfCandidacyListUrl;
var reasonForEntryListUrl;
var reasonForDepartureListUrl;
var politicalPartyListUrl;
var personSearchResultGridUrl;
var candidateForElectionGridUrl;
var occupiedElectedOfficeGrid;

const SCREEN_MODE_ADD = "Add";
const SCREEN_MODE_EDIT = "Edit";

const WILDCARD_ALL_TOKEN = "%";
const ERROR_MESSAGE_AUTO_DISMISS_MILLISECONDS = 5000;

$(function () {

    $("#errorMessageBlock").hide();
    $("#dialogErrorMessageBlock_Person").hide();
    $("#editPerson").attr("disabled", "disabled");
    $("#deletePerson").attr("disabled", "disabled");
    $("#addCandidateForElection").attr("disabled", "disabled");
    $("#editCandidateForElection").attr("disabled", "disabled");
    $("#deleteCandidateForElection").attr("disabled", "disabled");
    $("#addOccupiedElectedOffice").attr("disabled", "disabled");
    $("#editOccupiedElectedOffice").attr("disabled", "disabled");
    $("#deleteOccupiedElectedOffice").attr("disabled", "disabled");

    $("#territoryLevel_Candidate").find("option").remove();
    $("#territoryLevel_OccupiedOffice").find("option").remove();
    $("#resultOfCandidacy").find("option").remove();
    $("#reasonForEntry").find("option").remove();
    $("#reasonForDeparture").find("option").remove();
    $("#politicalParty").find("option").remove();
    $("#politicalParty").find("option").remove();
    $("#electedOfficeForTerritory_Candidate").find("option").remove();
    $("#distinctOfficeForTerritory_Candidate").find("option").remove();
    $("#electedOfficeForTerritory_OccupiedOffice").find("option").remove();
    $("#distinctOfficeForTerritory_OccupiedOffice").find("option").remove();

    ////////////////////////////////////////////////////
    //////////// Populate dictionary objects ///////////
    ////////////////////////////////////////////////////

    territoryLevelListUrl = $("#baseWebServiceUrl").val() + "/ws/dictionary/territoryLevel/list/";
    resultOfCandidacyListUrl = $("#baseWebServiceUrl").val() + "/ws/dictionary/resultOfCandidacy/list/";
    reasonForEntryListUrl = $("#baseWebServiceUrl").val() + "/ws/dictionary/reasonForEntry/list/";
    reasonForDepartureListUrl = $("#baseWebServiceUrl").val() + "/ws/dictionary/reasonForDeparture/list/";

    //----------------------------------------
    //---- Territory Level list dropdown -----
    //----------------------------------------

    $.ajax({
        type: "GET",
        url: territoryLevelListUrl,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (returnValue) {

            var territoryLevel_Candidate = $("#territoryLevel_Candidate");
            var territoryLevel_OccupiedOffice = $("#territoryLevel_OccupiedOffice");

            returnValue.forEach((element) => {

                var option = $("<option />");
                option.val(element.territoryLevelId);
                option.html(element.referenceName);
                territoryLevel_Candidate.append(option);
                territoryLevel_OccupiedOffice.append(option);

            });

            // Select first item in list and trigger initial populate
            $("#territoryLevel_Candidate option:eq(0)").prop('selected', true);

            // PROGRAMMER'S NOTE:  If triggering immediately, would routinely fail the refresh call to the web service.
            setTimeout(function () {
                $("#territoryLevel_Candidate").trigger("change");
            }, 100);

            // Select first item in list and trigger initial populate
            $("#territoryLevel_OccupiedOffice option:eq(0)").prop('selected', true);

            // PROGRAMMER'S NOTE:  If triggering immediately, would routinely fail the refresh call to the web service.
            setTimeout(function () {
                $("#territoryLevel_OccupiedOffice").trigger("change");
            }, 100);

        },
        error: function (jqXHR, textStatus, errorThrown) {

            $("#errorMessageBlock").show();
            $("#errorMessageText").html("An error occured during web service call to populate Territory Level list");
            setTimeout(function () {

                $("#errorMessageText").html("");
                $("#errorMessageBlock").hide();

            }, ERROR_MESSAGE_AUTO_DISMISS_MILLISECONDS);

        }
    });        // .ajax()

    //--------------------------------------------
    //---- Result of Candidacy list dropdown -----
    //--------------------------------------------

    $.ajax({
        type: "GET",
        url: resultOfCandidacyListUrl,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (returnValue) {

            var resultOfCandidacy = $("#resultOfCandidacy");

            returnValue.forEach((element) => {

                var option = $("<option />");
                option.val(element.resultOfCandidacyId);
                option.html(element.description);
                resultOfCandidacy.append(option);

            });
        },
        error: function (jqXHR, textStatus, errorThrown) {

            $("#errorMessageBlock").show();
            $("#errorMessageText").html("An error occured during web service call to populate Result of Candidacy list");
            setTimeout(function () {

                $("#errorMessageText").html("");
                $("#errorMessageBlock").hide();

            }, ERROR_MESSAGE_AUTO_DISMISS_MILLISECONDS);

        }
    });        // .ajax()

    //-----------------------------------------
    //---- Reason for Entry list dropdown -----
    //-----------------------------------------

    $.ajax({
        type: "GET",
        url: reasonForEntryListUrl,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (returnValue) {

            var reasonForEntry = $("#reasonForEntry");

            returnValue.forEach((element) => {

                var option = $("<option />");
                option.val(element.reasonForEntryId);
                option.html(element.description);
                reasonForEntry.append(option);

            });
        },
        error: function (jqXHR, textStatus, errorThrown) {

            $("#errorMessageBlock").show();
            $("#errorMessageText").html("An error occured during web service call to populate Reason for Entry list");
            setTimeout(function () {

                $("#errorMessageText").html("");
                $("#errorMessageBlock").hide();

            }, ERROR_MESSAGE_AUTO_DISMISS_MILLISECONDS);

        }
    });        // .ajax()

    //---------------------------------------------
    //---- Reason for Departure list dropdown -----
    //---------------------------------------------

    $.ajax({
        type: "GET",
        url: reasonForDepartureListUrl,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (returnValue) {

            var reasonForDeparture = $("#reasonForDeparture");

            returnValue.forEach((element) => {

                var option = $("<option />");
                option.val(element.reasonForDepartureId);
                option.html(element.description);
                reasonForDeparture.append(option);

            });
        },
        error: function (jqXHR, textStatus, errorThrown) {

            $("#errorMessageBlock").show();
            $("#errorMessageText").html("An error occured during web service call to populate Reason for Departure list");
            setTimeout(function () {

                $("#errorMessageText").html("");
                $("#errorMessageBlock").hide();

            }, ERROR_MESSAGE_AUTO_DISMISS_MILLISECONDS);

        }
    });        // .ajax()

    ///////////////////////////////////////////////////////////
    //////////// Populate other single-load objects ///////////
    ///////////////////////////////////////////////////////////

    //---------------------------------------------
    //---- Political Party list dropdown -----
    //---------------------------------------------

    $.ajax({
        type: "GET",
        url: politicalPartyListUrl,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (returnValue) {

            var politicalParty = $("#politicalParty");

            returnValue.forEach((element) => {

                var option = $("<option />");
                option.val(element.politicalPartyId);
                option.html(element.referenceName + "(" + element.abbreviation + ")");
                politicalParty.append(option);

            });
        },
        error: function (jqXHR, textStatus, errorThrown) {

            $("#errorMessageBlock").show();
            $("#errorMessageText").html("An error occured during web service call to populate Political Party list");
            setTimeout(function () {

                $("#errorMessageText").html("");
                $("#errorMessageBlock").hide();

            }, ERROR_MESSAGE_AUTO_DISMISS_MILLISECONDS);

        }
    });        // .ajax()


    $("#personSearchResultGrid").jqGrid({
        url: personSearchResultGridUrl,
        type: 'GET',
        ajaxGridOptions: {
            contentType: 'application/json; charset=utf-8',
            timeout: 60000,
            dataType: "json",
            type: "POST",
            cache: false
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
        colNames: ['Person ID', 'Last Name', 'First Name', 'Preferred First Name', 'Middle Name', 'Date of Birth'],
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

            $("#addCandidateForElection").attr("disabled", "disabled");
            $("#editCandidateForElection").attr("disabled", "disabled");
            $("#deleteCandidateForElection").attr("disabled", "disabled");
            $("#addOccupiedElectedOffice").attr("disabled", "disabled");
            $("#editOccupiedElectedOffice").attr("disabled", "disabled");
            $("#deleteOccupiedElectedOffice").attr("disabled", "disabled");

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

                $("#addCandidateForElection").removeAttr("disabled");
                $("#addOccupiedElectedOffice").removeAttr("disabled");

                candidateForElectionGridUrl = $("#baseWebServiceUrl").val() + "/ws/candidateForElection/forPerson/" + rowId + "/";
                $("#candidateForElectionGrid").jqGrid('setGridParam', { url: candidateForElectionGridUrl }).trigger("reloadGrid");

                occupiedElectedOfficesGridUrl = $("#baseWebServiceUrl").val() + "/ws/occupiedElectedOffice/forPerson/" + rowId + "/";
                $("#occupiedElectedOfficeGrid").jqGrid('setGridParam', { url: occupiedElectedOfficesGridUrl }).trigger("reloadGrid");

            }       // left-click
        }       // onSelectRow event

    });      // $("#personSearchResultGrid").jqGrid()

    $("#okOnAddEditPersonModal").click(function () {

        // Perform validations
        $("#dialogErrorMessageBlock_Person").hide();

        if ($("#lastName").val() == "") {
            $("#dialogErrorMessageText_Person").html("A Last Name must be specified.");
            $("#lastName").focus();
            $("#dialogErrorMessageBlock_Person").show();
            return;
        }

        if ($("#firstName").val() == "") {
            $("#dialogErrorMessageText_Person").html("A First Name must be specified.");
            $("#firstName").focus();
            $("#dialogErrorMessageBlock_Person").show();
            return;
        }

        if ($("#preferredFirstName").val() == "") {
            $("#dialogErrorMessageText_Person").html("A Preferred First Name must be specified.");
            $("#preferredFirstName").focus();
            $("#dialogErrorMessageBlock_Person").show();
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

                    $("#errorMessageBlock_Person").show();
                    $("#errorMessageText_Person").html("An error occured during web service call to update Person information");
                    setTimeout(function () {

                        $("#errorMessageText_Person").html("");
                        $("#errorMessageBlock_Person").hide();

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

                    $("#errorMessageBlock_Person").show();
                    $("#errorMessageText_Person").html("An error occured during web service call to update Person information");
                    setTimeout(function () {

                        $("#errorMessageText_Person").html("");
                        $("#errorMessageBlock_Person").hide();

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

        $("#personSearchResultGrid").jqGrid('setGridParam', { url: personSearchResultGridUrl }).trigger("reloadGrid");

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

                $("#errorMessageBlock_Person").show();
                $("#errorMessageText_Person").html("An error occured during web service call to populate Person information");
                setTimeout(function () {

                    $("#errorMessageText_Person").html("");
                    $("#errorMessageBlock_Person").hide();

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


    $("#onDialogDeletePerson").click(function () {

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

                $("#errorMessageBlock_Person").show();
                $("#errorMessageText_Person").html("An error occured during web service call to delete Person information");
                setTimeout(function () {

                    $("#errorMessageText").html("");
                    $("#errorMessageBlock").hide();

                }, ERROR_MESSAGE_AUTO_DISMISS_MILLISECONDS);

            }
        });        // .ajax()

    });     // $("#onDialogDeletePerson").click()

    $("#candidateForElectionGrid").jqGrid({
        url: candidateForElectionGridUrl,
        type: 'GET',
        ajaxGridOptions: {
            contentType: 'application/json; charset=utf-8',
            timeout: 60000,
            dataType: "json",
            type: "POST",
            cache: false
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
        colNames: ['CandidateForElectionID', 'Level', 'Territory', 'Office Name', 'Designation', 'Election Date', 'Political Party'],
        colModel: [
            {
                name: 'candidateForElectionId',
                index: 'candidateForElectionId',
                width: 50,
                align: "left",
                key: true,
                hidden: true,
                editable: false
            },
            {
                name: 'territoryLevelDescription',
                index: 'territoryLevelDescription',
                align: "left",
                width: 100,
                editable: false
            },
            {
                name: 'territoryDescription',
                index: 'territoryDescription',
                align: "left",
                width: 100,
                editable: false
            },
            {
                name: 'electedOfficeDescription',
                index: 'electedOfficeDescription',
                align: "left",
                width: 150,
                editable: false
            },
            {
                name: 'distinctOfficeDesignator',
                index: 'distinctOfficeDesignator',
                align: "left",
                width: 150,
                editable: false
            },
            {
                name: 'electionDate',
                index: 'electionDate',
                formatter: 'date', formatoptions: { srcformat: 'Y-M-d', newformat: 'Y-m-d' },
                width: 100,
                editable: false
            }
        ],
        beforeRequest: function () {

            $("#editCandidateForElection").attr("disabled", "disabled");
            $("#deleteCandidateForElection").attr("disabled", "disabled");

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

                $("#selectedCandidateForElectionId").val(rowId);

                $("#editCandidateForElection").removeAttr("disabled");
                $("#deleteCandidateForElection").removeAttr("disabled");

            }       // left-click
        }       // onSelectRow event

    });      // $("#candidateForElectionGrid").jqGrid()

    $("#addCandidateForElection").click(function () {

        $("#screenMode").val(SCREEN_MODE_ADD);
        $("#addEditCandidateForElectionModalLabel").text(SCREEN_MODE_ADD + " Candidate for Election record");

        /*

        $("#lastName").val("");
        $("#firstName").val("");
        $("#middleName").val("");
        $("#preferredFirstName").val("");
        $("#generationSuffix").val("");
        $("#dateOfBirth").val("");
        */

        $("#addEditCandidateForElectionModal").modal("show");

        /*
         * 
        candidate_for_election_id integer NOT NULL,
        person_id integer NOT NULL,
        election_for_territory_id integer NOT NULL,
        distinct_elected_office_for_territory_id integer NOT NULL,
        political_party_id integer NOT NULL,
        record_added_date_time date NOT NULL,
        record_last_updated_date_time date NOT NULL,
         *
            public int personId { get; set; }
            public int electionForTerritoryId { get; set; }
            public int distinctElectedOfficeForTerritoryId { get; set; }
            public int politicalPartyId { get; set; }
            public int resultOfCandidacyId { get; set; }
        
         * */


    });     // $("#addCandidateForElection").click()

    $("#editCandidateForElection").click(function () {

        var candidateForElectionInformationUrl = $("#baseWebServiceUrl").val() + "/ws/candidateForElection/" + $("#selectedCandidateForElectionId").val() + "/";

        $.ajax({
            type: "GET",
            url: personInformationUrl,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (returnValue) {

                /*
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
                */

                $("#screenMode").val(SCREEN_MODE_EDIT);
                $("#addEditCandidateForElectionModalLabel").text(SCREEN_MODE_EDIT + " Candidate for Election record");

                $("#addEditCandidateForElectionModal").modal("show");

            },
            error: function (jqXHR, textStatus, errorThrown) {

                $("#errorMessageBlock_CandidateForElection").show();
                $("#errorMessageText_CandidateForElection").html("An error occured during web service call to populate Candidate for Election information");
                setTimeout(function () {

                    $("#errorMessageText_CandidateForElection").html("");
                    $("#errorMessageBlock_CandidateForElection").hide();

                }, ERROR_MESSAGE_AUTO_DISMISS_MILLISECONDS);

            }
        });        // .ajax()

    });     // $("editCandidateForElection").click()



    $("#okOnAddEditCandidateForElectionModal").click(function () {

        // Perform validations
        $("#dialogErrorMessageBlock_CandidateForElection").hide();

        if ($("#lastName").val() == "") {
            $("#dialogErrorMessageText_CandidateForElection").html("A Last Name must be specified.");
            $("#lastName").focus();
            $("#dialogErrorMessageBlock_CandidateForElection").show();
            return;
        }


        /*
         *
         * public int candidateForElectionId { get; set; }
                public int personId { get; set; }
                public int territoryLevelId { get; set; }
                public string territoryLevelDescription { get; set; }
                public int territoryId { get; set; }
                public string territoryFullName { get; set; }
                public string electedOfficeDescription { get; set; }
                public int distinctElectedOfficeForTerritoryId { get; set; }
                public string distinctOfficeDesignator { get; set; }
                public int electionForTerritoryId { get; set; }
                public DateTime electionDate { get; set; }
                public int politicalPartyId { get; set; }
                public string politicalPartyReferenceName { get; set; }
                public int resultOfCandidacyId { get; set; }
                public string resultOfCandidacyDescription { get; set; }
        
         * */


        var candidateForElection = new Object();

        //candidateForElection....... = $("#firstName").val();


        if ($("#screenMode").val() == SCREEN_MODE_ADD) {

            var addCandidateForElectionInformationUrl = $("#baseWebServiceUrl").val() + "/ws/candidateForElection/";

            $.ajax({
                type: "POST",
                url: addCandidateForElectionInformationUrl,
                traditional: true,
                data: JSON.stringify(person),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (returnValue) {

                    candidateForElectionGridUrl = $("#baseWebServiceUrl").val() + "/ws/candidateForElection/forPerson/" + $("#selectedPersonId").val() + "/";
                    $("#candidateForElectionGrid").jqGrid('setGridParam', { url: candidateForElectionGridUrl }).trigger("reloadGrid");

                },
                error: function (e) {

                    $("#errorMessageBlock_CandidateForElection").show();
                    $("#errorMessageText_CandidateForElection").html("An error occured during web service call to add Candidate for Election information");
                    setTimeout(function () {

                        $("#errorMessageText_CandidateForElection").html("");
                        $("#errorMessageBlock_CandidateForElection").hide();

                    }, ERROR_MESSAGE_AUTO_DISMISS_MILLISECONDS);

                }
            });     // .ajax()


        } else {

            candidateForElection.candidateForElectionId = parseInt($("#selectedCandidateForElectionId").val());

            // Edit mode
            var updateCandidateForElectionInformationUrl = $("#baseWebServiceUrl").val() + "/ws/candidateForElection/";

            $.ajax({
                type: "PUT",
                url: updateCandidateForElectionInformationUrl,
                traditional: true,
                data: JSON.stringify(candidateForElection),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (returnValue) {

                    candidateForElectionGridUrl = $("#baseWebServiceUrl").val() + "/ws/candidateForElection/forPerson/" + $("#selectedPersonId").val() + "/";
                    $("#candidateForElectionGrid").jqGrid('setGridParam', { url: candidateForElectionGridUrl }).trigger("reloadGrid");

                },
                error: function (e) {

                    $("#errorMessageBlock").show();
                    $("#errorMessageText").html("An error occured during web service call to update Person information");
                    setTimeout(function () {

                        $("#errorMessageText_CandidateForElection").html("");
                        $("#errorMessageBlock_CandidateForElection").hide();

                    }, ERROR_MESSAGE_AUTO_DISMISS_MILLISECONDS);

                }
            });     // .ajax()

        }

        $("#addEditCandidateForElection").modal("hide");

        if ($("#screenMode").val() == SCREEN_MODE_ADD) {
            $("#addCandidateForElection").focus();
        }

    });


    $("#occupiedElectedOfficeGrid").jqGrid({
        url: occupiedElectedOfficesGridUrl,
        type: 'GET',
        ajaxGridOptions: {
            contentType: 'application/json; charset=utf-8',
            timeout: 60000,
            dataType: "json",
            type: "POST",
            cache: false
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
        colNames: ['Occupied Elected Office ID', 'Level', 'Territory', 'Office Name', 'Designation', 'Start Date', 'End Date'],
        colModel: [
            {
                name: 'occupiedElectedOfficeId',
                index: 'occupiedElectedOfficeId',
                width: 50,
                align: "left",
                key: true,
                hidden: true,
                editable: false
            },
            {
                name: 'territoryLevelDescription',
                index: 'territoryLevelDescription',
                align: "left",
                width: 200,
                editable: false
            },
            {
                name: 'territoryDescription',
                index: 'territoryDescription',
                align: "left",
                width: 100,
                editable: false
            },
            {
                name: 'electedOfficeDescription',
                index: 'electedOfficeDescription',
                align: "left",
                width: 150,
                editable: false
            },
            {
                name: 'distinctOfficeDesignator',
                index: 'distinctOfficeDesignator',
                align: "left",
                width: 150,
                editable: false
            },
            {
                name: 'startDate',
                index: 'startDate',
                formatter: 'date', formatoptions: { srcformat: 'Y-M-d', newformat: 'Y-m-d' },
                width: 80,
                editable: false
            },
            {
                name: 'endDate',
                index: 'endDate',
                formatter: 'date', formatoptions: { srcformat: 'Y-M-d', newformat: 'Y-m-d' },
                width: 80,
                editable: false
            },
            {
                name: 'reasonForEntryDescription',
                index: 'reasonForEntryDescription',
                align: "left",
                width: 100,
                editable: false
            },
            {
                name: 'reasonForDepartureDescription',
                index: 'reasonForDepartureDescription',
                align: "left",
                width: 100,
                editable: false
            }
        ],
        beforeRequest: function () {

            $("#editOccupiedElectedOffice").attr("disabled", "disabled");
            $("#deleteOccupiedElectedOffice").attr("disabled", "disabled");

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

                $("#selectedOccupiedElectedOfficeId").val(rowId);

                $("#editOccupiedElectedOffice").removeAttr("disabled");
                $("#deleteOccupiedElectedOffice").removeAttr("disabled");

            }       // left-click
        }       // onSelectRow event

    });      // $("#occupiedElectedOfficeGrid").jqGrid()


    $("#addOccupiedElectedOffice").click(function () {

        /*
        $("#screenMode").val(SCREEN_MODE_ADD);
        $("#addEditPersonModalLabel").text(SCREEN_MODE_ADD + " Person record");
    
        $("#lastName").val("");
        $("#firstName").val("");
        $("#middleName").val("");
        $("#preferredFirstName").val("");
        $("#generationSuffix").val("");
        $("#dateOfBirth").val("");
    
        $("#addEditPersonModal").modal("show");
        */


        /*
    
            public int occupiedElectedOfficeId { get; set; }
            public int personId { get; set; }
            public int distinctElectedOfficeForTerritoryId { get; set; }
            public DateTime startDate { get; set; }
            public DateTime endDate { get; set; }
            public int reasonForEntryId { get; set; }
            public int reasonForDepartureId { get; set; }
         *
         * */

    });     // $("#addOccupiedElectedOffice").click()

    $("#editOccupiedElectedOffice").click(function () {


        /*
    
            public int occupiedElectedOfficeId { get; set; }
            public int personId { get; set; }
            public int distinctElectedOfficeForTerritoryId { get; set; }
            public DateTime startDate { get; set; }
            public DateTime endDate { get; set; }
            public int reasonForEntryId { get; set; }
            public int reasonForDepartureId { get; set; }
         *
         * */


        /*
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
        */

    });     // $("editOccupiedElectedOffice").click()

});     // // document.ready()
