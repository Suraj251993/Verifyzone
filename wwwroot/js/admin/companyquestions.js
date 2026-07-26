$(document).ready(function () {
    $("#SaveQuestionMapper").click(function (e) {
        e.preventDefault();
        alert('eee');
        var url = '/Admin/AddCompanyQuestions';
        var type = 'POST';
        var requestData = $('#frmQuestionaireMapping').serialize();
        console.log(requestData);
        $.ajax({
            url: url,
            type: type,
            data: JSON.stringify(requestData),
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            error: function (xhr) {
                alert('Error: ' + xhr.statusText);
            },
            success: function (result) {
                if (result == true) {
                    $("#LargeModal").modal('hide');
                    // Show success toast message
                    showSuccessMessage('Record saved');
                }
            },
            async: true,
            processData: false
        });
    });
});