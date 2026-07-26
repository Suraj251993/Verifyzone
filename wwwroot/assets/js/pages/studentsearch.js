var tableData = [];
const recordsPerPage = 10;
let currentPage = 1;

$(function () {
    $('#studentlist').hide();
    //$('#studentform').hide();
    //$('#actionrow').hide();
    //$('#loading').hide();
    //$('#GetApproval').hide();
    //$('#GenerateReport').hide();
    BindInstitutions("institutiondropdownMenu");

    $('#GetStu').click(function (e) {
        e.preventDefault();
        
        var url = "/Customer/GetStudentDetails";
        if (selectedInstitutionId == '' || $('#txtcode').val() == '') {
            showErrorMessage('Both fields are mandatory to do search');
            return;
        }

        //$('#loading').show();
        $.ajax({
            type: 'GET',
            url: url,
            data: { custid: selectedInstitutionId, studentid: $('#txtcode').val() },
            contentType: "application/json",
            dataType: 'json', // changing data type to json
            success: function (jsonData) { // here I'm adding data as a parameter which stores the response
                //console.log(data); // instead of alert I'm changing this to console.log which logs all the response in console.
                if (jsonData == null || jsonData.length == 0) {
                    $('#studentlist').hide();
                    tableData = [];
                    renderTable(1);
                    $('#divnorecord').show();
                }
                else {
                    $('#divnorecord').hide();
                    $('#studentlist').show();                    
                    for (let i = 0; i < jsonData.length; i++) {
                        tableData.push({
                            "studentname": jsonData[i].studentname,
                            "studentid": jsonData[i].studentid,
                            "degreetype": jsonData[i].degreetype,
                            "majorsubject": jsonData[i].majorsubject,
                            "educationperiod": jsonData[i].educationperiod,
                            "action": "Choose"
                        });
                    }
                    renderTable(currentPage);
                }

                //if (rowcontent == "") {
                $('#actionrow').show();
                $('#RaiseRequest').show();
                //}
                $('#loading').hide();
            },
            error: function (err) {
                console.log(err);
            }
        });
    });
    //$('#GenerateReport').click(function (e) {
    //    e.preventDefault();
    //    GenerateReport();
    //});
    //$('#GetApproval').click(function (e) {
    //    e.preventDefault();
    //    SendApproval($('#id').val(), $('#hdnsearch').val(), "false");
    //});
    ShowBalance();
    //$('#btnsturaiserequest').click(function (e) {
    //    e.preventDefault();
    //    //console.log('hdnsearch: ' + $('#hdnsearch').val()); $('#advancedAutoCompleteSelected').val();
    //    var custId = $('#mddsearchinstitution').val();
    //    var searchId = 0;
    //    if ($('#hdnsearch').val() != null && $('#hdnsearch').val() != "")
    //        searchId = parseInt($('#hdnsearch').val());
    //    requestData = {
    //        searchid: searchId,
    //        regno: $.trim($('#txtsturegno').val()),
    //        customerid: parseInt(custId),
    //        requestcomments: $.trim($('#txtstudentdetails').val()),
    //    }
    //    //console.log(requestData);
    //    $.ajax({
    //        url: '@Url.Action("StudentRaiseRequest", "Customer")',
    //        type: 'POST',
    //        data: JSON.stringify(requestData),
    //        dataType: 'json',
    //        contentType: 'application/json; charset=utf-8',
    //        error: function (xhr) {
    //            alert('Error: ' + xhr.statusText);
    //        },
    //        success: function (result) {
    //            if (result == true) {
    //                $("#sturaiserequestModal").modal('hide');
    //                $('#txtsturegno').val('');
    //                $('#txtstudentdetails').val('');
    //                ClearData();
    //                // Show success toast message
    //                showSuccessMessage('Request raised successfully');
    //            }
    //            else {
    //                showErrorMessage('Error occured while raising request');
    //            }
    //        },
    //        async: true,
    //        processData: false
    //    });
    //});
    
});

function renderTable(page = 1) {

    const tbody = document.querySelector("#tableIdx");
    if (!tbody) return;

    tbody.innerHTML = "";

    const start = (page - 1) * recordsPerPage;
    const end = start + recordsPerPage;
    const pageData = tableData.slice(start, end);

    pageData.forEach(item => {
        const row = `
            <tr>
                <td>${item.studentname}</td>
                <td>${item.studentid}</td>
                <td>${item.degreetype}</td>
                <td>${item.majorsubject}</td>
                <td>${item.educationperiod}</td>
                <td><span class="forget-pass-text" data-bs-toggle="modal" data-bs-target="#forgetpopup">View detail</span></td>
            </tr>
        `;
        tbody.insertAdjacentHTML("beforeend", row);
    });

    renderPagination();
}
function renderPagination() {
    const pagination = document.querySelector(".pagination-custom");
    if (!pagination) return;

    const totalPages = Math.ceil(tableData.length / recordsPerPage);
    let paginationHTML = `
        <li class="page-item-custom">
            <a class="page-link-custom prev" href="#" aria-label="Previous" onclick="changePage(${currentPage - 1})">
                <span aria-hidden="true"><i class="ri-arrow-left-s-line"></i></span>
            </a>
        </li>`;

    for (let i = 1; i <= totalPages; i++) {
        paginationHTML += `
        <li class="page-item-custom ${i === currentPage ? 'active' : ''}">
            <a class="page-link-custom" href="#" onclick="changePage(${i})">${i}</a>
        </li>`;
    }

    paginationHTML += `
        <li class="page-item-custom">
            <a class="page-link-custom next" href="#" aria-label="Next" onclick="changePage(${currentPage + 1})">
                <span aria-hidden="true"><i class="ri-arrow-right-s-line"></i></span>
            </a>
        </li>`;

    pagination.innerHTML = paginationHTML;
}
function changePage(page) {
    const totalPages = Math.ceil(tableData.length / recordsPerPage);
    if (page < 1) page = 1;
    if (page > totalPages) page = totalPages;

    currentPage = page;

    renderTable(currentPage);
}

function GetRecord(id) {
    $('#loading').show();
    $.ajax({
        type: 'GET',
        dataType: 'JSON',
        url: '@Url.Action("GetStudentById", "Customer")',
        data: { id: id },
        success:
            function (response) {
                $('#studentform').show();
                $('#actionrow').show();
                $('#id').val(response.id);
                $('#lblstudentname').val(response.studentname);
                $('#lblstudentid').val(response.studentid);
                $('#lbldegreetype').val(response.degreetype);
                $('#lblmajorsubject').val(response.majorsubject);
                $('#lblperiodfrom').val(response.periodfrom);
                $('#lblperiodto').val(response.periodto);
                $('#lblauthorizedby').val(response.authorizedby);
                $('#lblauthorizeddate').val(response.authorizeddate);
                $('#hdnsearch').val(response.searchid);
                $('#RaiseRequest').hide();
                $('#GetApproval').show();
                $('#GenerateReport').show();
                $('#loading').hide();
            },
        error:
            function (response) {
                $('#loading').hide();
                alert("Error: " + response);
            }
    });
}
function ClearData() {
    $('#studentlist').hide();
    $('#studentform').hide();
    $('#actionrow').hide();
    $('#hdnsearch').val('');
    $('#id').val('');
    $('#lblstudentname').val('');
    $('#divname').removeClass('focused');
    $('#lblstudentid').val('');
    $('#divid').removeClass('focused');
    $('#lbldegreetype').val('');
    $('#divdegreetype').removeClass('focused');
    $('#lblmajorsubject').val('');
    $('#divmajorsubject').removeClass('focused');
    $('#lblperiodfrom').val('');
    $('#divfrom').removeClass('focused');
    $('#lblperiodto').val('');
    $('#divto').removeClass('focused');
    $('#lblauthorizedby').val('');
    $('#divauthorizedby').removeClass('focused');
    $('#lblauthorizeddate').val('');
    $('#divauthorizeddate').removeClass('focused');
    $('#GetApproval').hide();
    $('#GenerateReport').hide();

    $('#mddsearchinstitution').val('').selectpicker("refresh");
    $('#txtcode').val('');
    $('#divname').removeClass("focused");
}
function GenerateReport() {
    var custId = $('#mddsearchinstitution').val();
    var url = '@Url.Action("GenerateStudentReportBySearchId", "Customer")';
    $('#loading').show();
    $.ajax({
        type: 'GET',
        url: url,
        data: { searchid: $('#hdnsearch').val(), stuid: $('#id').val(), custid: custId, studentid: $('#txtcode').val() },
        contentType: "application/json",
        dataType: 'json', // changing data type to json
        success: function (data) { // here I'm adding data as a parameter which stores the response
            //console.log(data); // instead of alert I'm changing this to console.log which logs all the response in console.
            if (data == true) {
                ShowBalance();
                $('#loading').hide();
                window.open("/Customer/DownloadStudentFile?searchid=" + $('#hdnsearch').val(), '_blank');
                ClearData();
            }
            else {
                showSuccessMessage('Error occured while generating report');
            }
        },
        error: function (err) {
            console.log(err);
        }
    });
}
function SendApproval(stuId, searchId, isEdit) {
    var url = '@Url.Action("GetStudentApproval", "Customer")';
    $('#loading').show();
    $.ajax({
        type: 'GET',
        url: url,
        data: { stuId: stuId, searchId: searchId, isedit: "false" },
        contentType: "application/json",
        dataType: 'json', // changing data type to json
        success: function (data) { // here I'm adding data as a parameter which stores the response
            //console.log(data); // instead of alert I'm changing this to console.log which logs all the response in console.
            if (data == true) {
                ClearData();
                showSuccessMessage('Re-Verify sent to institution');
            }
            else {
                showSuccessMessage('Error occured while sending Re-Verify to institution');
            }
            $('#loading').hide();
        },
        error: function (err) {
            $('#loading').hide();
            console.log(err);
        }
    });
}