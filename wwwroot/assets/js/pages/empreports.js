const monthdropdownMenus = document.querySelectorAll("#month");
var tableData = [];
const recordsPerPage = 10;
let currentPage = 1;
var selectedmonth = '';
var selectedyear = '';
function BindMonthDropdown() {
    const monthdata = [{
        id: "0",
        name: "- Choose month -"
        }, {
        id: "1",
        name: "January"
        }, {
        id: "2",
        name: "February"
        }, {
            id: "3",
            name: "March"
        }, {
            id: "4",
            name: "April"
        }, {
            id: "5",
            name: "May"
        }, {
            id: "6",
            name: "June"
        }, {
            id: "7",
            name: "July"
        }, {
            id: "8",
            name: "August"
        }, {
            id: "9",
            name: "September"
        }, {
            id: "10",
            name: "October"
        }, {
            id: "11",
            name: "November"
        }, {
            id: "12",
            name: "December"
        }];

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
                selectedmonth = item.id;
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

function BindYearDropdown() {
    var year = new Date();

    const yeardata = [{
        id: 0,
        name: "- Choose year -"
    }, {
        id: year.getFullYear() - 2,
        name: (year.getFullYear() - 2).toString()
    }, {
        id: year.getFullYear() - 1,
        name: (year.getFullYear() - 1).toString()
    }, {
        id: year.getFullYear(),
        name: year.getFullYear().toString()
    }, {
        id: year.getFullYear() + 1,
        name: (year.getFullYear() + 1).toString()
    }];

    const yeardropdownMenus = document.querySelectorAll("#year");

    yeardropdownMenus.forEach(dropmenu => {
        const input = dropmenu.closest(".inputContainer").querySelector("input");

        // Populate dropdown dynamically
        yeardata.forEach(item => {
            const div = document.createElement("div");
            div.className = "newinput-dropdown-item";
            div.innerHTML = `<i style="color:var(--purple-color)"></i> ${item.name}`;

            // On selecting an option
            div.addEventListener("click", () => {
                input.value = item.name;
                selectedyear = item.id;
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
        yeardropdownMenus.forEach(dropmenu => dropmenu.classList.remove("show"));
    });
}

function getData(_url) {
    $.ajax({
        type: 'GET',
        url: _url,
        data: { month: selectedmonth, year: selectedyear },
        contentType: "application/json",
        dataType: 'json', // changing data type to json
        success: function (jsonData) { // here I'm adding data as a parameter which stores the response
            //console.log(data); // instead of alert I'm changing this to console.log which logs all the response in console.
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
                        "searchresult": jsonData[i].searchresult,
                        "createdbyname": jsonData[i].createdbyname,
                        "reportdate": jsonData[i].reportdate,
                        "clientname": jsonData[i].clientname
                    });
                }
                renderTable(currentPage);
            }
            //$('#loading').hide();
        },
        error: function (err) {
            //$('#loading').hide();
            showErrorMessage(err.statusText);
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
                <td>${item.searchresult}</td>
                <td>${item.createdbyname}</td>
                <td>${item.reportdate}</td>
                <td>${item.clientname}</td>
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