var customerid = '';
var searchcustomerid = '';
var tableData = [];
const recordsPerPage = 10;
let currentPage = 1;

function BindSearchCustomers(_url) {
    $.ajax({
        type: 'GET',
        url: _url,
        contentType: "application/json",
        dataType: 'json', // changing data type to json
        success: function (res) { // here I'm adding data as a parameter which stores the response
            //console.log(res); // instead of alert I'm changing this to console.log which logs all the response in console.\
            const jsonData = [];
            var emptyjson = { id: "", name: "- Please choose customer -" };
            jsonData.push(emptyjson);
            for (let i = 0; i < res.length; i++) {
                var json = { id: res[i].id, name: res[i].name };
                jsonData.push(json);
            }

            const customerdropdownMenus = document.querySelectorAll("#mddsearchcustomer");

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
                        searchcustomerid = item.id;
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
        },
        error: function (err) {
            //$('#loading').hide();
            showErrorMessage(err.statusText);
        }
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
            const jsonData = [];
            var emptyjson = { id: "", name: "- Please choose customer -" };
            jsonData.push(emptyjson);
            for (let i = 0; i < res.length; i++) {
                var json = { id: res[i].id, name: res[i].name };
                jsonData.push(json);
            }

            const customerdropdownMenus = document.querySelectorAll("#dropdownCustomerMenu");

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
                        customerid = item.id;
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
        },
        error: function (err) {
            //$('#loading').hide();
            showErrorMessage(err.statusText);
        }
    });
}
function SaveCustomerCredit(_url) {

    // Show loader overlay
    const overlay = document.getElementById("formLoaderOverlay");
    overlay.style.display = "flex";

    var type = 'POST';
    var requestData = {
        id: 0,
        customerid: customerid,
        credit: parseInt($('#Credit').val()),
        transactiontype: $('#Transactiontype').val(),
        referenceno: $('#Referenceno').val(),
        remarks: $('#Remarks').val(),
    }

    $.ajax({
        url: _url,
        type: type,
        data: JSON.stringify(requestData),
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
        error: function (xhr) {
            showErrorMessage('Error: ' + xhr.statusText);
        },
        success: function (result) {
            if (result == true) {
                overlay.style.display = "none";
                clearFields();
                $('#adduser').modal('hide');
                // Show success toast message
                showSuccessMessage('Credit added');
                BindCredits(bindurl);
            }
            else {
                overlay.style.display = "none";
                $('#adduser').modal('hide');
                showErrorMessage('Error occurred while adding credits.');
            }
        },
        async: true,
        processData: false
    });
}

function clearFields() {    
    $('#Credit').val('');
    $('#Transactiontype').val('');
    $('#Referenceno').val('');
    $('#Remarks').val('');
}
function BindCredits(_url) {
    if (searchcustomerid == '') {
        showErrorMessage('Please choose customer');
        return;
    }
    // Show loader overlay
    const overlay = document.getElementById("formLoaderOverlay");
    overlay.style.display = "flex";
    $.ajax(
        {
            type: 'GET',
            data: { customerId: searchcustomerid },
            dataType: 'JSON',
            url: _url,
            success:
                function (jsonData) {
                    // Generate HTML table.
                    tableData = [];
                    for (let i = 0; i < jsonData.length; i++) {
                        tableData.push({
                            "customername": jsonData[i].customername,
                            "credit": jsonData[i].credit,
                            "transactiontype": jsonData[i].transactiontype,
                            "referenceno": jsonData[i].referenceno,
                            "creditdate": getDisplayDate(jsonData[i].creditdate)
                        });
                    }
                    renderTable(currentPage);
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
                <td>${item.customername}</td>
                <td>${item.credit}</td>
                <td>${item.transactiontype}</td>
                <td>${item.referenceno}</td>
                <td>${item.creditdate}</td>
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
function getDisplayDate(data) {
    var date = new Date(data);
    var month = date.getMonth() + 1;
    return date.getDate() + "/" + (month.toString().length > 1 ? month : "0" + month) + "/" + date.getFullYear();
}