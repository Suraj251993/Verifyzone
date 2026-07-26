$(document).ready(function () {
    ShowBalance();
    $('#loading').hide();
    $.ajax({
        type: "GET",
        url: "/Customer/GetEmployee",
        data: { Empcode: "", Mode: "AddEx" },
        contentType: "application/html; charset=utf-8",
        dataType: 'html', // changing data type to json
        success: function (data) { // here I'm adding data as a parameter which stores the response
            //console.log(data); // instead of alert I'm changing this to console.log which logs all the response in console.
            $('#divForm').html(data);
            $('#divForm').show();
            $('#divnorecord').hide();

            $('#btndraft').hide();
            $('#btndraft2').hide();
            $('#btndraft3').hide();
            $('#btnsubmit').hide();
            //$('#loading').hide();
        },
        error: function (err) {
            console.log(err.status);
        }
    });

    $('#SaveEmpForm').click(function (e) {
        if ($('#chknew').is(":checked")) {
            if ($.trim($('#Customername').val()) == "" || $.trim($('#Hrname').val()) == "" || $.trim($('#Hremail').val()) == "") {
                showErrorMessage("Employer name, HR name and HR email are mandatory for new employer");
                return;
            }
        }
        if ($.trim($('#Employeecode').val()) == "" && $.trim($('#Name').val()) == "" && $.trim($('#Designation').val()) == ""
            && $.trim($('#Fromdate').val()) == "" && $.trim($('#Todate').val()) == "" && $.trim($('#Reasonforleaving').val()) == "" && $.trim($('#Location').val()) == ""
            && $('#Jobtype').val() == "" && $.trim($('#Lastdrawnsalary').val()) == "" && $.trim($('#Reportingto').val()) == ""
            && $.trim($('#Managerdesignation').val()) == "") {
            showErrorMessage("Please fill in couple of employment details before submitting the request");
            return;
        }

        //if (!$('#frmEmpForm').valid()) return;
        e.preventDefault();
        $('#loading').show();
        var url = '@Url.Action("SendExRequest", "Customer")';
        const form = document.getElementById('frmEmpForm');
        const data = new FormData(form);
        const empform = Object.fromEntries(data.entries());
        var questAns = [];
        $('#divQuestions input[type="text"]').each(function () {
            questAns.push({ questionId: $(this).attr('id'), answer: $(this).val() })
        });

        //console.log(empform);
        //console.log(questAns);
        postdata = { ...empform, employeeQuestions: questAns };

        $.ajax({
            type: 'POST',
            url: url,
            data: JSON.stringify(postdata),
            contentType: "application/json",
            dataType: 'json', // changing data type to json
            success: function (data) { // here I'm adding data as a parameter which stores the response
                //console.log(data); // instead of alert I'm changing this to console.log which logs all the response in console.
                if (data == "true") {
                    //clearFields();
                    document.getElementById("frmEmpForm").reset();
                    $('#divQuestions > div > div > div > textarea').each(function (index, value) {
                        $(this).val('');
                    });
                    $('#loading').hide();
                    showSuccessMessage('Request was sent to the HR.');
                }
                else if (data == "exists") {
                    $('#loading').hide();
                    showErrorMessage('Employee code already exists');
                }
                else {
                    $('#loading').hide();
                    showErrorMessage(data);
                }
            },
            error: function (err) {
                console.log(err);
                $('#loading').hide();
            }
        });
    });

});