var consentTableData = [];
const consentRecordsPerPage = 10;
let consentCurrentPage = 1;
var consentStatusId = 0;
var consentStatuses = [];

function InitConsentStatuses(_url) {
    $.ajax({
        type: 'GET',
        dataType: 'JSON',
        url: _url,
        success: function (data) {
            consentStatuses = data || [];
            const dropmenu = document.getElementById("dropdownStatus");
            const input = document.getElementById("txtsearchstatus");
            if (!dropmenu || !input) return;

            const options = [{ value: "0", text: "All statuses" }].concat(
                consentStatuses.map(s => ({ value: s.value, text: s.text }))
            );
            options.forEach(item => {
                const div = document.createElement("div");
                div.className = "newinput-dropdown-item";
                div.innerHTML = item.text;
                div.addEventListener("click", () => {
                    input.value = item.text;
                    consentStatusId = parseInt(item.value);
                    $("#hdnstatus").val(consentStatusId);
                    dropmenu.classList.remove("show");
                });
                dropmenu.appendChild(div);
            });
            input.addEventListener("click", (e) => {
                e.stopPropagation();
                dropmenu.classList.toggle("show");
            });
            document.addEventListener("click", () => dropmenu.classList.remove("show"));
        },
        error: function (err) {
            console.log(err);
        }
    });
}

function isValidEmailFormat(email) {
    return /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email);
}

function SendConsentRequest(_sendUrl, _listUrl) {
    const firstname = $.trim($('#Firstname').val());
    const lastname = $.trim($('#Lastname').val());
    const employeecode = $.trim($('#Employeecode').val());
    const employeeemail = $.trim($('#Employeeemail').val());

    if (firstname === "") {
        showErrorMessage("First name is required");
        return;
    }
    if (lastname === "") {
        showErrorMessage("Last name is required");
        return;
    }
    if (employeeemail === "" || !isValidEmailFormat(employeeemail)) {
        showErrorMessage("Please enter a valid employee email");
        return;
    }

    const overlay = document.getElementById("formLoaderOverlay");
    overlay.style.display = "flex";
    $.ajax({
        type: 'POST',
        url: _sendUrl,
        data: JSON.stringify({
            firstname: firstname,
            lastname: lastname,
            employeecode: employeecode,
            employeeemail: employeeemail
        }),
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        headers: { 'X-CSRF-TOKEN': csrfToken },
        success: function (result) {
            overlay.style.display = "none";
            if (result && result.success) {
                showSuccessMessage("Consent request " + result.consentrequestid + " sent successfully");
                $('#Firstname').val('');
                $('#Lastname').val('');
                $('#Employeecode').val('');
                $('#Employeeemail').val('');
                RefreshConsentList(_listUrl);
            } else {
                showErrorMessage((result && result.message) || "Error occured while sending the consent request");
            }
        },
        error: function (err) {
            overlay.style.display = "none";
            showErrorMessage("Error: " + err.statusText);
        }
    });
}

function RefreshConsentList(_url) {
    const overlay = document.getElementById("formLoaderOverlay");
    overlay.style.display = "flex";
    $.ajax({
        type: 'GET',
        dataType: 'JSON',
        url: _url,
        data: {
            name: $.trim($('#txtsearchname').val()),
            empcode: $.trim($('#txtsearchcode').val()),
            email: $.trim($('#txtsearchemail').val()),
            statusId: consentStatusId,
            fromdate: $.trim($('#fromDate').val()),
            todate: $.trim($('#toDate').val())
        },
        success: function (jsonData) {
            overlay.style.display = "none";
            consentTableData = jsonData || [];
            if (consentTableData.length === 0) {
                $('#divnorecord').show();
            } else {
                $('#divnorecord').hide();
            }
            consentCurrentPage = 1;
            renderConsentTable(1);
        },
        error: function (response) {
            overlay.style.display = "none";
            showErrorMessage("Error: " + response.statusText);
        }
    });
}

function renderConsentTable(page = 1) {
    const tbody = document.querySelector("#tableIdx");
    if (!tbody) return;
    tbody.innerHTML = "";

    const start = (page - 1) * consentRecordsPerPage;
    const end = start + consentRecordsPerPage;
    const pageData = consentTableData.slice(start, end);

    pageData.forEach(item => {
        const row = `
            <tr>
                <td>${item.consentrequestid}</td>
                <td>${item.employeename}</td>
                <td>${item.employeecode || ''}</td>
                <td>${item.employeeemail}</td>
                <td>${formatConsentDate(item.requestdate)}</td>
                <td>${item.status}</td>
                <td>${item.consentdate ? formatConsentDate(item.consentdate) : ''}</td>
                <td>${formatConsentDate(item.lastupdated)}</td>
                <td>${ConsentActions(item)}</td>
            </tr>
        `;
        tbody.insertAdjacentHTML("beforeend", row);
    });

    renderConsentPagination();
}

function ConsentActions(item) {
    if (item.statusid === 1) {
        return '<button class="action-btn consent-cancel-btn" data-bs-toggle="tooltip" data-bs-placement="top" data-bs-custom-class="tooltip-delete" title="Cancel request" style="color: #651305;" data-id="' + item.id + '"><i class="ri-close-circle-line"></i></button>';
    }
    return '';
}

function formatConsentDate(value) {
    if (!value) return '';
    const d = new Date(value);
    if (isNaN(d)) return value;
    return d.toLocaleDateString() + ' ' + d.toLocaleTimeString();
}

function renderConsentPagination() {
    const pagination = document.querySelector(".pagination-custom");
    if (!pagination) return;

    const totalPages = Math.ceil(consentTableData.length / consentRecordsPerPage);
    let html = `
        <li class="page-item-custom">
            <a class="page-link-custom prev" href="#" aria-label="Previous">
                <span aria-hidden="true"><i class="ri-arrow-left-s-line"></i></span>
            </a>
        </li>`;

    for (let i = 1; i <= totalPages; i++) {
        html += `
            <li class="page-item-custom ${i === consentCurrentPage ? 'active' : ''}">
                <a class="page-link-custom" href="#" data-page="${i}">${i}</a>
            </li>`;
    }

    html += `
        <li class="page-item-custom">
            <a class="page-link-custom next" href="#" data-page="${consentCurrentPage + 1}" aria-label="Next">
                <span aria-hidden="true"><i class="ri-arrow-right-s-line"></i></span>
            </a>
        </li>`;

    pagination.innerHTML = html;
}

function changeConsentPage(page) {
    const totalPages = Math.ceil(consentTableData.length / consentRecordsPerPage);
    if (page < 1) page = 1;
    if (page > totalPages) page = totalPages;
    consentCurrentPage = page;
    renderConsentTable(consentCurrentPage);
}

function CancelConsentRequest(id, _cancelUrl, _listUrl) {
    const overlay = document.getElementById("formLoaderOverlay");
    overlay.style.display = "flex";
    $.ajax({
        type: 'GET',
        url: _cancelUrl,
        data: { id: id },
        dataType: 'json',
        success: function (result) {
            overlay.style.display = "none";
            if (result === true) {
                showSuccessMessage("Consent request cancelled");
                RefreshConsentList(_listUrl);
            } else {
                showErrorMessage("Error occured while cancelling the request");
            }
        },
        error: function (err) {
            overlay.style.display = "none";
            showErrorMessage("Error: " + err.statusText);
        }
    });
}
