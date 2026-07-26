var exittype = '';
var highlight = '';

$(function () {
    const fromPicker = flatpickr("#Fromdate", {
        dateFormat: "d-m-Y",
        onChange: function (selectedDates) {
            if (selectedDates.length === 0) return;

            let nextDay = new Date(selectedDates[0]);
            nextDay.setDate(nextDay.getDate() + 1);

            toPicker.set("minDate", nextDay);

            if (toPicker.selectedDates.length > 0 && toPicker.selectedDates[0] < nextDay) {
                toPicker.clear();
            }
        }
    });
    
    const toPicker = flatpickr("#Todate", {
        dateFormat: "d-m-Y"
    });
    
    if ($('#Mode').val() == "Add" || $('#Mode').val() == "AddNew") {
        $('#AuthorizedBy').hide();
        $('#AuthorizedDate').hide();
        $('#divalert').hide();
        if ($('#Mode').val() == "AddNew") {
            $('#btndraft').hide();
            $('#btndraft2').hide();
            $('#btnsubmits2').hide();
            if ($('.verify-button-modify') != null) {
                $('.verify-button-modify').hide();
            }
        }
        BindExittype();
        if ($('#Mode').val() == "Add") {
            BindHighlight();
        }
    }
    else if ($('#Mode').val() == "Approve") {
        if ($('#ExitType').val() != '') {
            exittype = $('#ExitType').val();
        }
        if ($('#Highlight').val() != '') {
            highlight = $('#Highlight').val();
        }
        BindExittype();
        BindHighlight();
        $('#divalert').hide();

        $('#btndraft').hide();
        $('#btndraft2').hide();
        $('#btnsubmits2').hide();
        if ($('#IsEdit').val() == "True") {
            if ($('.verify-button-modify') != null) {
                $('.verify-button-modify').show();
            }
            $('#btnapprove').hide();
        }
        else {
            $('#btnapprove').show();
            if ($('.verify-button-modify') != null) {
                $('.verify-button-modify').hide();
            }
        }
        
    }
    else if ($('#Mode').val() == "View") {
        $('#divalert').hide();

        $('#btndraft').hide();
        $('#btndraft2').hide();
        $('#btnsubmits2').hide();
        if ($('#ExitType').val() != '') {
            exittype = $('#ExitType').val();
        }
        if ($('#Highlight').val() != '') {
            highlight = $('#Highlight').val();
        }
        BindExittype();
        BindHighlight();
    }
    // Initialize all tooltips
    const tooltipTriggerList = document.querySelectorAll('[data-bs-toggle="tooltip"]');
    [...tooltipTriggerList].forEach(el => new bootstrap.Tooltip(el));

    function BindExittype() {    
        var l_exittype = $('#ExitType').val();
        const typeOfExit = [{
            id: "0",
            name: "Choose type of exit",
            icon: "ri-corner-down-right-line"
        }, {
            id: "1",
            name: "Voluntary",
            icon: "ri-file-user-line"
        }, {
            id: "2",
            name: "Involuntary",
            icon: "ri-folder-user-line"
        }];

        const dropdownMenus = document.querySelectorAll("#dropdownExitType");

        dropdownMenus.forEach(dropmenu => {
            const input = dropmenu.closest(".inputContainer").querySelector("input");

            // Populate dropdown dynamically
            typeOfExit.forEach(item => {
                const div = document.createElement("div");
                div.className = "newinput-dropdown-item";
                div.innerHTML = `<i class="${item.icon}" style="color:var(--purple-color)"></i> ${item.name}`;
                if (l_exittype != null && item.id == l_exittype) input.value = item.name;
                if (exittype != null && exittype != '' && item.id == exittype) {  // Setting the dropdown value on edit mode
                    input.value = item.name;
                }
                // On selecting an option
                div.addEventListener("click", () => {
                    input.value = item.name;
                    exittype = item.id;
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
            dropdownMenus.forEach(dropmenu => dropmenu.classList.remove("show"));
        });
    }

    function BindHighlight() {
    const highlightData = [{
        id: "0",
        name: "Please choose"
    }, {
        id: "1",
        name: "Abscond"
    }, {
        id: "2",
        name: "Stop comer"
    }, {
        id: "3",
        name: "No dues pending"
    }, {
        id: "4",
        name: "Harassment"
    }, {
        id: "5",
        name: "Theft"
    }, {
        id: "6",
        name: "Absenteeism"
    }, {
        id: "7",
        name: "Alcohol consumption at work"
    }, {
        id: "8",
        name: "Others"
    }];

    const dropdownMenus = document.querySelectorAll("#dropdownHighlight");

    dropdownMenus.forEach(dropmenu => {
        const input = dropmenu.closest(".inputContainer").querySelector("input");

        // Populate dropdown dynamically
        highlightData.forEach(item => {
            const div = document.createElement("div");
            div.className = "newinput-dropdown-item";
            div.innerHTML = `<i class="${item.icon}" style="color:var(--purple-color)"></i> ${item.name}`;
            if (highlight != null && highlight != '' && item.id == highlight) {  // Setting the dropdown value on edit mode
                input.value = item.name;
            }
            // On selecting an option
            div.addEventListener("click", () => {
                input.value = item.name;
                highlight = item.id;
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
        dropdownMenus.forEach(dropmenu => dropmenu.classList.remove("show"));
    });
    }

});