var exittype = '';
var highlight = '';
$(function () {
    //$('#btnsubmits2').click(function (e) {
    //    SaveEmployee(e);
    //})
    //$('#btnsubmit').click(function (e) {
    //    SaveEmployee(e);
    //})
    BindExittype();
    BindHighlight();


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
    const typeOfExit = [{
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
        typeOfExit.forEach(item => {
            const div = document.createElement("div");
            div.className = "newinput-dropdown-item";
            div.innerHTML = `<i class="${item.icon}" style="color:var(--purple-color)"></i> ${item.name}`;

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

function SaveEmployee(_url, _balanceurl) {
    
    var isAns = true;
    $('#divQuestions input[type="text"]').each(function () {
        if ($(this).val() == "")
            isAns = false;
    });
    
    //e.preventDefault();
    const overlay = document.getElementById("formLoaderOverlay");
    overlay.style.display = "flex";
    //var url = "/Customer/SaveEmployeeForm";
    const form = document.getElementById('msform');
    const data = new FormData(form);
    const empform = Object.fromEntries(data.entries());
    empform.exittype = exittype;
    empform.highlight = highlight;
    var files = $('#bulkFileUpload').prop("files");
    if (files[0] == null || files[0] == '') {
        empform.resume = files[0];
    }
    var questAns = [];
    var isempty = true;
    $('#divQuestions input[type="text"]').each(function () {
        questAns.push({ questionId: $(this).attr('id'), answer: $(this).val() })
        if ($(this).val() != '') {
            isempty = false;
        }
    });
    if (isempty == true) {
        //showErrorMessage("BGV questions should not be blank. Please fill more details.");
        return;
    }
    //console.log(empform);
    //console.log(questAns);
    postdata = { ...empform, employeeQuestions: questAns };

    $.ajax({
        type: 'POST',
        url: _url,
        data: JSON.stringify(postdata),
        contentType: "application/json",
        dataType: 'json', // changing data type to json
        success: function (data) { // here I'm adding data as a parameter which stores the response
            //console.log(data); // instead of alert I'm changing this to console.log which logs all the response in console.
            if (data == "exists") {
                overlay.style.display = "none";
                showErrorMessage('Employee code already exists');
                return;
            }
            else if (data == "true") {
                //clearFields();
                document.getElementById("msform").reset();
                $('#divQuestions > div > div > div > text').each(function (index, value) {
                    $(this).val('');
                });
                overlay.style.display = "none";
                showSuccessMessage('Record saved');
                ShowBalance(_balanceurl);
            }
            else {
                overlay.style.display = "none";
                showErrorMessage(data);
            }
        },
        error: function (err) {
            overlay.style.display = "none";
            console.log(err.statusText);
        }
    });
}
});