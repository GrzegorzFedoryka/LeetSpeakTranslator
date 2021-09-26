$(document).ready(function () {
    $("#form").submit(function (e) {
        e.preventDefault(); // avoid to execute the actual submit of the form.

        var translatorName = $('#Translators').find(":selected").text()
        if (translatorName === "Select Translator") {
            $("#translatedResult").append("<tr><td colspan=\"2\">Please select translator</td></tr>");
            return;
        }
        //parse form data to json format
        var data = JSON.stringify($(form).serializeArray().reduce(function (obj, item) {
            if (item.name != "Translators") {
                obj[item.name] = item.value;
            }
            return obj;
        }, {}
        ));
        console.log(data);
        // send ajax
        
        $.ajax({
            url: '/Translator/ConvertText/' + translatorName, // url where to submit the request
            type: "POST",
            contentType: 'application/json',
            data: data,

            success: function (result) {
                $("#translatedResult").append(result);
            },
            error: function (xhr, resp, text) {
                $("#translatedResult").append("<tr><td colspan=\"2\">Error has occured</td></tr>");
                console.log(xhr, resp, text);
            }
        })
    });
});