var industrytype = '';
var tableData = [];
const recordsPerPage = 10;
let currentPage = 1;
$(function () {
    BindIndustry();
});
function BindIndustry() {
    const monthdata = [{
        id: "",
        name: "Choose industry"
    }, {
        id: "1",
        name: "IT sector"
    }, {
        id: "2",
        name: "Non-IT sector"
    }, {
        id: "3",
        name: "Data analysis"
    }, {
        id: "4",
        name: "Data science"
    }];

    const monthdropdownMenus = document.querySelectorAll("#dropdownIndustryMenu");
    $('#dropdownIndustryMenu').empty();

    monthdropdownMenus.forEach(dropmenu => {
        const input = dropmenu.closest(".inputContainer").querySelector("input");

        // Populate dropdown dynamically
        monthdata.forEach(item => {
            const div = document.createElement("div");
            div.className = "newinput-dropdown-item";
            div.innerHTML = `<i style="color:var(--purple-color)"></i> ${item.name}`;
            if (item.id == industrytype) {
                input.value = item.name;
            }
            // On selecting an option
            div.addEventListener("click", () => {
                input.value = item.name;
                industrytype = item.id;
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
        monthdropdownMenus.forEach(dropmenu => dropmenu.classList.remove("show"));
    });
}
function SaveCustomer(_url) {
    //if (!$('#frmCustomer').valid()) return;
    // Show loader overlay
    const overlay = document.getElementById("formLoaderOverlay");
    overlay.style.display = "flex";
    var customerid = $('#Id').val();
    var type = 'POST';
    var requestData = $('#frmCustomer').serialize();
    
    if (customerid != '') {
        type = 'PUT';
        requestData = {
            id: $('#Id').val(),
            industry: industrytype,
            name: $('#Name').val(),
            address: $('#Address').val(),
            contactname: $('#Contactname').val(),
            email: $('#Email').val(),
            contactnumber: $('#Contactnumber').val(),
            commencementdate: $('#CommencementDate').val(),
            closeddate: $('#Closeddate').val(),
            gstnumber: $('#GstNumber').val(),
            tannumber: $('#TanNumber').val(),
            pannumber: $('#PanNumber').val(),
            iseducation: document.getElementById("check-box-edu").checked,
            isemployment: document.getElementById("check-box-emp").checked,
            isbgv: document.getElementById("check-box-bgv").checked
        }
    }
    else {
        requestData = {
            id: 0,
            industry: industrytype,
            name: $('#Name').val(),
            address: $('#Address').val(),
            contactname: $('#Contactname').val(),
            email: $('#Email').val(),
            contactnumber: $('#Contactnumber').val(),
            commencementdate: $('#CommencementDate').val(),
            closeddate: $('#Closeddate').val(),
            gstnumber: $('#GstNumber').val(),
            tannumber: $('#TanNumber').val(),
            pannumber: $('#PanNumber').val(),
            iseducation: document.getElementById("check-box-edu").checked,
            isemployment: document.getElementById("check-box-emp").checked,
            isbgv: document.getElementById("check-box-bgv").checked
        }
    }
    //console.log(requestData);
    $.ajax({
        url: _url,
        type: type,
        data: JSON.stringify(requestData),
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
        error: function (xhr, status, error) {
            overlay.style.display = "none";
            console.log(xhr);
            showErrorMessage('Error: ' + xhr.statusText);
        },
        success: function (result) {
            console.log(result);
            if (result == "true") {
                overlay.style.display = "none";
                clearFields();
                $('#adduser').modal('hide');
                // Show success toast message
                showSuccessMessage('Record saved');
                //$('#customerlist').show();
                //$('#customerform').hide();
                //$('#btnadd').show();
                BindCustomers();
            }
            else if (result == "exists") {
                overlay.style.display = "none";
                $('#adduser').modal('hide');
                showErrorMessage('Customer name already exists');
            }            
            else {
                overlay.style.display = "none";
                $('#adduser').modal('hide');
                showErrorMessage(result);
            }
        },
        async: true,
        processData: false
    });
}
function clearFields() {
    $('#staticBackdropLabel').text('Add Customer');
    $('#Id').val('');
    industrytype = '';
    BindIndustry();
    $('#Name').val('');
    $('#Address').val('');
    $('#Contactname').val('');
    $('#Email').val('');
    $('#Contactnumber').val('');
    $('#CommencementDate').val('');
    $('#ClosedDate').val('');
    $('#GstNumber').val('');
    $('#TanNumber').val('');
    $('#PanNumber').val('');
    document.getElementById("check-box-edu").checked = false;
    document.getElementById("check-box-emp").checked = false;
    document.getElementById("check-box-bgv").checked = false;
}
function BindCustomers(_url) {
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
                                "name": jsonData[i].name,
                                "address": jsonData[i].address,
                                "contactname": jsonData[i].contactname,
                                "contactnumber": jsonData[i].contactnumber,
                                "email": jsonData[i].email,
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
                    console.log(response);
                    showErrorMessage("Error: " + response.statusText);
                }
        }
    );
}

function EditCustomer(customerId, _url) {
    // Show loader overlay
    const overlay = document.getElementById("formLoaderOverlay");
    overlay.style.display = "flex";
    $.ajax({
        type: 'GET',
        dataType: 'JSON',
        url: _url,
        data: { id: customerId },
        success:
            function (response) {
                //console.log(response);                
                $('#adduser').modal('show');

                clearFields();
                const h1 = document.querySelectorAll("h1");
                h1[0].innerText = "Update Customer";
                $('#Id').val(response.id);
                industrytype = String(response.industry);
                BindIndustry();
                $('#Name').val(response.name);
                $('#Address').val(response.address);
                $('#Contactname').val(response.contactname);
                $('#Email').val(response.email);
                $('#Contactnumber').val(response.contactnumber);
                $('#CommencementDate').val(response.commencementdate);
                $('#Closeddate').val(response.closeddate);
                $('#GstNumber').val(response.gstnumber);
                $('#TanNumber').val(response.tannumber);
                $('#PanNumber').val(response.pannumber);
                if (response.iseducation == true) {
                    $('#check-box-edu').prop('checked', true);
                } else {
                    $('#check-box-edu').prop('checked', false);
                }
                if (response.isemployment == true) {
                    $('#check-box-emp').prop('checked', true);
                } else {
                    $('#check-box-emp').prop('checked', false);
                }
                if (response.isbgv == true) {
                    $('#check-box-bgv').prop('checked', true);
                } else {
                    $('#check-box-bgv').prop('checked', false);
                }
                overlay.style.display = "none";
            },
        error:
            function (response) {
                overlay.style.display = "none";
                showErrorMessage("Error: " + response);
            }
    });
};

function SetAction(id) {

    return '<div data-bs-toggle="modal" data-bs-target="#customerquestion"> ' +
        '<button class="manage-hr-pencil-question" data-bs-toggle="tooltip"' +
        'data-bs-placement="top" data-bs-custom-class="tooltip-delete" title="Question" style="color: blue;" >' +
        '<i class="ri-question-line"></i></button></div>' +
        '<div data-bs-toggle="modal" data-bs-target="#adduser" id="delid" data-id="' + id + '"> ' +
        '<button class="manage-hr-pencil-edit" data-bs-toggle="tooltip"' +
        'data-bs-placement="top" data-bs-custom-class="tooltip-delete" title="Edit"> ' +
        '<i class="ri-pencil-line "></i></button></div> ' +
        '<div data-bs-toggle="modal" data-bs-target="#deleteuser"> ' +
        '<button class="manage-hr-pencil-delete" data-bs-toggle="tooltip" ' +
        'data-bs-placement="top" data-bs-custom-class="tooltip-delete" title="Delete"> ' +
        '<i class="ri-delete-bin-6-line"></i></button></div>';

}
function renderTable(page = 1) {

    const tbody = document.querySelector("#tableIdx");
    if (!tbody) return;

    tbody.innerHTML = "";

    const start = (page - 1) * recordsPerPage;
    const end = start + recordsPerPage;
    const pageData = tableData.slice(start, end);
    let rows = '';
    pageData.forEach(item => {
        rows += `
            <tr>
                <td>${item.name}</td>
                <td>${item.address}</td>
                <td>${item.contactname}</td>
                <td>${item.contactnumber}</td>
                <td>${item.email}</td>
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