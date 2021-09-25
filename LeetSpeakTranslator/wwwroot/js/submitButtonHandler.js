$(document).ready(function () {
    $("#form").submit(function (e) {
        e.preventDefault(); // avoid to execute the actual submit of the form.

        //parse form data to json format
        var data = JSON.stringify($(form).serializeArray().reduce(function (obj, item) {
            obj[item.name] = item.value;
            return obj;
        }, {}
        ));
        console.log(data);
        // send ajax
        $.ajax({
            url: '/Leetspeak/ConvertToLeetSpeak', // url where to submit the request
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