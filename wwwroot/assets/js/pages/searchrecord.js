var tableData = [];
const recordsPerPage = 10;
let currentPage = 1;



function BindDetail(_url) {
    if ($('#EmailId').val() == "" || $('#Mobile').val() == "" || $('#Name').val() == "" || $('#Uan').val() == "" || $('#Details').val() == "") {
        showErrorMessage("Atleast one field should be entered to do the search");
        return;
    }
    const overlay = document.getElementById("formLoaderOverlay");
    overlay.style.display = "flex";
    $.ajax(
        {
            type: 'GET',
            data: { email: $('#EmailId').val(), mobile: $('#Mobile').val(), name: $('#Name').val(), uan: $('#Uan').val(), others: $('#Details').val() },
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
                                "employeecode": jsonData[i].employeecode,
                                "customername": jsonData[i].customername,
                                "name": jsonData[i].name,
                                "joindate": jsonData[i].joindate,
                                "lastworkingdate": jsonData[i].lastworkingdate,
                                "linkedinurl": jsonData[i].linkedinurl,
                                "download": 'download'
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

    const start = (page - 1) * recordsPerPage;
    const end = start + recordsPerPage;
    const pageData = tableData.slice(start, end);
    let rows = '';
    pageData.forEach(item => {
        rows += `
            <tr>
                <td>${item.employeecode}</td>
                <td>${item.customername}</td>
                <td>${item.name}</td>
                <td>${item.joindate}</td>
                <td>${item.lastworkingdate}</td>
                <td>${SetAction(item.linkedinurl)}</td>
                <td></td>
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
function SetAction(url) {
    return '<a href="' + url + ')" target="new">' + url + '</a>';
}

function setDetailToTable(jsonData) {
    //console.log(jsonData);
    var data = [];
    for (let i = 0; i < jsonData.length; i++) {
        data.push({
            "employeecode": jsonData[i].employeecode,
            "customername": jsonData[i].customername,
            "name": jsonData[i].name,
            "joindate": jsonData[i].joindate,
            "lastworkingdate": jsonData[i].lastworkingdate,
            "linkedinurl": jsonData[i].linkedinurl,
            "download": 'download'
        });
        //arraydata.push(data);
    }
    var ajaxdata = {
        "data": data
    };

    var dt_ajax = $('#tablelist').dataTable({
        destroy: true,
        processing: true,
        data: ajaxdata.data,
        columns: [
            { data: 'customername' },
            { data: 'employeecode' },
            { data: 'name' },
            { data: 'joindate' },
            { data: 'lastworkingdate' },
            { data: 'linkedinurl' },
            { data: 'download' }
        ],
        columnDefs: [
            {
                // Label
                targets: 5,
                render: function (data, type, full, meta) {
                    var $url = full['linkedinurl'];
                    return (
                        '<a href="' + $url + ')" target="new">' + $url + '</a>'
                    );
                }
            },
            {
                // Label
                targets: 5,
                render: function (data, type, full, meta) {
                    var $id = full['id'];
                    return (
                        '<button type="button" class="btn rounded-pill btn-info waves-effect" onclick="">Download CV</button>'
                    );
                }
            },
        ],
        dom: '<"row"<"col-sm-12 col-md-6"l><"col-sm-12 col-md-6 d-flex justify-content-center justify-content-md-end"f>><"table-responsive"t><"row"<"col-sm-12 col-md-6"i><"col-sm-12 col-md-6"p>>',
        language: {
            paginate: {
                next: '<i class="ri-arrow-right-s-line"></i>',
                previous: '<i class="ri-arrow-left-s-line"></i>'
            }
        }
    });
}