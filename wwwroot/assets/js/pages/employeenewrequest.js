var exittype = '';
var current_fs, next_fs, previous_fs; // fieldsets
var opacity;
var current = 1;
var steps = $("fieldset").length;
var customerid = '';

$(function () {
    BindExittype();
});

function BindExittype() {
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
    $("#dropdownExitType").empty();
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

function SaveEmployee(_url) {

    //e.preventDefault();
    const overlay = document.getElementById("formLoaderOverlay");
    overlay.style.display = "flex";
    const form = document.getElementById('msform');
    const data = new FormData(form);
    const empform = Object.fromEntries(data.entries());
    empform.customername = $('#Customername').val();
    empform.customeraddress = $('#Companyaddress').val();
    empform.hrname = $('#Hrname').val();
    empform.hremail = $('#Hremail').val();
    empform.emailbody = $('#Emailbody').val();
    empform.customerid = customerid;
    empform.exittype = exittype;
    var questAns = [];
    $('#divQuestions input[type="text"]').each(function () {
        questAns.push({ questionId: $(this).attr('id'), answer: $(this).val() });
    });
    console.log(questAns);
    postdata = { ...empform, employeeQuestions: questAns };

    $.ajax({
        type: 'POST',
        url: _url,
        data: JSON.stringify(postdata),
        contentType: "application/json",
        dataType: 'json', // changing data type to json
        success: function (data) { // here I'm adding data as a parameter which stores the response
            //console.log(data); // instead of alert I'm changing this to console.log which logs all the response in console.
            if (data == "true") {
                //clearFields();
                document.getElementById("msform").reset();
                $('#divQuestions > div > div > div > text').each(function (index, value) {
                    $(this).val('');
                });
                overlay.style.display = "none";
                showSuccessMessage('Verification initiation request was sent to the employer');
            }
            else {
                overlay.style.display = "none";
                showErrorMessage(data);
            }
        },
        error: function (err) {
            overlay.style.display = "none";
            showErrorMessage(err.statusText);
        }
    });
}

function validateField(id) {
    if ($('#' + id).val() == '') {
        $('#' + id).removeClass('filled');
        return true;
    }
    else {
        $('#' + id).addClass('filled');
        return false;
    }
}


$(document).ready(function () {
    const $secondFieldset = $("fieldset").eq(1);
    const $nextBtn = $("#second-btn");
    const $step2Label = $("#step2-label");

    setButtonToSubmit();

    //$nextBtn.on("click", function (e) {
    //    if (!$checkbox.is(":checked")) {
    //        e.stopImmediatePropagation();
    //        e.preventDefault();
    //    }
    //});

    function setButtonToNext() {
        //const text = "Next";
        //$nextBtn.find("#btnnexts2").text(text);

        //const $letters = $nextBtn.find(".letters");
        //$letters.empty();
        //for (let char of text) {
        //    $letters.append(`<span>${char}</span>`);
        //}
        $('#btnnexts2').show();
        $('#btnsubmits2').hide();
    }

    function setButtonToSubmit() {
        //const text = "Submit";
        //$nextBtn.find("#btnnexts2").text(text);

        //const $letters = $nextBtn.find(".letters");
        //$letters.empty();
        //for (let char of text) {
        //    $letters.append(`<span>${char}</span>`);
        //}
        $('#btnnexts2').hide();
    }
});

document.querySelectorAll('.reasonContainer').forEach(container => {
    const input = container.querySelector('input');
    const dropdown = container.querySelector('.newinput-dropdown-menu');
    const icon = container.querySelector('.chevronIcon');

    container.addEventListener('click', () => {
        dropdown.classList.toggle('show');
        icon.classList.toggle('rotate');
    });

    dropdown.querySelectorAll('.newinput-dropdown-item').forEach(item => {
        item.addEventListener('click', () => {
            // Update input value
            input.value = item.textContent.trim();
            input.classList.add('filled');

            // Remove "selected" from all items
            dropdown.querySelectorAll('.newinput-dropdown-item').forEach(i => i.classList.remove('selected'));

            // Add "selected" to clicked item
            item.classList.add('selected');

            // Close dropdown
            dropdown.classList.remove('show');
            icon.classList.remove('rotate');
        });
    });

    document.addEventListener('click', (e) => {
        if (!container.contains(e.target)) {
            dropdown.classList.remove('show');
            icon.classList.remove('rotate');
        }
    });
});



function handleFocus(input) {
    input.placeholder = "From date";
    // Automatically open the date picker
    setTimeout(() => input.showPicker && input.showPicker(), 100);
}

function handleBlur(input) {
    if (!input.value) {
        input.placeholder = "From date";
    }
}
flatpickr("#fromDate", {
    dateFormat: "d-m-Y",
    allowInput: true
});
flatpickr("#joinDate", {
    dateFormat: "d-m-Y",
    allowInput: true
});
flatpickr("#lastworkingDate", {
    dateFormat: "d-m-Y",
    allowInput: true
});
flatpickr("#toDate", {
    dateFormat: "d-m-Y",
    allowInput: true
});

$(document).ready(function () {
    setProgressBar(current);
});
// Next button
function gotoNext(e) {
    //e.preventDefault(); // prevent form submission
    let isValid = true;
    const inputFields = document.querySelectorAll(".input-details");

    inputFields.forEach(inputField => {
        const validationMessage = inputField.parentElement.querySelector(".validationMessage");
        const dateIcon = inputField.parentElement.querySelector(".dateIcon");

        validateField(inputField, validationMessage, dateIcon);
        if (validateField(inputField, validationMessage, dateIcon) == false) {
            if (validationMessage != null)
                isValid = false;
        }
    });
    
    if (isValid) {
        current_fs = $(e).closest("fieldset");
        next_fs = current_fs.next("fieldset");
        console.log('next: ' + next_fs.length);
        if (next_fs.length === 0) return; // if no next fieldset, do nothing

        // Add class active to next step in progressbar
        $("#progressbar li").eq($("fieldset").index(next_fs)).addClass("active");

        // Show the next fieldset
        next_fs.show();

        // Hide the current fieldset with animation
        current_fs.animate({
            opacity: 0
        }, {
            step: function (now) {
                opacity = 1 - now;

                current_fs.css({
                    'display': 'none',
                    'position': 'relative'
                });
                next_fs.css({
                    'opacity': opacity
                });
            },
            duration: 500
        });

        setProgressBar(++current);
    }
}

// Progress bar function
function setProgressBar(curStep) {
    var percent = parseFloat(100 / steps) * curStep;
    percent = percent.toFixed();
    $(".progress-bar").css("width", percent + "%");
}

// Previous button
function gotoClose(e) {
    //e.preventDefault(); // prevent form submission

    current_fs = $(e).closest("fieldset");
    previous_fs = current_fs.prev("fieldset");

    if (previous_fs.length === 0) return; // if no previous fieldset, do nothing

    // Remove active class from current step
    $("#progressbar li").eq($("fieldset").index(current_fs)).removeClass("active");

    // Show the previous fieldset
    previous_fs.show();

    // Hide the current fieldset with animation
    current_fs.animate({
        opacity: 0
    }, {
        step: function (now) {
            opacity = 1 - now;

            current_fs.css({
                'display': 'none',
                'position': 'relative'
            });
            previous_fs.css({
                'opacity': opacity
            });
        },
        duration: 500
    });

    setProgressBar(--current);
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

    const customerdropdownMenus = document.querySelectorAll("#mddsearchcustomer");
    $('#mddsearchcustomer').empty();

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