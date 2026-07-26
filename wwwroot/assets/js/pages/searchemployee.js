var exittype = '';
var highlight = '';

function loademployee(empcode, _url) {
    if (empcode == null || empcode == "") {
        showErrorMessage('Please enter employee code to search');
        return;
    }
    const overlay = document.getElementById("formLoaderOverlay");
    overlay.style.display = "flex";
    $.ajax({
        type: "GET",
        url: _url,
        data: { Empcode: empcode.toString(), Mode: "View" },
        contentType: "application/html; charset=utf-8",
        dataType: 'json', // changing data type to json
        success: function (data) { // here I'm adding data as a parameter which stores the response
            console.log(data); // instead of alert I'm changing this to console.log which logs all the response in console.
            //$('#divForm').html(data);
            if (data == null || data.name == "") {
                $('#divForm').hide();
                $('#btnexclude').hide();
                $('#divnorecord').show();
            }
            else {
                $('#divForm').show();
                $('#btnexclude').show();
                $('#divnorecord').hide();

                checkField("Name"); checkField("Employeecode"); checkField("Designation"); checkField("Fromdate"); checkField("Todate"); checkField("Reasonforleaving");
                checkField("Location"); checkField("Jobtype"); checkField("Lastdrawnsalary"); checkField("Reportingto"); checkField("Managerdesignation");
                checkField("AuthorizedBy"); checkField("AuthorizedDate");

                $('#Id').val(data.id); $('#Mode').val(data.mode); $('#IsEdit').val(data.isedit);
                $('#Name').val(data.name); $('#Employeecode').val(data.employeecode); $('#Designation').val(data.designation); $('#Fromdate').val(data.fromdate);
                $('#Todate').val(data.todate); $('#Reasonforleaving').val(data.reasonforleaving); $('#Location').val(data.location); $('#Jobtype').val(data.jobtype);
                $('#Lastdrawnsalary').val(data.lastdrawnsalary); $('#Reportingto').val(data.reportingto); $('#Managerdesignation').val(data.managerdesignation);
                $('#Comments').val(data.comments); $('#AuthorizedBy').val(data.authorizedby); $('#AuthorizedDate').val(data.authorizeddate);
                exittype = data.exittype; highlight = data.highlight;

                appendquestions(data.employeeQuestions);
                BindExittype();
                BindHighlight();
            }
            overlay.style.display = "none";
        },
        error: function (err) {
            overlay.style.display = "none";
            showErrorMessage(err.statusText);
        }
    });
}
function excludeemployee(id, _url) {
    const overlay = document.getElementById("formLoaderOverlay");
    overlay.style.display = "flex";
    $.ajax({
        type: "GET",
        url: _url,
        data: { Id: id },
        contentType: "application/html; charset=utf-8",
        dataType: 'json', // changing data type to json
        success: function (data) { // here I'm adding data as a parameter which stores the response
            console.log(data); // instead of alert I'm changing this to console.log which logs all the response in console.
            //$('#divForm').html(data);
            if (data == "True") {
                showSuccessMessage("Employee record is excluded from auto approval.");
            }
            else if (data == "Exists") {
                showErrorMessage("Employee record was already excluded from auto approval.");
            }
            else {
                showErrorMessage("Error occurred while excluding from auto approval.");
            }
            overlay.style.display = "none";
        },
        error: function (err) {
            overlay.style.display = "none";
            showErrorMessage("Error: " + err.statusText);
        }
    });
}
function appendquestions(questions) {
    $("#divQuestions").empty();
    var data = '';
    var divcontent = '<div class="col-7"></div><div class="col-5"><h2 id="step2-label" class="steps">Step 2 - 2</h2></div>';
    for (let i = 0; i < questions.length; i++) {
        console.log(questions[i]);
        divcontent += '<div class="col-md-6"><div class="inputContainer"><input id="qns_' + questions[i].questionId + '" class="input-container" autocomplete="off" placeholder="' + questions[i].questionname + '" value="' + questions[i].answer + '" type="text">';
        divcontent += '<label class="usernameLabel">' + questions[i].questionname + '</label><i class="ri-creative-commons-by-line userIcon"></i></div></div> ';
        //divcontent += '<input type="hidden" id="QQ-' + questions[i].questionId + "' name='QQ-" + questions[i].questionId + '" value="' + questions[i].questionId + '" />';
        //divcontent += '<input type="text" class="form-control" id="qt' + questions[i].questionId + '" name="qt' + questions[i].questionId + '" />';
        //divcontent += '<label for="qt' + questions[i].questionId + '">' + questions[i].questionname + '</label>';
        //divcontent += '</div>';
        data += divcontent;
    }
    //console.log(data);
    $("#divQuestions").append(divcontent);
}
function checkField(id) {
if ($('#' + id).val() == '') {
    $('#' + id).removeClass('filled');
}
else {
    $('#' + id).addClass('filled');
}
}

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