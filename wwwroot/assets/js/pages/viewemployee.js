var tableData = [];
const recordsPerPage = 10;
let currentPage = 1;

function exportEmployees(_url) {
    window.location.href = _url;
    showSuccessMessage("File exported successfully");
}
function excludeemployee(id, _url) {
    const overlay = document.getElementById("formLoaderOverlay");
    overlay.style.display = "flex";
    $.ajax({
        type: "GET",
        url: _url,
        data: { Id: id },
        contentType: "application/html; charset=utf-8",
        dataType: 'json', // changing data type to json
        success: function (data) { // here I'm adding data as a parameter which stores the response
            console.log(data); // instead of alert I'm changing this to console.log which logs all the response in console.
            //$('#divForm').html(data);
            if (data == "True") {
                showSuccessMessage("Employee record is excluded from auto approval.");
            }
            else if (data == "Exists") {
                showErrorMessage("Employee record was already excluded from auto approval.");
            }
            else {
                showErrorMessage("Error occurred while excluding from auto approval.");
            }
            overlay.style.display = "none";
        },
        error: function (err) {
            overlay.style.display = "none";
            showErrorMessage("Error: " + err.statusText);
        }
    });
}

function BindEmployees(_url) {

    const overlay = document.getElementById("formLoaderOverlay");
    overlay.style.display = "flex";
    $.ajax(
        {
            type: 'GET',
            dataType: 'JSON',
            async: false,
            url: _url,
            success:
                function (jsonData) {
                    if (jsonData == null || jsonData.length == 0) {
                        tableData = [];
                        renderTable(1);
                        $('#divnorecord').show();
                    }
                    else {
                        $('#divnorecord').hide();
                        for (let i = 0; i < jsonData.length; i++) {
                            tableData.push({
                                "id": jsonData[i].id,
                                "employeename": jsonData[i].name,
                                "employeecode": jsonData[i].employeecode,
                                "fromdate": jsonData[i].fromdate,
                                "todate": jsonData[i].todate,
                                "designation": jsonData[i].designation
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
                                    <td>${item.employeename}</td>
                                    <td>${item.employeecode}</td>
                                    <td>${SetAction(item.id)}</td>
                                    <td>${item.fromdate}</td>
                                    <td>${item.todate}</td>
                                    <td>${item.designation}</td>
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
function SetAction(id) {
    return '<button class="action-btn new-request-table-delete-btn" data-bs-toggle="tooltip" data-bs-placement="top" data-bs-custom-class="tooltip-delete" title = "Exclude from auto approval" style = "color: #651305;" data-id="' + id + '" data-action="capture"><i class="ri-article-line"></i></button> ';
}
function changePage(page) {
    const totalPages = Math.ceil(tableData.length / recordsPerPage);
    if (page < 1) page = 1;
    if (page > totalPages) page = totalPages;

    currentPage = page;

    renderTable(currentPage);
}