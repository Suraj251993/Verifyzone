var industrytype = '';
var tableData = [];
const recordsPerPage = 10;
let currentPage = 1;

function saveQuestion(_url) {
    if ($('#Question').val() == "") {
        showErrorMessage("All fields are mandatory");
        return;
    }
    //if (!$('#frmQuestion').valid()) return;
    // Show loader overlay
    const overlay = document.getElementById("formLoaderOverlay");
    overlay.style.display = "flex";
    var companyid = $('#Id').val();
    var type = 'POST';
    var requestData = '';

    if (companyid != '') {
        type = 'PUT';
        requestData = {
            id: $('#Id').val(),
            question: $('#Question').val(),
        }
    }
    else {
        requestData = {
            id: 0,
            question: $('#Question').val(),
        }
    }
    //console.log(requestData);
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
            overlay.style.display = "none";
            if (result.success) {
                clearFields();
                // Show success toast message
                $('#addquestion').modal('hide');
                showSuccessMessage('Record saved');
                BindQuestions();
            }
            else if (result.message === "exists") {
                $('#addquestion').modal('hide');
                showErrorMessage('Question already exists');
            }
            else {
                $('#addquestion').modal('hide');
                showErrorMessage(result.statusText);
            }
        },
        async: true,
        processData: false
    });
}

function clearFields() {
    $('#staticBackdropLabel').text('Add Question');
    $('#Id').val('');
    $('#Question').val('');
}
function BindQuestions(_url) {
    // Show loader overlay
    const overlay = document.getElementById("formLoaderOverlay");
    overlay.style.display = "flex";
    $.ajax(
        {
            type: 'GET',
            dataType: 'JSON',
            url: _url,
            success:
                function (jsonData) {
                    tableData = [];
                    // Generate HTML table.
                    for (let i = 0; i < jsonData.length; i++) {
                        tableData.push({
                            "id": jsonData[i].value,
                            "question": jsonData[i].text
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

function EditQuestion(questionId, _url) {
    // Show loader overlay
    const overlay = document.getElementById("formLoaderOverlay");
    overlay.style.display = "flex";
    $.ajax({
        type: 'GET',
        dataType: 'JSON',
        url: _url,
        data: { id: questionId },
        success:
            function (response) {
                $('#staticBackdropLabel').text('Update Question');
                $('#Id').val(response.id);
                $('#Question').val(response.question);
                overlay.style.display = "none";
            },
        error:
            function (response) {
                overlay.style.display = "none";
                showErrorMessage("Error: " + response.statusText);
            }
    });
};
function SetAction(id) {

    return '<div data-bs-toggle="modal" data-bs-target="#addquestion" id="editid" data-id="' + id + '"> ' + 
        '<button class="manage-hr-pencil-edit" data-bs-toggle="tooltip"' +
        'data-bs-placement="top" data-bs-custom-class="tooltip-delete" title="Edit">' +
        '<i class="ri-pencil-line "></i></button></div>' +        
        '<div data-bs-toggle="modal" data-bs-target="#deletequestion"> ' +
        '<button class="manage-hr-pencil-delete" data-bs-toggle="tooltip" ' +
        'data-bs-placement="top" data-bs-custom-class="tooltip-delete" title="Delete"> ' +
        '<i class="ri-delete-bin-6-line"></i></button></div>';    
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
                <td>${item.question}</td>
                <td class="d-flex gap-2">${SetAction(item.id)}</td>
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