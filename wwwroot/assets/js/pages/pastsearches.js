var searchresult = '';
var tableData = [];
const recordsPerPage = 10;
let currentPage = 1;

$(function () {
    //$('#loading').hide();
    $('#divnorecord').hide();
    BindSearchOption(); 
});
function BindSearchOption() {
    const highlightData = [{
        id: "",
        name: "Please choose"
    }, {
        id: "Searched",
        name: "Searched"
    }, {
        id: "Sent for approval",
        name: "Sent for approval"
    }, {
        id: "Approved",
        name: "Approved"
    }, {
        id: "Generated",
        name: "Generated"
    }];

    const dropdownMenus = document.querySelectorAll("#dropdownMenu");

    dropdownMenus.forEach(dropmenu => {
        const input = dropmenu.closest(".inputContainer").querySelector("input");

        // Populate dropdown dynamically
        highlightData.forEach(item => {
            const div = document.createElement("div");
            div.className = "newinput-dropdown-item";
            div.innerHTML = `<i class="${item.icon}" style="color:var(--purple-color)"></i> ${item.name}`;
            
            // On selecting an option
            div.addEventListener("click", () => {
                input.value = item.name;
                searchresult = item.id;
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
        dropdownMenus.forEach(dropmenu => dropmenu.classList.remove("show"));
    });
}
function btnGenerateReport(_balanceurl, _reporturl, _searchurl) {
    if ($('#txtclientname').val() == '') {
        showErrorMessage('Please enter client name before generating the report');
        return;
    }
    const overlay = document.getElementById("formLoaderOverlay");
    overlay.style.display = "flex";
    $.ajax({
        type: 'GET',
        url: _balanceurl,
        dataType: 'json', // changing data type to json
        success: function (data) { // here I'm adding data as a parameter which stores the response
            //console.log(data); // instead of alert I'm changing this to console.log which logs all the response in console.
            if (data == 0)
                showErrorMessage("Insufficient balance to generate report");
            else
                GenerateReport($('#hdnsearch').val(), _reporturl, _searchurl, _balanceurl);
            overlay.style.display = "none";
        },
        error: function (err) {
            showErrorMessage(err.statusText);
        }
    });
}
function refreshGrid(_url) {
    const overlay = document.getElementById("formLoaderOverlay");
    overlay.style.display = "flex";

    $.ajax({
        type: 'GET',
        data: { fromdate: $('#fromDate').val(), todate: $('#toDate').val(), finalresult: searchresult },
        dataType: 'JSON',
        url: _url,
        success:
            function (jsonData) {
                // Generate HTML table.
                if (jsonData == null || jsonData.length == 0) {
                    tableData = [];
                    renderTable(1);
                    $('#divnorecord').show();
                }
                else {
                    $('#divnorecord').hide();
                    tableData = [];
                    for (let i = 0; i < jsonData.length; i++) {
                        tableData.push({
                            "id": jsonData[i].id,
                            "searchrequestid": jsonData[i].searchrequestid,
                            "employeecode": jsonData[i].employeecode,
                            "name": jsonData[i].name,
                            "customername": jsonData[i].customername,
                            "searchresult": jsonData[i].searchresult,
                            "createddate": jsonData[i].createddate,
                            "approveddate": jsonData[i].approveddate,
                            "finalresult": jsonData[i].finalresult,
                            "action": ''
                        });
                    }
                    renderTable(currentPage);
                }
                overlay.style.display = "none";
            },
        error:
            function (response) {
                overlay.style.display = "none";
                showErrorMessage("Error: " + response.statusText);
            }
    }
    );
}

function CaptureClient(id) {
    $('#hdnsearch').val(id);
    $('#clientname').modal('show');
}
function GenerateReport(id, _url, _searchurl, _balanceurl) {
    const overlay = document.getElementById("formLoaderOverlay");
    overlay.style.display = "flex";
    $.ajax({
        type: 'GET',
        url: _url,
        data: { searchid: id, clientname: $('#txtclientname').val() },
        contentType: "application/json",
        dataType: 'json', // changing data type to json
        success: function (data) { // here I'm adding data as a parameter which stores the response
            console.log(data); // instead of alert I'm changing this to console.log which logs all the response in console.
            if (data != null) {
                if ($('#hdncategory').val() == 'True') {
                    $('#clientname').modal('hide');
                }
                overlay.style.display = "none";
                downloadReportPDF(data);
                showSuccessMessage("Report generated successfully");
                ShowBalance(_balanceurl);
                refreshGrid(_searchurl);
            }
            else {
                overlay.style.display = "none";
                showSuccessMessage('Error occured while generating the employment report');
            }
        },
        error: function (err) {
            overlay.style.display = "none";
            showErrorMessage(err.statusText);
        }
    });
}
function DownloadReport(id) {
    window.open("/Customer/DownloadFile?searchid=" + id, '_blank');
}
function SendReminder(id, _url) {
    
    $.ajax({
        type: 'GET',
        url: _url,
        data: { searchid: id },
        contentType: "application/json",
        dataType: 'json', // changing data type to json
        success: function (data) { // here I'm adding data as a parameter which stores the response
            console.log(data); // instead of alert I'm changing this to console.log which logs all the response in console.
            if (data == true) {
                showSuccessMessage('Email reminder was sent successfully');
                //$('#loading').hide();
            }
            else {
                //$('#loading').hide();
                showErrorMessage('Error occured while sending the reminder email');
            }
        },
        error: function (err) {
            showErrorMessage(err.statusText);
        }
    });
}
function SetFinalResult(finalresult) {
    if (finalresult == 'Searched') return '<span class="bulk-table-upload"><i class="ri-history-line"></i> Searched</span>';
    else if (finalresult == 'Sent for approval') return '<span class="bulk-table-upload"><i class="ri-history-line"></i> Sent for approval</span>';
    else if (finalresult == 'Approved') return '<span class="bulk-table-approve"><i class="ri-checkbox-circle-line"></i> Approved</span>';
    else if (finalresult == 'Generated') return '<span class="bulk-table-approve"><i class="ri-checkbox-circle-line"></i> Report generated</span>';
    else return '';
}
function SetAction(row) {
    
    if (row.finalresult == "Searched" || row.finalresult == "Approved") {
        if ($('#hdncategory').val() == 'True') {
            return '<button class="action-btn new-request-table-delete-btn" data-bs-toggle="tooltip" data-bs-placement="top" data-bs-custom-class="tooltip-delete" title="Generate Report" style="color: #052c65;" data-id="' + row.id + '" data-action="capture"><i class="ri-article-line"></i></button>';
        } else {
            return '<button class="action-btn new-request-table-delete-btn" data-bs-toggle="tooltip" data-bs-placement="top" data-bs-custom-class="tooltip-delete" title="Generate Report" style="color: #052c65;" data-id="' + row.id + '" data-action="generate"><i class="ri-article-line"></i></button>';
        }
    }
    else if (row.finalresult == "Sent for approval") {
        return '<button class="action-btn new-request-table-delete-btn" data-bs-toggle="tooltip" data-bs-placement="top" data-bs-custom-class="tooltip-delete" title = "Send a reminder" style = "color: #651305;" data-id="' + row.id + '" data-action="reminder"><i class="ri-mail-send-line"></i></button> ';
    }
    else if (row.finalresult == "Generated") {
        return '';
        //const diffTime = Math.abs(new Date() - new Date(row.reportdate));
        //const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24));
        ////alert(diffDays);

        //if (row.finalresult == 'Generated' && diffDays > 4)
        //    return '';
        //else
        //    return '<button class="new-request-table-delete-btn" data-bs-toggle="tooltip" data-bs-placement="top" data-bs-custom-class="tooltip-delete" title="Download report" style="color: green;" onclick="DownloadReport(' + row.id + ')"><i class="ri-download-cloud-2-line"></i></button>';
    }
    else return '';
}

function renderTable(page = 1) {

    const tbody = document.querySelector("#tableIdx");
    if (!tbody) return;

    const start = (page - 1) * recordsPerPage;
    const end = start + recordsPerPage;
    const pageData = tableData.slice(start, end);
    let rows = '';
    pageData.forEach(item => {
        rows += `
            <tr>
                <td>${item.searchrequestid}</td>
                <td>${item.employeecode}</td>
                <td>${item.name}</td>
                <td>${item.customername}</td>
                <td>${item.searchresult}</td>
                <td>${item.createddate}</td>
                <td>${item.approveddate}</td>
                <td>${SetFinalResult(item.finalresult)}</td>
                <td>${SetAction(item)}</td>
            </tr>
        `;
        //tbody.insertAdjacentHTML("beforeend", row);
    });
    $('#tableIdx').empty();
    tbody.insertAdjacentHTML("beforeend", rows);
    renderPagination();
}
function renderPagination() {
    const pagination = document.querySelector(".pagination-custom");
    if (!pagination) return;

    const totalPages = Math.ceil(tableData.length / recordsPerPage);
    let paginationHTML = `
        <li class="page-item-custom">
            <a class="page-link-custom prev" href="#" aria-label="Previous">
                <span aria-hidden="true"><i class="ri-arrow-left-s-line"></i></span>
            </a>
        </li>`;

    for (let i = 1; i <= totalPages; i++) {
        paginationHTML += `
            <li class="page-item-custom ${i === currentPage ? 'active' : ''}">
                <a class="page-link-custom" href="#" data-page="${i}">${i}</a>
            </li>`;
    }

    paginationHTML += `
        <li class="page-item-custom">
            <a class="page-link-custom next" href="#" data-page="${currentPage + 1}" aria-label="Next">
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