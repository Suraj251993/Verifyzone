var tableData = [];
const recordsPerPage = 10;
let currentPage = 1;

function renderTable(page = 1) {

    const tbody = document.querySelector("#tableIdx");
    if (!tbody) return;

    tbody.innerHTML = "";

    const start = (page - 1) * recordsPerPage;
    const end = start + recordsPerPage;
    const pageData = tableData.slice(start, end);
    tbody.insertAdjacentHTML("beforeend", "");
    pageData.forEach(item => {
        const row = `
                                <tr>
                                    <td>${item.empcode}</td>
                                    <td>${item.employeename}</td>
                                    <td>${item.approveddate}</td>
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

function changePage(page) {
    const totalPages = Math.ceil(tableData.length / recordsPerPage);
    if (page < 1) page = 1;
    if (page > totalPages) page = totalPages;

    currentPage = page;

    renderTable(currentPage);
}

function refreshGrid(_url) {
    if ($('#fromDate').val() == "" || $('#toDate').val() == "") {
        showErrorMessage("Both date filters are mandatory");
        return;
    }
    const overlay = document.getElementById("formLoaderOverlay");
    overlay.style.display = "flex";
    $.ajax({
        type: 'GET',
        data: { fromdate: $('#fromDate').val(), todate: $('#toDate').val() },
        dataType: 'JSON',
        url: _url,
        success:
            function (jsonData) {
                if (jsonData != null) {
                    for (let i = 0; i < jsonData.length; i++) {
                        tableData.push({
                            "empcode": jsonData[i].empcode,
                            "employeename": jsonData[i].employeename,
                            "approveddate": jsonData[i].approveddate
                        });
                    }
                    renderTable(currentPage);
                }
                else {

                }
                overlay.style.display = "none";
            },
        error:
            function (response) {
                overlay.style.display = "none";
                showErrorMessage("Error: " + response.statusText);
            }
    });
}
