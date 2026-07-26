var tableData = [];
const recordsPerPage = 10;
let currentPage = 1;

function getData(_url) {
    if ($('#fromDate').val() == "" || $('#fromDate').val() == "") {
        showErrorMessage("Both filters are mandatory");
        return;
    }
    const overlay = document.getElementById("formLoaderOverlay");
    overlay.style.display = "flex";
    $.ajax({
        type: 'GET',
        data: { fromDate: $('#fromDate').val(), toDate: $('#toDate').val() },
        dataType: 'JSON',
        url: _url,
        success: function (jsonData) { // here I'm adding data as a parameter which stores the response
            //console.log(data); // instead of alert I'm changing this to console.log which logs all the response in console.
            if (jsonData == true) {
                overlay.style.display = "none";
                showSuccessMessage('Error occured while getting the report');
            }
            else {
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
                            "employeecode": jsonData[i].employeecode,
                            "name": jsonData[i].name,
                            "clientname": jsonData[i].clientname,
                            "customername": jsonData[i].customername,
                            "createddate": jsonData[i].createddate
                        });
                    }
                    renderTable(currentPage);
                }
                overlay.style.display = "none";
            }
            //$('#loading').hide();
        },
        error: function (err) {
            overlay.style.display = "none";
            console.log(err.statusText);
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
                <td>${item.employeecode}</td>
                <td>${item.name}</td>
                <td>${item.clientname}</td>
                <td>${item.customername}</td>
                <td>${item.createddate}</td>
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
