function GetDashboardData(_url) {
    const monthNames = ["January", "February", "March", "April", "May", "June",
        "July", "August", "September", "October", "November", "December"
    ];
    var date = new Date();
    var url = _url + '?month=' + date.getDate() + '&year=' + date.getFullYear();
    var type = 'GET';
    $.ajax({
        url: url,
        type: type,
        processData: false,
        contentType: false,
        error: function (xhr) {
            console.log(xhr);
            showErrorMessage('Error: ' + xhr.statusText);
        },
        success: function (response) {
            for (let i = 0; i < response.data.length; i++) {
                if (response.data[i].text == "Customers") {
                    $("#customerslabel").html(response.data[i].value);
                    $("#customerssubtitle").html("as on " + monthNames[date.getMonth()] + ", " + date.getFullYear());
                }
                else if (response.data[i].text == "Reports") {
                    $("#reportslabel").html(response.data[i].value);
                }
                else if (response.data[i].text == "Companies") {
                    $("#bgvslabel").html(response.data[i].value);
                    $("#bgvssubtitle").html("as on " + monthNames[date.getMonth()] + ", " + date.getFullYear());
                }
                else if (response.data[i].text == "Users") {
                    $("#userslabel").html(response.data[i].value);
                }
            }
        },
        async: true,
        processData: false
    });
}