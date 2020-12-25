$(() => {

    $("#redirectToSSO").click(() => {

        window.location.replace($("#baseWebServiceUrl").val() + "/redirectToSSO");

    });


});