var customerid = '';
var usertype = '';
var searchusertype = '';
var accesslevel = '';
var category = '';
var tableData = [];
const recordsPerPage = 10;
let currentPage = 1;
let userid = 0;


function SaveUser(_url, _bindurl) {
    const overlay = document.getElementById("formLoaderOverlay");
    if ($('#DisplayName').val() == "" || $('#LoginName').val() == "" || $('#Emailid').val() == ""
        || $('#Contactnumber').val() == "" || $('#Designation').val() == ""
        || (usertype == '2' && customerid == "")
    ) {
        showErrorMessage("All fields are mandatory");
        return;
    }
    //if (!$('#frmUser').valid()) return;
    overlay.style.display = "flex";
    var type = 'POST';
    var requestData = $('#frmUser').serialize();
    requestData = requestData.replace('mdid', 'id');
    var userid = $('#Id').val();
    if (parseInt(userid) > 0) {
        type = 'PUT';
        requestData = {
            id: parseInt(userid),
            loginname: $('#LoginName').val(),
            displayname: $('#DisplayName').val(),
            usertype: parseInt(usertype),
            customertypeid: parseInt(accesslevel),
            customerid: parseInt(customerid),
            category: parseInt(category),
            emailid: $('#Emailid').val(),
            contactnumber: $('#Contactnumber').val(),
            designation: $('#Designation').val(),
        }
    }
    else {
        requestData = {
            id: 0,
            loginname: $('#LoginName').val(),
            displayname: $('#DisplayName').val(),
            usertype: parseInt(usertype),
            customertypeid: parseInt(accesslevel),
            customerid: parseInt(customerid),
            category: parseInt(category),
            emailid: $('#Emailid').val(),
            contactnumber: $('#Contactnumber').val(),
            designation: $('#Designation').val(),
        }
    }1
    
    $.ajax({
        url: _url,
        type: type,
        data: JSON.stringify(requestData),
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',        
        success: function (result) {            
            overlay.style.display = "none";
            $('#adduser').modal('hide');            
            if (result.success) {
                clearFields();

                if (type == 'PUT')
                    showSuccessMessage('User updated');
                else
                    showSuccessMessage('User created');

                var usertypeId = $('#searchusertype option:selected').val();
                if (usertypeId != 0 && usertypeId != '')
                    BindUsers(_bindurl);
            }
            else if (result.message === "exists") {
                showWarningMessage('Email Id already exists');
            }
            else {
                showErrorMessage(result.message || 'Error occurred');
            }
        },
        error: function (xhr) {
            console.log(xhr);
            overlay.style.display = "none";
            showErrorMessage('Error: ' + xhr.responseText);
        },
        async: true
    });
    $('#loading').hide();
}

    
function BindSearchUsertype() {
        const monthdata = [{
            id: "0",
            name: "Choose usertype"
        }, {
            id: "1",
            name: "Administrator"
        }, {
            id: "2",
            name: "Customer"
        }, {
            id: "3",
            name: "Institution"
        }, {
            id: "5",
            name: "Support team"
        }];

        const monthdropdownMenus = document.querySelectorAll("#dropdownSearchUsertypeMenu");

        monthdropdownMenus.forEach(dropmenu => {
            const input = dropmenu.closest(".inputContainer").querySelector("input");

            // Populate dropdown dynamically
            monthdata.forEach(item => {
                const div = document.createElement("div");
                div.className = "newinput-dropdown-item";
                div.innerHTML = `<i style="color:var(--purple-color)"></i> ${item.name}`;

                // On selecting an option
                div.addEventListener("click", () => {
                    input.value = item.name;
                    searchusertype = item.id;
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
    
function SetAction(id) {
        return '<div data-bs-toggle="modal" data-bs-target="#adduser" id="editid" data-id="' + id + '"> ' + 
            '<button class="manage-hr-pencil-edit" data-bs-toggle="tooltip"' +
            'data-bs-placement="top" data-bs-custom-class="tooltip-delete" title="Edit"> ' +
            '<i class="ri-pencil-line "></i></button></div> ' +
            '<div data-bs-toggle="modal" data-bs-target="#deleteuser" id="delid" data-id="' + id + '"> ' + 
            '<button class="manage-hr-pencil-delete" data-bs-toggle="tooltip"' +
            'data-bs-placement="top" data-bs-custom-class="tooltip-delete" title="Delete"> ' +
            '<i class="ri-delete-bin-6-line"></i></button></div> ' +
            '<div data-bs-toggle="modal" data-bs-target="#resetuser" id="resid" data-id="' + id + '"> ' +
            '<button class="manage-hr-pencil-reset" data-bs-toggle="tooltip"' +
            'data-bs-placement="top" data-bs-custom-class="tooltip-delete" title="Reset password"> ' +
            '<i class="ri-mail-send-line"></i></button></div>';
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
                    <td>${item.name}</td>
                    <td>${item.loginname}</td>
                    <td>${item.designation}</td>
                    <td>${item.email}</td>
                    <td>${item.contactnumber}</td>
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
                <a class="page-link-custom prev" href="#" aria-label="Previous" onclick="changePage(${currentPage - 1})">
                    <span aria-hidden="true"><i class="ri-arrow-left-s-line"></i></span>
                </a>
            </li>`;

        for (let i = 1; i <= totalPages; i++) {
            paginationHTML += `
            <li class="page-item-custom ${i === currentPage ? 'active' : ''}">
                <a class="page-link-custom" href="#" onclick="changePage(${i})">${i}</a>
            </li>`;
        }

        paginationHTML += `
            <li class="page-item-custom">
                <a class="page-link-custom next" href="#" aria-label="Next" onclick="changePage(${currentPage + 1})">
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

function addUser(cusurl) {
    $('#adduser').modal('show');
    clearFields();
    $('#staticBackdropLabel').text('Add User');
    BindUsertype(cusurl);
    //BindCustomers(cusurl);
    BindAccesslevel();
    BindCategory();
    $('#inputContainerCustomer').hide();
}
function BindUsers(_url) {
    if (searchusertype == '' || searchusertype == '0')
        showErrorMessage('Select Usertype ');
    else {
        $.ajax(
            {
                type: 'GET',
                dataType: 'JSON',
                url: _url,
                data: { usertypeId: searchusertype },
                success:
                    function (jsonData) {
                        // Generate HTML table.
                        tableData = [];
                        for (let i = 0; i < jsonData.length; i++) {
                            tableData.push({
                                "id": jsonData[i].id,
                                "name": jsonData[i].displayname,
                                "loginname": jsonData[i].loginname,
                                "designation": jsonData[i].designation,
                                "email": jsonData[i].emailid,
                                "contactnumber": jsonData[i].contactnumber
                            });
                        }
                        renderTable(currentPage);
                    },
                error:
                    function (response) {
                        showErrorMessage("Error: " + response.statusText);
                    }
            }
        );
    }
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
            $('#loading').hide();
            alert(err.statusText);
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

    const customerdropdownMenus = document.querySelectorAll("#dropdownCustomerMenu");
    
    customerdropdownMenus.forEach(dropmenu => {
        const input = dropmenu.closest(".inputContainer").querySelector("input");

        // Populate dropdown dynamically
        jsonData.forEach(item => {
            const div = document.createElement("div");
            div.className = "newinput-dropdown-item";
            div.innerHTML = `<i style="color:var(--purple-color)"></i> ${item.name}`;
            if (customerid != '' && item.id == customerid) {
                input.value = item.name;
            }
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
}
function ResetUser(_url) {   
    // Show loader overlay
    const overlay = document.getElementById("formLoaderOverlay");
    overlay.style.display = "flex";
    $.ajax({
        type: 'GET',
        dataType: 'JSON',
        url: _url,
        data: { id: userid.toString() },
        success:
            function (response) {
                overlay.style.display = "none";
                $('#resetuser').modal('hide');
                if (response == true) {
                    showSuccessMessage("The password was reset and sent to the respective user");
                }
                else {
                    showErrorMessage("Error occurred while reset the password. Please contact support");
                }
            },
        error:
            function (response) {
                //console.log(response);
                overlay.style.display = "none";
                showErrorMessage("Error: " + response.statusText);
            }
    });    
}

function BindAccesslevel() {
    const monthdata = [{
        id: "0",
        name: "Choose access level"
    }, {
        id: "1",
        name: "Ex-Zone"
    }, {
        id: "2",
        name: "V-Zone"
    }, {
        id: "3",
        name: "Both"
    }];

    const monthdropdownMenus = document.querySelectorAll("#dropdownAccessLevelMenu");
    $("#dropdownAccessLevelMenu").empty();
    monthdropdownMenus.forEach(dropmenu => {
        const input = dropmenu.closest(".inputContainer").querySelector("input");

        // Populate dropdown dynamically
        monthdata.forEach(item => {
            const div = document.createElement("div");
            div.className = "newinput-dropdown-item";
            div.innerHTML = `<i style="color:var(--purple-color)"></i> ${item.name}`;
            if (accesslevel != '' && item.id == accesslevel) {
                input.value = item.name;
            }
            // On selecting an option
            div.addEventListener("click", () => {
                input.value = item.name;
                accesslevel = item.id;
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
function BindCategory() {
    const monthdata = [{
        id: "0",
        name: "Choose category"
    }, {
        id: "1",
        name: "HR"
    }, {
        id: "2",
        name: "HR Manager"
    }, {
        id: "3",
        name: "BGV User"
    }];

    const monthdropdownMenus = document.querySelectorAll("#dropdownCategoryMenu");
    $("#dropdownCategoryMenu").empty();
    monthdropdownMenus.forEach(dropmenu => {
        const input = dropmenu.closest(".inputContainer").querySelector("input");

        // Populate dropdown dynamically
        monthdata.forEach(item => {
            const div = document.createElement("div");
            div.className = "newinput-dropdown-item";
            div.innerHTML = `<i style="color:var(--purple-color)"></i> ${item.name}`;
            if (category != null && category != '' && item.id == category) {
                input.value = item.name;
            }
            // On selecting an option
            div.addEventListener("click", () => {
                input.value = item.name;
                category = item.id;
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
function BindUsertype(_url) {
    const monthdata = [{
        id: "0",
        name: "Choose usertype"
    }, {
        id: "1",
        name: "Administrator"
    }, {
        id: "2",
        name: "Customer"
    }, {
        id: "3",
        name: "Institution"
    }, {
        id: "5",
        name: "Support team"
    }];

    const monthdropdownMenus = document.querySelectorAll("#dropdownUserTypeMenu");
    $("#dropdownUserTypeMenu").empty();
    monthdropdownMenus.forEach(dropmenu => {
        const input = dropmenu.closest(".inputContainer").querySelector("input");

        // Populate dropdown dynamically
        monthdata.forEach(item => {
            const div = document.createElement("div");
            div.className = "newinput-dropdown-item";
            div.innerHTML = `<i style="color:var(--purple-color)"></i> ${item.name}`;

            if (usertype != '' && item.id == usertype) {
                input.value = item.name;
            }
            // On selecting an option
            div.addEventListener("click", () => {
                input.value = item.name;
                usertype = item.id;
                if (item.id == "2") {
                    $('#inputContainerCustomer').show();
                    BindCustomers(_url);
                }
                else {
                    $('#inputContainerCustomer').hide();
                }
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
function clearFields() {
    userid = 0;
    $('#Id').val('');
    $('#DisplayName').val('');
    $('#LoginName').val('');
    $('#inputContainerCustomer').hide();
    $('#Emailid').val('');
    $('#Contactnumber').val('');
    $('#Designation').val('');
}
function EditUser(userId, _url, _cusurl) {
    const overlay = document.getElementById("formLoaderOverlay");
    overlay.style.display = "flex";
    clearFields();
    //console.log(document.getElementById('staticBackdropLabel'));
    document.getElementById('staticBackdropLabel').innerHTML = 'Edit User';
    $.ajax({
        type: 'GET',
        dataType: 'JSON',
        url: _url,
        data: { id: userId },
        success:
            function (response) {
                //console.log(response);
                overlay.style.display = "none";
                $('#Id').val(response.id);
                $('#DisplayName').val(response.displayname);
                $('#LoginName').val(response.loginname);
                $('#Emailid').val(response.emailid);
                $('#Contactnumber').val(response.contactnumber);
                $('#Designation').val(response.designation);
                usertype = response.usertype;
                accesslevel = response.customertypeid;
                category = response.category;
                customerid = response.customerid;
                if (response.usertype == "2") {
                    $('#inputContainerCustomer').show();
                }
                else {
                    $('#inputContainerCustomer').hide();
                }
                BindUsertype();
                BindCustomers(_cusurl);
                BindAccesslevel();
                BindCategory();
                //$('#mddusertype').val(response.usertype);                                
            },
        error:
            function (response) {
                overlay.style.display = "none";
                showErrorMessage("Error: " + response.statusText);
            }
    });
}
