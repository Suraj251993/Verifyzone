var tableData = [];
const recordsPerPage = 10;
let currentPage = 1;
var deleteid = '';


function clearFields() {
    $('#fromDate').val('');
    $('#toDate').val('');
}
function BindConfig(_url) {

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
                        tableData = [];
                        for (let i = 0; i < jsonData.length; i++) {
                            tableData.push({
                                "id": jsonData[i].id,
                                "startdate": jsonData[i].startdate,
                                "enddate": jsonData[i].enddate
                            });
                        }
                        renderTable(currentPage);
                        overlay.style.display = "none";
                    }
                },
            error:
                function (response) {
                    overlay.style.display = "none";
                    showErrorMessage("Error: " + response.statusText);
                }
        }
    );
    //$('#loading').hide();
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
                                    <td>${item.startdate}</td>
                                    <td>${item.enddate}</td>
                                    <td>${SetAction(item.id)}</td>
                                </tr>
                            `;
        //tbody.insertAdjacentHTML("beforeend", row);
    });
    $('#tableIdx').empty();
    tbody.insertAdjacentHTML("beforeend", rows);
    renderPagination();
}
function SetAction(id) {
    return '<div data-bs-toggle="modal" data-bs-target="#deleteuser" id="delid" data-id="' + id + '"> ' + 
        '<button class="manage-hr-pencil-delete" data-bs-toggle="tooltip"' +
        'data-bs-placement="top" data-bs-custom-class="tooltip-delete" title="Delete"> ' +
        '<i class="ri-delete-bin-6-line"></i></button></div> ' +
        '<div> ';
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
function SetDeleteApproval(id) {
    deleteid = id;
}
function DeleteApproval(_url) {
    //$('#loading').show();
    $.ajax({
        type: 'GET',
        dataType: 'JSON',
        url: _url,
        data: { id: deleteid },
        success:
            function (response) {
                $('#deleteuser').modal('hide');
                showSuccessMessage("Record deleted successfully");
                BindConfig();
                //$('#loading').hide();
            },
        error:
            function (response) {
                //$('#loading').hide();
                showErrorMessage("Error: " + response.statusText);
            }
    });
};

function SaveConfig(_url) {
    var requestData = {
        startdate: $('#fromDate').val(),
        enddate: $('#toDate').val(),
    }
    postdata = { requestData };
    //console.log(postdata);
    //console.log(requestData);
    const overlay = document.getElementById("formLoaderOverlay");
    overlay.style.display = "flex";
    $.ajax({
        url: _url,
        type: 'POST',
        data: JSON.stringify(requestData),
        dataType: 'json',
        contentType: 'application/json;',
        error: function (xhr) {
            alert('Error: ' + xhr.statusText);
        },
        success: function (result) {
            if (result == true) {
                overlay.style.display = "none";
                clearFields();
                // Show success toast message
                showSuccessMessage('Auto approval configured successfully');
                BindConfig();
            }
            else {
                overlay.style.display = "none";
                showErrorMessage('Error occured while configuring Auto approval');
            }
        },
        async: true,
        processData: false
    });
}