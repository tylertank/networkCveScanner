function QueryAPI() {
    $.post(
        {
            url: "/Nist/QueryAPI",
            
        })
        .done(function (response) {
            console.log("Done");
        }).catch(error => {
            window.location.reload();
            console.log("Error");
        });
}