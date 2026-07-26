
function loademployee(empcode, customerid, _url) {
    const overlay = document.getElementById("formLoaderOverlay");
    overlay.style.display = "flex";
    $.ajax({
        type: "GET",
        url: _url,
        data: { Empcode: empcode.toString(), CustId: customerid.toString() },
        contentType: "application/html; charset=utf-8",
        dataType: 'json', // changing data type to json
        success: function (data) { // here I'm adding data as a parameter which stores the response
            console.log(data); // instead of alert I'm changing this to console.log which logs all the response in console.
            checkField("Name"); checkField("Employeecode"); checkField("Designation"); checkField("Fromdate"); checkField("Todate");
            checkField("Location"); checkField("Jobtype"); checkField("Lastdrawnsalary"); checkField("Reportingto"); checkField("Managerdesignation"); checkField("Comments");
            checkField("AuthorizedBy"); checkField("AuthorizedDate");

            $('#Id').val(data.id); $('#Mode').val(data.mode); $('#IsEdit').val(data.isedit);
            $('#Name').val(data.name); $('#Employeecode').val(data.employeecode); $('#Designation').val(data.designation);
            $('#Fromdate').val(data.fromdate); $('#Todate').val(data.todate); $('#Location').val(data.location); $('#Jobtype').val(data.jobtype);
            $('#Lastdrawnsalary').val(data.lastdrawnsalary); $('#Reportingto').val(data.reportingto); $('#Managerdesignation').val(data.managerdesignation);
            //$('#Comments').val(data.comments);
            $('#AuthorizedBy').val(data.authorizedby); $('#AuthorizedDate').val(data.authorizeddate);
            exittype = data.exittype; highlight = data.highlight;
            //$('#SearchId').val(searchid);

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
            else if ($('#Mode').val() == "AddNew") {
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
                $('#btnapprove').show();
                $('#btnreject').show();
                $('#btnmodify').hide();

            }
            overlay.style.display = "none";
        },
        error: function (err) {
            console.log(err.status);
        }
    });
}
function reject(id, _url, comments) {
    

    const overlay = document.getElementById("formLoaderOverlay");
    overlay.style.display = "flex";
    $.ajax({
        type: "GET",
        url: _url,
        data: { id: id, comments: comments },
        contentType: "application/json",
        dataType: 'json', // changing data type to json
        success: function (data) { // here I'm adding data as a parameter which stores the response
            //console.log(data); // instead of alert I'm changing this to console.log which logs all the response in console.
            overlay.style.display = "none";
            if (data == "True") {
                $("#empViewModal").modal("hide");
                showSuccessMessage('Request was replied');
                $('#div_' + id).hide();
            }
            else {
                $('#div_' + id).hide();
                showErrorMessage(data);
            }
        },
        error: function (err) {
            overlay.style.display = "none";
            showErrorMessage(err.statusText);
        }
    });
}
function approve(id, _url) {
    //var url = "/Customer/ApproveEmployeeRequest";
    const form = document.getElementById('divForm');
    const data = new FormData(form);
    const empform = Object.fromEntries(data.entries());
    
    empform.exittype = exittype;
    empform.highlight = highlight;
    
    var questAns = [];
    var isempty = true;
    $('#divQuestions input[type="text"]').each(function () {
        questAns.push({ questionId: $(this).attr('id'), answer: $(this).val() })
        if ($(this).val() != '') {
            isempty = false;
        }
    });
    if (isempty == true) {
        showErrorMessage("BGV questions should not be blank. Please fill more details.");
        return;
    }
    //console.log(empform);
    //console.log(questAns);
    postdata = { ...empform, employeeQuestions: questAns };

    const overlay = document.getElementById("formLoaderOverlay");
    overlay.style.display = "flex";
    $.ajax({
        type: 'POST',
        url: _url,
        data: JSON.stringify(postdata),
        contentType: "application/json",
        dataType: 'json', // changing data type to json
        success: function (data) { // here I'm adding data as a parameter which stores the response
            //console.log(data); // instead of alert I'm changing this to console.log which logs all the response in console.
            overlay.style.display = "none";
            if (data == 'true') {
                $("#empViewModal").modal("hide");
                showSuccessMessage('Request was approved');
                $('#div_' + id).hide();
            }
            else {
                $('#div_' + id).hide();
                showErrorMessage(data);
            }
        },
        error: function (err) {
            overlay.style.display = "none";
            showErrorMessage(err.statusText);
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