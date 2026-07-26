var tableData = [];
const recordsPerPage = 10;
let currentPage = 1;
var editcustomerid = '';
var edittemplateid = '';
    
function saveSettings(_url) {
    var requestData = $('#msform').serialize();
    requestData = {
        templateid: $('#Templateid').val(),
        customerid: $('#Customerid').val(),
        templatecontent: $('#Templatecontent').val()
    }
    const overlay = document.getElementById("formLoaderOverlay");
    overlay.style.display = "flex";
    $.ajax({
        url: _url,
        type: 'POST',
        data: JSON.stringify(requestData),
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
        error: function (xhr) {
            overlay.style.display = "none";
            showErrorMessage('Error: ' + xhr.statusText);
        },
        success: function (result) {
            if (result == "true") {
                overlay.style.display = "none";
                clearFields();
                // Show success toast message
                showSuccessMessage('Email setting saved');
                BindEmailSettings();
            }
            else if (result == "exists") {
                overlay.style.display = "none";
                showErrorMessage('The selected template already exists');
            }
            else {
                overlay.style.display = "none";
                showErrorMessage(result);
            }
        },
        async: true,
        processData: false
    });
}

function clearFields() {
    $('#Templatecontent').val('');
    editcustomerid = '';
    edittemplateid = '';
}
function BindEmailTemplate() {
    var l_exittype = $('#ExitType').val();
    const typeOfExit = [{
        id: "0",
        name: "- Choose email template -",
        icon: "ri-corner-down-right-line"
    }, {
        id: "1",
        name: "Candidate Requisition Email Template",
        icon: "ri-file-user-line"
    }, {
        id: "2",
        name: "Candidate Re-verification Email Template",
        icon: "ri-folder-user-line"
    }, {
        id: "3",
        name: "Candidate Requisition Followup Email Template",
        icon: "ri-file-user-line"
    }, {
        id: "4",
        name: "Candidate Re-verification Followup Email Template",
        icon: "ri-folder-user-line"
    }];

    const dropdownMenus = document.querySelectorAll("#mddtemplate");

    dropdownMenus.forEach(dropmenu => {
        const input = dropmenu.closest(".inputContainer").querySelector("input");

        // Populate dropdown dynamically
        typeOfExit.forEach(item => {
            const div = document.createElement("div");
            div.className = "newinput-dropdown-item";
            div.innerHTML = `<i class="${item.icon}" style="color:var(--purple-color); text-align:left"></i> ${item.name}`;
            if (item.id == edittemplateid) {  // Setting the dropdown value on edit mode
                input.value = item.name;
            }

            // On selecting an option
            div.addEventListener("click", () => {
                input.value = item.name;
                $('#Templateid').val(item.id);
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
function BindCustomers(_url) {
    $.ajax({
        type: 'GET',
        url: _url,
        contentType: "application/json",
        dataType: 'json', // changing data type to json
        success: function (res) { // here I'm adding data as a parameter which stores the response
            //console.log(res); // instead of alert I'm changing this to console.log which logs all the response in console.\
            BindCustomerData(res);
        },
        error: function (err) {
            //$('#loading').hide();
            showErrorMessage(err.statusText);
            console.log(err);
        }
    });
}
function BindCustomerData(res) {
    const jsonData = [];
    var emptyjson = { id: "", name: "- Please choose customer -" };
    jsonData.push(emptyjson);
    for (let i = 0; i < res.length; i++) {
        var json = { id: res[i].id, name: res[i].name };
        jsonData.push(json);
    }

    const customerdropdownMenus = document.querySelectorAll("#mddcustomer");

    customerdropdownMenus.forEach(dropmenu => {
        const input = dropmenu.closest(".inputContainer").querySelector("input");

        // Populate dropdown dynamically
        jsonData.forEach(item => {
            const div = document.createElement("div");
            div.className = "newinput-dropdown-item";
            div.innerHTML = `<i style="color:var(--purple-color)"></i> ${item.name}`;
            if (item.id == editcustomerid) {  // Setting the dropdown value on edit mode
                input.value = item.name;
            }

            // On selecting an option
            div.addEventListener("click", () => {
                input.value = item.name;
                $('#Customerid').val(item.id);
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
function BindEmailSettings(_url) {
    const overlay = document.getElementById("formLoaderOverlay");
    overlay.style.display = "flex";
    $.ajax({
        type: 'GET',
        url: _url,
        contentType: "application/json",
        dataType: 'json', // changing data type to json
        success: function (jsonData) { // here I'm adding data as a parameter which stores the response
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
                    var templatename = '';
                    if (jsonData[i].templateid == 1)
                        templatename = "Candidate Requisition Email Template";
                    else if (jsonData[i].templateid == 2)
                        templatename = "Candidate Re-verification Email Template";
                    else if (jsonData[i].templateid == 3)
                        templatename = "Candidate Requisition Followup Email Template";
                    else if (jsonData[i].templateid == 4)
                        templatename = "Candidate Re-verification Followup Email Template";
                    tableData.push({
                        "templateid": templatename,
                        "customername": jsonData[i].customername,
                        "templatecontent": jsonData[i].templatecontent.replaceAll("\n", "<br>")
                    });
                }
                renderTable(currentPage);
            }
            overlay.style.display = "none";
        },
        error: function (err) {
            overlay.style.display = "none";
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
                <td>${item.templateid}</td>
                <td>${item.customername}</td>
                <td>${item.templatecontent}</td>
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