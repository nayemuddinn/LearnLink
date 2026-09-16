document.addEventListener("DOMContentLoaded", function () {

    console.log("===== REGISTRATION JS LOADED =====");

    const form = document.getElementById("registrationForm");
    const registerButton = document.getElementById("registerButton");

    if (!form) {
        console.error("registrationForm was not found!");
        return;
    }

    console.log("Registration form found.");
    console.log("POST URL:", form.action);

    const name = document.getElementById("Name");
    const email = document.getElementById("Email");

    const password = document.getElementById("Password");
    const confirmPassword = document.getElementById("ConfirmPassword");

    const phone = document.getElementById("Phone");
    const institution = document.getElementById("Institution");
    const address = document.getElementById("Address");

    const nameError = document.getElementById("nameError");
    const emailError = document.getElementById("emailError");
    const passwordError = document.getElementById("passwordError");
    const confirmPasswordError =
        document.getElementById("confirmPasswordError");

    const phoneError = document.getElementById("phoneError");
    const institutionError =
        document.getElementById("institutionError");

    const addressError = document.getElementById("addressError");


    function showError(input, errorElement, message) {

        errorElement.textContent = message;

        input.classList.remove("input-success");
        input.classList.add("input-error");
    }


    function showSuccess(input) {

        input.classList.remove("input-error");
        input.classList.add("input-success");
    }


    function clearError(input, errorElement) {

        errorElement.textContent = "";

        input.classList.remove("input-error");
    }


    function clearAllErrors() {

        const inputs = [
            name,
            email,
            password,
            confirmPassword,
            phone,
            institution,
            address
        ];

        const errors = [
            nameError,
            emailError,
            passwordError,
            confirmPasswordError,
            phoneError,
            institutionError,
            addressError
        ];

        inputs.forEach(function (input) {

            if (input) {
                input.classList.remove(
                    "input-error",
                    "input-success"
                );
            }
        });

        errors.forEach(function (error) {

            if (error) {
                error.textContent = "";
            }
        });
    }


    form.addEventListener("submit", async function (event) {

        event.preventDefault();

        console.log("===== SUBMIT FIRED =====");

        clearAllErrors();

        let isValid = true;


        // -------------------------
        // NAME
        // -------------------------

        if (name.value.trim() === "") {

            showError(
                name,
                nameError,
                "Full name is required."
            );

            isValid = false;

        } else {

            showSuccess(name);
        }


        // -------------------------
        // EMAIL
        // -------------------------

        if (email.value.trim() === "") {

            showError(
                email,
                emailError,
                "Email is required."
            );

            isValid = false;

        } else if (!email.validity.valid) {

            showError(
                email,
                emailError,
                "Please enter a valid email address."
            );

            isValid = false;

        } else {

            showSuccess(email);
        }


        // -------------------------
        // PASSWORD
        // -------------------------

        const passwordValue = password.value;

        if (passwordValue === "") {

            showError(
                password,
                passwordError,
                "Password is required."
            );

            isValid = false;

        } else if (passwordValue.length < 6) {

            showError(
                password,
                passwordError,
                "Password must be at least 6 characters."
            );

            isValid = false;

        } else if (!/[0-9]/.test(passwordValue)) {

            showError(
                password,
                passwordError,
                "Password must contain at least one digit."
            );

            isValid = false;

        } else if (!/[^a-zA-Z0-9]/.test(passwordValue)) {

            showError(
                password,
                passwordError,
                "Password must contain at least one special character."
            );

            isValid = false;

        } else {

            showSuccess(password);
        }


        // -------------------------
        // CONFIRM PASSWORD
        // -------------------------

        if (confirmPassword.value === "") {

            showError(
                confirmPassword,
                confirmPasswordError,
                "Please confirm your password."
            );

            isValid = false;

        } else if (confirmPassword.value !== password.value) {

            showError(
                confirmPassword,
                confirmPasswordError,
                "Passwords do not match."
            );

            isValid = false;

        } else {

            showSuccess(confirmPassword);
        }


        // -------------------------
        // PHONE
        // -------------------------

        if (phone.value.trim() === "") {

            showError(
                phone,
                phoneError,
                "Phone number is required."
            );

            isValid = false;

        } else {

            showSuccess(phone);
        }


        // -------------------------
        // INSTITUTION
        // -------------------------

        if (institution.value.trim() === "") {

            showError(
                institution,
                institutionError,
                "Institution is required."
            );

            isValid = false;

        } else {

            showSuccess(institution);
        }


        // -------------------------
        // ADDRESS
        // -------------------------

        if (address.value.trim() === "") {

            showError(
                address,
                addressError,
                "Address is required."
            );

            isValid = false;

        } else {

            showSuccess(address);
        }


        // -------------------------
        // STOP IF INVALID
        // -------------------------

        if (!isValid) {

            console.log("===== CLIENT VALIDATION FAILED =====");

            return;
        }


        console.log("===== CLIENT VALIDATION PASSED =====");


        // -------------------------
        // BUTTON
        // -------------------------

        registerButton.disabled = true;
        registerButton.textContent = "Creating Account...";


        try {

            const formData = new FormData(form);

            console.log("===== FORM DATA =====");

            for (const [key, value] of formData.entries()) {
                console.log(key + ":", value);
            }


            console.log("===== SENDING POST REQUEST =====");
            console.log("URL:", form.action);


            const response = await fetch(
                form.action,
                {
                    method: "POST",
                    body: formData
                }
            );


            console.log(
                "===== SERVER RESPONSE ====="
            );

            console.log(
                "Status:",
                response.status
            );

            console.log(
                "OK:",
                response.ok
            );


            const result = await response.json();


            console.log(
                "Response JSON:",
                result
            );


            if (result.success) {

                console.log(
                    "===== REGISTRATION SUCCESS ====="
                );

                showPopup(
                    "success",
                    result.message
                );

                form.reset();

                clearAllErrors();

                const teacherRole =
                    document.getElementById("role-teacher");

                if (teacherRole) {
                    teacherRole.checked = true;
                }

            } else {

                console.log(
                    "===== REGISTRATION FAILED ====="
                );

                showPopup(
                    "error",
                    result.message
                );
            }

        } catch (error) {

            console.error(
                "===== FETCH ERROR ====="
            );

            console.error(error);

            showPopup(
                "error",
                "Unable to connect to the server. Please try again."
            );

        } finally {

            registerButton.disabled = false;
            registerButton.textContent = "Create Account";
        }
    });


    // -------------------------
    // INPUT EVENTS
    // -------------------------

    name.addEventListener("input", function () {

        clearError(
            name,
            nameError
        );
    });


    email.addEventListener("input", function () {

        clearError(
            email,
            emailError
        );
    });


    password.addEventListener("input", function () {

        clearError(
            password,
            passwordError
        );
    });


    confirmPassword.addEventListener("input", function () {

        clearError(
            confirmPassword,
            confirmPasswordError
        );
    });


    phone.addEventListener("input", function () {

        clearError(
            phone,
            phoneError
        );
    });


    institution.addEventListener("input", function () {

        clearError(
            institution,
            institutionError
        );
    });


    address.addEventListener("input", function () {

        clearError(
            address,
            addressError
        );
    });

});


function showPopup(type, message) {

    const toast = document.getElementById("glass-toast");
    const toastMsg = document.getElementById("toastMessage");
    const toastIcon = toast ? toast.querySelector("ion-icon") : null;

    if (!toast || !toastMsg || !toastIcon) {
        console.error("Toast elements not found.");
        return;
    }

    toastMsg.textContent = message;

    // Update icon based on type
    if (type === "success") {
        toastIcon.setAttribute("name", "checkmark-circle");
        toast.classList.remove("error-toast");
    } else {
        toastIcon.setAttribute("name", "alert-circle");
        toast.classList.add("error-toast");
    }

    // Show toast
    toast.classList.add("show");

    // Auto-hide after 4 seconds
    setTimeout(function () {
        toast.classList.remove("show");
    }, 4000);
}


function closeRegistrationPopup() {

    const toast = document.getElementById("glass-toast");

    if (toast) {
        toast.classList.remove("show");
    }
}