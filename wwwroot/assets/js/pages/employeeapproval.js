var exittype = '';
var highlight = '';
//document.querySelectorAll('.sidebar-items').forEach(item => {
//    item.addEventListener('click', () => {
//        item.parentElement.classList.toggle('open');
//    });
//});

function editemployee(_url) {
    const overlay = document.getElementById("formLoaderOverlay");
    overlay.style.display = "flex";
    //var url = "/Customer/SaveEmployeeQuestions";
    const form = document.getElementById('frmEmpForm');
    const data = new FormData(form);
    const empform = Object.fromEntries(data.entries());
    var questAns = [];
    $('#divQuestions input[type="text"]').each(function () {
        questAns.push({ questionId: $(this).attr('id'), answer: $(this).val() })
    });
    postdata = { ...empform, employeeQuestions: questAns };
    $.ajax({
        type: 'POST',
        url: _url,
        data: JSON.stringify(postdata),
        contentType: "application/json",
        dataType: 'json', // changing data type to json
        success: function (data) { // here I'm adding data as a parameter which stores the response
            //console.log(data); // instead of alert I'm changing this to console.log which logs all the response in console.
            overlay.style.display = "none";
            if (data == "true") {
                //clearFields();
                document.getElementById("frmEmpForm").reset();
                $('#divQuestions > div > div > div > textarea').each(function (index, value) {
                    $(this).val('');
                });
                $('#div' + id).hide();
                showSuccessMessage('Ex-employee record approved');
            }
            else {
                overlay.style.display = "none";
                showErrorMessage(data);
            }
        },
        error: function (err) {
            overlay.style.display = "none";
            console.log(err);
            showErrorMessage("Error: " + err.statusText);
        }
    });
}
function loademployee(empcode, searchid, _url) {
    const overlay = document.getElementById("formLoaderOverlay");
    overlay.style.display = "flex";
    $.ajax({
        type: "GET",
        url: _url,
        data: { Empcode: empcode.toString(), Mode: "Approve" },
        contentType: "application/html; charset=utf-8",
        dataType: 'json', // changing data type to json
        success: function (data) { // here I'm adding data as a parameter which stores the response
            console.log(data); // instead of alert I'm changing this to console.log which logs all the response in console.
            //$('#divForm').html(data);
            if (data.isedit == true) {
                $('#Name').attr('readonly', false);
                $('#Employeecode').attr('readonly', false);
                $('#Designation').attr('readonly', false);
                $('#Fromdate').attr('readonly', false);
                $('#Todate').attr('readonly', false);
                $('#Location').attr('readonly', false);
                $('#Jobtype').attr('readonly', false);
                $('#Lastdrawnsalary').attr('readonly', false);
                $('#Reportingto').attr('readonly', false);
                $('#Managerdesignation').attr('readonly', false);
            } else {
                $('#Name').attr('readonly', true);
                $('#Employeecode').attr('readonly', true);
                $('#Designation').attr('readonly', true);
                $('#Fromdate').attr('readonly', true);
                $('#Todate').attr('readonly', true);
                $('#Location').attr('readonly', true);
                $('#Jobtype').attr('readonly', true);
                $('#Lastdrawnsalary').attr('readonly', true);
                $('#Reportingto').attr('readonly', true);
                $('#Managerdesignation').attr('readonly', true);
            }            
            checkField("Name"); checkField("Employeecode"); checkField("Designation"); checkField("Fromdate"); checkField("Todate");
            checkField("Location"); checkField("Jobtype"); checkField("Lastdrawnsalary"); checkField("Reportingto"); checkField("Managerdesignation");            
            checkField("AuthorizedBy"); checkField("AuthorizedDate");

            $('#Id').val(data.id); $('#Mode').val(data.mode); $('#IsEdit').val(data.isedit);
            $('#Name').val(data.name); $('#Employeecode').val(data.employeecode); $('#Designation').val(data.designation);
            $('#Fromdate').val(data.fromdate); $('#Todate').val(data.todate); $('#Location').val(data.location); $('#Jobtype').val(data.jobtype);
            $('#Lastdrawnsalary').val(data.lastdrawnsalary); $('#Reportingto').val(data.reportingto); $('#Managerdesignation').val(data.managerdesignation);
            $('#Comments').val(data.comments); $('#AuthorizedBy').val(data.authorizedby); $('#AuthorizedDate').val(data.authorizeddate);
            exittype = data.exittype; highlight = data.highlight;
            $('#SearchId').val(searchid);
            
            appendquestions(data.employeeQuestions);
            
            $('#btnreject').hide();
            $('#btndraft').hide();
            $('#btndraft2').hide();
            $('#btndraft3').hide();
            $('#btnsubmit').hide();
            if ($('#Mode').val() == "Approve") {
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
                if (data.isedit == true) {
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
            overlay.style.display = "none";
        },
        error: function (err) {
            console.log(err.status);
            showErrorMessage("Error: " + err.statusText);
        }
    });
}
function approve(_url) {
    //var url = "/Customer/EmployeeApproval";
    var ids = [];
    ids.push($('#SearchId').val());
    const overlay = document.getElementById("formLoaderOverlay");
    overlay.style.display = "flex";

    $.ajax({
        type: 'POST',
        url: _url,
        data: { Ids: ids },
        dataType: 'json', // changing data type to json
        success: function (data) { // here I'm adding data as a parameter which stores the response
            //console.log(data); // instead of alert I'm changing this to console.log which logs all the response in console.
            if (data == 'True') {

                overlay.style.display = "none";
                $('#empViewModal').modal('hide');
                $('#div_' + $('#SearchId').val()).hide();
                showSuccessMessage('Approved successfully');
            }
            else {
                //$('#loading').hide();
                showErrorMessage(data);
            }
        },
        error: function (err) {
            showErrorMessage(err.statusText);
        }
    });
}
function appendquestions(questions) {
    //$("#divQuestions").empty();
    //var data = '';
    //var divcontent = '<div class="col-7"></div><div class="col-5"><h2 id="step2-label" class="steps">Step 2 - 2</h2></div>';
    for (let i = 0; i < questions.length; i++) {
        console.log(questions[i]);
        //divcontent += '<div class="col-md-6"><div class="inputContainer"><input id="qns_' + questions[i].questionId + '" class="input-container" autocomplete="off" placeholder="' + questions[i].questionname + '" value="' + questions[i].answer + '" type="text">';
        //divcontent += '<label class="usernameLabel">' + questions[i].questionname + '</label><i class="ri-creative-commons-by-line userIcon"></i></div></div> ';
        //data += divcontent;
        $('#qns_' + questions[i].questionId).val(questions[i].answer);
    }
    //$("#divQuestions").append(divcontent);
}
function checkField(id) {
    if ($('#' + id).val() == '') {
        $('#' + id).removeClass('filled');
        return true;
    }
    else {
        $('#' + id).addClass('filled');
        return false;
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