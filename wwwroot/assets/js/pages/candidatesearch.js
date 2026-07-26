$(function () {    
    //$('#loading').hide();    
    $('#employeeform').hide();
    $('#editApprovalBtn').hide();
    $('#editapprovalrow').hide();
    $('#GetApproval').hide();
    $('#GenerateReport').hide();    
});

function btnGenerateReport(_balanceUrl, _reportUrl) {
    if ($('#txtclientname').val() == '') {
        showErrorMessage('Please enter client name before generating the report');
        return;
    }
    else {
        //$('#loading').show();
        $.ajax({
            type: 'GET',
            url: _balanceUrl,
            dataType: 'json', // changing data type to json
            success: function (data) { // here I'm adding data as a parameter which stores the response
                console.log(data); // instead of alert I'm changing this to console.log which logs all the response in console.
                if (data == 0)
                    showErrorMessage("Insufficient balance to generate report");
                else
                    GenerateReport(_reportUrl, _balanceUrl);
                //$('#loading').hide();
            },
            error: function (err) {
                showErrorMessage(err.statusText);
            }
        });
    }
}

function ClearData() {
    $('#id').val('');
    $('#Candidatename').val('');
    $('#Employeecode').val('');
    $('#Designation').val('');
    $('#Employmentperiod').val('');
    $('#Lastauthorizedby').val('');
    $('#Lastauthorizeddate').val('');
    $('#hdnsearch').val('');
    $('#txtempcode').val('');
    $('#txtdetails').val('');
    $('#GetApproval').hide();
    $('#employeeform').hide();
    //$('#GetEditApproval').hide();
    $('#editapprovalrow').hide();
    $('#GenerateReport').hide();
    //$('#RaiseRequest').hide();

    $('#advancedAutoCompleteSelected').val('');
    $('#txtcode').val('');
}
function GetEmployee(_url) {
    if (selectedCustId == '' || $('#txtcode').val() == '' || $('#lastworkingDate').val() == '') {
        showErrorMessage('Both fields are mandatory to do search');
        return;
    }
    const overlay = document.getElementById("formLoaderOverlay");
    overlay.style.display = "flex";
    $.ajax({
        type: 'GET',
        url: _url,
        data: { CustomerId: selectedCustId, Empcode: $('#txtcode').val(), Lastworkingdate: $('#lastworkingDate').val() },
        contentType: "application/json",
        dataType: 'json', // changing data type to json
        success: function (data) {
            //console.log(data); 
            if (data == null || data.id == null) {
                ClearData();
                showErrorMessage('Employee details not found');
            }
            else {
                $('#employeeform').show();
                $('#id').val(data.id);
                $('#Candidatename').val(data.name);
                formatField('Candidatename');
                $('#Employeecode').val(data.employeecode);
                formatField('Employeecode');
                $('#Designation').val(data.designation);
                formatField('Designation');
                $('#Employmentperiod').val('Available');
                formatField('Employmentperiod');
                $('#Lastauthorizedby').val(data.authorizedby);
                formatField('Lastauthorizedby');
                if (data.authorizeddate != null) {
                    $('#Lastauthorizeddate').val(data.authorizeddate.substring(0, 10));
                }
                else {
                    $('#Lastauthorizeddate').val('');
                }
                formatField('Lastauthorizeddate');
                $('#hdnsearch').val(data.searchid);
                $('#isedit').val(data.isedit);
                if (data.isedit == true) {
                    //$('#GetEditApproval').show();
                    $('#editapprovalrow').show();
                }
                else {
                    //$('#GetEditApproval').hide();
                    $('#editapprovalrow').hide();
                }
                $('#GetApproval').show();
                $('#GenerateReport').show();
                //$('#RaiseRequest').hide();
            }
            overlay.style.display = "none";
        },
        error: function (err) {
            overlay.style.display = "none";
            showErrorMessage(err.statusText);
        }
    });
}
function SendApproval(empId, searchId, isEdit, approvalUrl) {
    const overlay = document.getElementById("formLoaderOverlay");
    overlay.style.display = "flex";
    $.ajax({
        type: 'GET',
        url: approvalUrl,
        data: { EmpId: empId, searchId: searchId, isedit: isEdit },
        contentType: "application/json",
        dataType: 'json', // changing data type to json
        success: function (data) { // here I'm adding data as a parameter which stores the response
            //console.log(data); // instead of alert I'm changing this to console.log which logs all the response in console.
            overlay.style.display = "none";
            if (data == true) {
                ClearData();
                showSuccessMessage('Approval request was sent to the employer');
            }
            else {
                showSuccessMessage('Error occured while sending the request to employer');
            }
        },
        error: function (err) {
            overlay.style.display = "none";
            console.log(err);
            showErrorMessage("Error: " + err.statusText);
        }
    });
}
function OpenClientPopup(_balanceurl, _reporturl) {
    if ($('#hdncategory').val() == true) {
        $('#clientname').modal('show');
    }
    else {
        $.ajax({
            type: 'GET',
            url: _balanceurl,
            dataType: 'json', // changing data type to json
            success: function (data) { // here I'm adding data as a parameter which stores the response
                //console.log(data); // instead of alert I'm changing this to console.log which logs all the response in console.
                if (data == 0)
                    showErrorMessage("Insufficient balance to generate report");
                else
                    GenerateReport(_reporturl);
                //$('#loading').hide();
            },
            error: function (err) {
                showErrorMessage(err.statusText);
            }
        });
    }
}
function GenerateReport(_reporturl, _balanceurl) {
    const overlay = document.getElementById("formLoaderOverlay");
    overlay.style.display = "flex";
    $.ajax({
        type: 'GET',
        url: _reporturl,
        data: { searchid: $('#hdnsearch').val(), clientname: $('#txtclientname').val() },
        contentType: "application/json",
        dataType: 'json', // changing data type to json
        success: function (data) { // here I'm adding data as a parameter which stores the response
            console.log(data); // instead of alert I'm changing this to console.log which logs all the response in console.
            if (data != null) {
                ShowBalance(_balanceurl);
                downloadReportPDF(data);
                overlay.style.display = "none";
                //window.open("/Company/DownloadFile?searchid=" + $('#hdnsearch').val(), '_blank');
                ClearData();
            }
            else {
                overlay.style.display = "none";
                showErrorMessage('Error occured while generating the employment report');
            }
        },
        error: function (err) {
            overlay.style.display = "none";
            showErrorMessage(err.statusText);
        }
    });
}

function BindCustomers(id, _url) {
    $.ajax({
        type: 'GET',
        url: _url,
        contentType: "application/json",
        dataType: 'json', // changing data type to json
        success: function (res) { // here I'm adding data as a parameter which stores the response
            //console.log(res); // instead of alert I'm changing this to console.log which logs all the response in console.\
            BindCustomerData(res, id);
        },
        error: function (err) {
            $('#loading').hide();
            alert(err.statusText);
        }
    });
}
function BindCustomerData(res, id) {
    const jsonData = [];
    var emptyjson = { id: "", name: "- Please choose customer -" };
    jsonData.push(emptyjson);
    for (let i = 0; i < res.length; i++) {
        var json = { id: res[i].id, name: res[i].name };
        jsonData.push(json);
    }

    const customerdropdownMenus = document.querySelectorAll("#" + id);

    customerdropdownMenus.forEach(dropmenu => {
        const input = dropmenu.closest(".inputContainer").querySelector("input");

        // Populate dropdown dynamically
        jsonData.forEach(item => {
            const div = document.createElement("div");
            div.className = "newinput-dropdown-item";
            div.innerHTML = `<i style="color:var(--purple-color)"></i> ${item.name}`;

            // On selecting an option
            div.addEventListener("click", () => {
                input.value = item.name;
                selectedCustId = item.id;
                dropmenu.classList.remove("show");
            });

            dropmenu.appendChild(div);
        });

        // Toggle dropdown on input click
        input.addEventListener("click", (e) => {
            e.stopPropagation();
            dropmenu.classList.toggle("show");
        });
    });

    // Close dropdown if clicked outside
    document.addEventListener("click", () => {
        customerdropdownMenus.forEach(dropmenu => dropmenu.classList.remove("show"));
    });
}