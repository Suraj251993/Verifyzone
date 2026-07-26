var searchresult = "";
var tableData = [];
const recordsPerPage = 10;
let currentPage = 1;

//document.querySelectorAll('.sidebar-items').forEach(item => {
//    item.addEventListener('click', () => {
//        item.parentElement.classList.toggle('open');
//    });
//});

$(function () {
    $('#divnorecord').hide();
    BindResult();
});

function BindResult() {
    const typeOfExit = [{
        id: "",
        name: "Choose option",
        icon: "ri-corner-down-right-line"
        }, {
            id: "Open",
            name: "Open",
            icon: "ri-file-user-line"
        }, {
            id: "Approved",
            name: "Approved",
            icon: "ri-checkbox-circle-line"
        }, {
            id: "Rejected",
            name: "Rejected",
            icon: "ri-close-circle-line"
        }, {
            id: "Generated",
            name: "Generated",
            icon: "ri-file-list-3-line"
        }];

    const dropdownMenus = document.querySelectorAll("#searchresultdropdownMenu");

    dropdownMenus.forEach(dropmenu => {
        const input = dropmenu.closest(".inputContainer").querySelector("input");

        // Populate dropdown dynamically
        typeOfExit.forEach(item => {
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

function refreshGrid(_url) {
    if (searchresult == "" && $('#requestnumber').val() == "") {
        showErrorMessage("Any 1 filter is mandatory to search");
        if ($('#requestnumber').val() == "")
            $('#requestnumber').focus();
        return;
    }
    const overlay = document.getElementById("formLoaderOverlay");
    overlay.style.display = "flex";
    $.ajax({
        type: 'GET',
        data: { requestnumber: $('#requestnumber').val(), finalresult: searchresult },
        dataType: 'JSON',
        url: _url,
        success:
            function (jsonData) {
                overlay.style.display = "none";
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
                            "requestnumber": jsonData[i].comments,
                            "employeecode": jsonData[i].employeecode,
                            "employeename": jsonData[i].name,
                            "customername": jsonData[i].customername,
                            "searchresult": jsonData[i].location
                        });
                    }
                    renderTable(currentPage);
                }                
            },
        error:
            function (response) {
                console.log(response);
                overlay.style.display = "none";
                //alert("Error: " + response.statusText);
            }
    });
}


function SetAction(row) {

    if (row != null && (row.searchresult == "Approved" || row.searchresult == "Rejected")) {
        return '<button class="action-btn new-request-table-delete-btn" data-bs-toggle="tooltip" data-bs-placement="top" data-bs-custom-class="tooltip-delete" title="Generate Report" style="color: #052c65;" data-id="' + row.id + '" data-action="generate"><i class="ri-article-line"></i></button>';
    }
    else if (row.searchresult == "Open") {
        return '<button class="action-btn new-request-table-delete-btn" data-bs-toggle="tooltip" data-bs-placement="top" data-bs-custom-class="tooltip-delete" title = "Send a reminder" style = "color: #651305;" data-id="' + row.id + '" data-action="reminder"><i class="ri-mail-send-line"></i></button> ';
    }
    else return '';
}
function GenerateReportRequest(id, _reporturl, _searchurl) {
    const overlay = document.getElementById("formLoaderOverlay");
    overlay.style.display = "flex";
    $.ajax({
        type: 'GET',
        url: _reporturl,
        data: { id: id },
        contentType: "application/json",
        dataType: 'json', // changing data type to json
        success: function (data) { // here I'm adding data as a parameter which stores the response
            console.log(data); // instead of alert I'm changing this to console.log which logs all the response in console.
            if (data != null) {
                overlay.style.display = "none";
                if (data.status == "Approved")
                    downloadReportPDF(data);
                else if (data.status == "Rejected")
                    downloadNegativeReportPDF(data);
                showSuccessMessage("Report generated successfully");
                refreshGrid(_searchurl);
            }
            else {
                overlay.style.display = "none";
                showSuccessMessage('Error occured while generating the employment report');
            }
        },
        error: function (err) {
            showErrorMessage(err.statusText);
        }
    });
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
                <td>${item.requestnumber}</td>
                <td>${item.employeecode}</td>
                <td>${item.employeename}</td>
                <td>${item.customername}</td>
                <td>${item.searchresult}</td>
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

function GenerateReport(id) {
    var url = "Customer/GenerateReportByRequestId";
    const overlay = document.getElementById("formLoaderOverlay");
    overlay.style.display = "flex";
    $.ajax({
        type: 'GET',
        url: url,
        data: { id: id },
        contentType: "application/json",
        dataType: 'json', // changing data type to json
        success: function (data) { // here I'm adding data as a parameter which stores the response
            //console.log(data); // instead of alert I'm changing this to console.log which logs all the response in console.
            if (data == true) {
                showSuccessMessage('Report generated successfully');
                overlay.style.display = "none";
                window.open("/Customer/DownloadRequestReportFile?id=" + id, '_blank');
                refreshGrid();
            }
            else {
                overlay.style.display = "none";
                showErrorMessage('Error occured while generating report');
            }
        },
        error: function (err) {
            overlay.style.display = "none";
            showErrorMessage(err.statusText);
        }
    });
}