
$(function () {

    $("#affiliatedSalesResourceLookup").click(function () {

        window.location.replace($("#baseWebServiceUrl").val() + "/usrinq-ui/affiliatedSalesResourceLookup/");

    });     // $("#maintainMasterSubordinateCustomerList").click(function ()

    $("#viewMasterCustomerInformation").click(function () {

        window.location.replace($("#baseWebServiceUrl").val() + "/usrinq-ui/viewMasterCustomerInformation/");

    });     // $("#viewMasterCustomerInformation").click(function ()

    $("#maintainMasterSubordinateCustomerList").click(function () {

        window.location.replace($("#baseWebServiceUrl").val() + "/admin/maintainMasterSubordinateCustomerList/");

    });     // $("#maintainMasterSubordinateCustomerList").click(function ()

    $("#subordinateExistingCustomer").click(function () {

        window.location.replace($("#baseWebServiceUrl").val() + "/admin/subordinateExistingCustomer");

    });     // $("#subordinateExistingCustomer").click(function ()

    $("#remapToDifferentCorporateEndCustomer").click(function () {

        window.location.replace($("#baseWebServiceUrl").val() + "/admin/remapToDifferentCorporateEndCustomer");

    });     // $("#remapToDifferentCorporateEndCustomer").click(function ()

    $("#promoteSubordinateCustomer").click(function () {

        window.location.replace($("#baseWebServiceUrl").val() + "/admin/promoteSubordinateCustomer");

    });     // $("#promoteSubordinateCustomer").click(function ()

    $("#updateMasterCustomerInformation").click(function () {

        window.location.replace($("#baseWebServiceUrl").val() + "/admin/updateMasterCustomerInformation");

    });     // $("#updateMasterCustomerInformation").click(function ()

    //

    $("#signOut").click(function () {

        window.location.replace($("#baseWebServiceUrl").val() + "/logout/");

    });     // $("#mergeMasterCustomerEntries").click(function ()

});     // // document.ready()



