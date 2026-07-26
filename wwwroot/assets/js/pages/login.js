
const togglePassword = document.getElementById('togglePassword');
const passwordField = document.querySelector('.login-pass');

togglePassword.addEventListener('click', () => {
    const type = passwordField.getAttribute('type') === 'password' ? 'text' : 'password';
    passwordField.setAttribute('type', type);

    togglePassword.classList.toggle('ri-eye-line');
    togglePassword.classList.toggle('ri-eye-off-line');
});

// Generate random captcha
function generateCaptcha() {
    const chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
    let captcha = "";
    for (let i = 0; i < 5; i++) {
        captcha += chars.charAt(Math.floor(Math.random() * chars.length));
    }
    return captcha;
}

const captchaText = document.querySelector(".captcha-place");
const refreshBtn = document.querySelector(".captcha-refresh");
const captchaInput = document.querySelector('.captcha-section input[type="text"]');
const captchaMessage = document.getElementById("captchaMessage");

// Set initial captcha
function setNewCaptcha() {
    captchaText.textContent = generateCaptcha();
    captchaMessage.textContent = "";
    captchaMessage.style.color = "";
    captchaInput.value = "";
}

// Refresh captcha
refreshBtn.addEventListener("click", () => {
    setNewCaptcha();
});

// Validate captcha as user types
captchaInput.addEventListener("input", () => {
    captchaMessage.innerHTML = ""; // Clear previous message

    if (captchaInput.value === "") {
        return;
    }

    if (captchaInput.value === captchaText.textContent) {
        captchaMessage.innerHTML = `
    <i class="ri-checkbox-circle-line" style="color: green; font-size: 18px;"></i>
    <span style="color: green;">Captcha correct</span>
`;
    } else {
        captchaMessage.innerHTML = `
    <i class="ri-close-circle-line" style="color: red; font-size: 18px;"></i>
    <span style="color: red;">Captcha incorrect</span>
`;
    }
});

refreshBtn.addEventListener("click", () => {
    setNewCaptcha();

    // Add rotate animation class
    refreshBtn.classList.add("rotate");

    // Remove class after animation ends so it can play again next click
    setTimeout(() => {
        refreshBtn.classList.remove("rotate");
    }, 500); // match animation duration
});

const loginBtn = document.getElementById("loginBtn");
const loaderBox = document.getElementById("loaderBox");
const formBox = document.getElementById("loginFormBox");

loginBtn.addEventListener("click", () => {
    if ($('#LoginName').val() == "" || $("#Password").val() == "" || $("#txtcaptcha").val() == "") {
        showErrorMessage('All the fields are mandatory');
    }
    else {
        // Apply loading background only to form box
        formBox.classList.add("form-loading");

        // Show loader on top
        loaderBox.style.display = "block";

        // Disable login button
        loginBtn.disabled = true;
        loginBtn.textContent = "Processing...";

        $('#formAuthentication').submit();
    }
});

$(function () {    
    if (myError != '') {
        // Apply loading background only to form box
        formBox.classList.remove("form-loading");
        loaderBox.style.display = "none";
        showErrorMessage(myError);
    }
    
    setNewCaptcha();
    const inputField = document.getElementById("txtemail");
    const validationMessage = document.getElementById("forgotvalidationMessage");
    const dateIcon = inputField.parentElement.querySelector(".dateIcon");
    const submitButton = document.getElementById("Submit");
    submitButton.addEventListener("click", () => {
        let isValid = true;
        const fieldValid = validateField(inputField, validationMessage, dateIcon);
        if (!fieldValid) {
            isValid = false;
        }
    });
});

function validateField(inputField, validationMessage, dateIcon) {
    if (inputField.value == null || inputField.value == "") {
        inputField.style.border = "1px solid red";
        if (validationMessage != null) validationMessage.style.display = "block";
        if (dateIcon) dateIcon.style.top = "30%";
        return false;
    } else {
        inputField.style.border = "";
        if (validationMessage != null) validationMessage.style.display = "none";
        if (dateIcon) dateIcon.style.top = "";
        return true;
    }
}