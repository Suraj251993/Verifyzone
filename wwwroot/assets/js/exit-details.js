 
$(document).ready(function () {
    const $secondFieldset = $("fieldset").eq(1);
    const $thirdFieldset = $("fieldset").eq(2);
    const checkbox = document.getElementById("cbx-12");
    const $nextBtn = $("#second-btn");
    const $progressStep3 = $("#payment");
    const $step2Label = $("#step2-label");
    $secondFieldset.hide();
    $thirdFieldset.hide();
    $progressStep3.hide();

    setButtonToSubmit();
    if (checkbox != null) {
        checkbox.addEventListener("change", (function () {
            if ($(this).is(":checked")) {
                $thirdFieldset.slideDown(300);
                $progressStep3.fadeIn(200);

                $step2Label.text("Step 2 - 3");
                $secondFieldset.removeAttr("style").css("display", "block");
                $thirdFieldset.removeAttr("style");

                setButtonToNext();
            } else {
                $thirdFieldset.slideUp(300);
                $progressStep3.fadeOut(200);

                $step2Label.text("Step 2 - 2");
                setButtonToSubmit();
                $('#Emailid').val('');
                $('#Mobileno').val('');
                $('#Fathername').val('');
                $('#Uannumber').val('');
                $('#Linkedinurl').val('');
            }
        }));
    }
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
        const submitButtons2 = document.getElementById("btnsubmits2");
        submitButtons2.removeEventListener("click", SaveEmployee(submitButtons2));
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
        $('#btnsubmits2').show();
        const submitButton = document.getElementById("btnsubmit");
        if (submitButton != null && $('#btnsubmit').is(":visible")) {
            submitButton.addEventListener("click", SaveEmployee(submitButton));
        }
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

    var current_fs, next_fs, previous_fs; // fieldsets
    var opacity;
    var current = 1;
    var steps = $("fieldset").length;

    setProgressBar(current);

    //// Next button
    $("#next").click(function (e) {
        e.preventDefault(); // prevent form submission

        let isValid = true;
        const inputFields = document.querySelectorAll(".input-details");
        inputFields.forEach(inputField => {
            const validationMessage = inputField.parentElement.querySelector(".validationMessage");
            const dateIcon = inputField.parentElement.querySelector(".dateIcon");

            const fieldValid = validateField(inputField, validationMessage, dateIcon);
            if (!fieldValid) {
                isValid = false;
            }
        });

        if (isValid) {
            current_fs = $(this).closest("fieldset");
            next_fs = current_fs.next("fieldset");

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
    });
    $("#btnnexts2").click(function (e) {
        e.preventDefault(); // prevent form submission
        //alert('hi');
        current_fs = $(this).closest("fieldset");
        next_fs = current_fs.next("fieldset");

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
    });
    // Previous button
    $(".btn-closes").click(function (e) {
        e.preventDefault(); // prevent form submission

        current_fs = $(this).closest("fieldset");
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
    });

    // Progress bar function
    function setProgressBar(curStep) {
        var percent = parseFloat(100 / steps) * curStep;
        percent = percent.toFixed();
        $(".progress-bar").css("width", percent + "%");
    }

});

function formatField(id) {
    if ($('#' + id).val() == '') {
        $('#' + id).removeClass('filled');
    }
    else {
        $('#' + id).addClass('filled');
    }
}

function validateField(inputField, validationMessage, dateIcon) {
    console.log(inputField);
    if (inputField.value.length === 0) {
        inputField.style.border = "1px solid red";
        if (validationMessage != null)
            validationMessage.style.display = "block";
        if (dateIcon) dateIcon.style.top = "30%";
        return false;
    } else {
        inputField.style.border = "";
        if (validationMessage != null)
            validationMessage.style.display = "none";
        if (dateIcon) dateIcon.style.top = "";
        return true;
    }
}