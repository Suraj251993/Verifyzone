var documentViewed = false;

function isValidEmailFormat(email) {
    return /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email);
}

function MarkConsentDocumentViewed() {
    documentViewed = true;
    const checkbox = document.getElementById("cbxconsent");
    const wrapper = document.getElementById("consentCheckboxWrapper");
    const reviewMessage = document.getElementById("documentReviewMessage");
    if (checkbox) checkbox.disabled = false;
    if (wrapper) wrapper.style.opacity = "1";
    if (reviewMessage) reviewMessage.style.display = "none";
}

function SubmitConsent(_url) {
    const checkbox = document.getElementById("cbxconsent");
    const validationMessage = document.getElementById("consentValidationMessage");
    const optionalEmail = $.trim($('#Optionalemail').val());

    if (!documentViewed) {
        showErrorMessage("Please open and review the consent statement before submitting");
        return;
    }

    if (!checkbox.checked) {
        validationMessage.style.display = "block";
        return;
    }
    validationMessage.style.display = "none";

    if (optionalEmail !== "" && !isValidEmailFormat(optionalEmail)) {
        showErrorMessage("Please enter a valid email address");
        return;
    }

    const btn = document.getElementById("btnsubmitconsent");
    btn.disabled = true;

    $.ajax({
        type: 'POST',
        url: _url,
        data: JSON.stringify({
            token: $('#Token').val(),
            optionalemail: optionalEmail,
            consentaccepted: checkbox.checked,
            documentviewed: documentViewed
        }),
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        headers: { 'X-CSRF-TOKEN': csrfToken },
        success: function (result) {
            if (result && result.success) {
                document.getElementById("consentForm").style.display = "none";
                document.getElementById("consentSuccessBox").style.display = "block";
            } else {
                btn.disabled = false;
                showErrorMessage((result && result.message) || "Error occured while submitting your consent");
            }
        },
        error: function (err) {
            btn.disabled = false;
            showErrorMessage("Error: " + err.statusText);
        }
    });
}
