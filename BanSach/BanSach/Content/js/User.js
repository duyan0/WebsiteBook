function validateAccountInput(event) {
    var input = event.target.value;
    // Loại bỏ dấu (accent) và chỉ cho phép nhập chữ cái, số và ký tự '@'
    input = input.normalize("NFD").replace(/[\u0300-\u036f]/g, ""); // Xóa dấu
    input = input.replace(/[^a-zA-Z0-9@]/g, ""); // Chỉ cho phép chữ, số và ký tự '@'
    event.target.value = input;
}


    // Hàm lọc và loại bỏ các ký tự không mong muốn
    function sanitizeInput(input, allowedPattern) {
        input.value = input.value.replace(allowedPattern, '');
    }

    // Hàm kiểm tra email
    function validateEmail(input) {
        const value = input.value.trim();
    const allowedPattern = /[^a-zA-Z0-9@._-]/g;
    sanitizeInput(input, allowedPattern);

    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    const minLength = parseInt(input.getAttribute("minlength")) || 11;
    const maxLength = parseInt(input.getAttribute("maxlength")) || 255;
    const errorElement = input.parentNode.querySelector(".text-danger") || document.createElement("p");

    if (!emailRegex.test(value) || value.length < minLength || value.length > maxLength) {
        errorElement.textContent = "Email không hợp lệ hoặc không nằm trong độ dài cho phép (11-255 ký tự).";
    errorElement.className = "text-danger";
    errorElement.style.fontSize = "13px";
    errorElement.style.display = "block";
    input.parentNode.appendChild(errorElement);
    return false;
        } else {
            if (input.parentNode.querySelector(".text-danger")) {
        input.parentNode.removeChild(input.parentNode.querySelector(".text-danger"));
            }
    return true;
        }
    }

    // Hàm kiểm tra tài khoản
    function validateAccount(input) {
        const value = input.value.trim();
    const allowedPattern = /[^a-zA-Z0-9]/g; // Chỉ cho phép chữ và số
    sanitizeInput(input, allowedPattern);

    const minLength = parseInt(input.getAttribute("minlength")) || 6;
    const maxLength = parseInt(input.getAttribute("maxlength")) || 30;
    const errorElement = input.parentNode.querySelector(".text-danger") || document.createElement("p");

    if (value.length < minLength || value.length > maxLength) {
        errorElement.textContent = `Tài khoản phải có từ ${minLength} đến ${maxLength} ký tự và chỉ chứa chữ và số.`;
    errorElement.className = "text-danger";
    errorElement.style.fontSize = "13px";
    errorElement.style.display = "block";
    input.parentNode.appendChild(errorElement);
    return false;
        } else {
            if (input.parentNode.querySelector(".text-danger")) {
        input.parentNode.removeChild(input.parentNode.querySelector(".text-danger"));
            }
    return true;
        }
    }

    // Hàm kiểm tra mật khẩu
    function validatePassword(input) {
        const value = input.value.trim();
    const allowedPattern = /[^a-zA-Z0-9]/g; // Chỉ cho phép chữ và số
    sanitizeInput(input, allowedPattern);

    const minLength = parseInt(input.getAttribute("minlength")) || 6;
    const maxLength = parseInt(input.getAttribute("maxlength")) || 30;
    const errorElement = input.parentNode.querySelector(".text-danger") || document.createElement("p");

    if (value.length < minLength || value.length > maxLength) {
        errorElement.textContent = `Mật khẩu phải có từ ${minLength} đến ${maxLength} ký tự và chỉ chứa chữ và số.`;
    errorElement.className = "text-danger";
    errorElement.style.fontSize = "13px";
    errorElement.style.display = "block";
    input.parentNode.appendChild(errorElement);
    return false;
        } else {
            if (input.parentNode.querySelector(".text-danger")) {
        input.parentNode.removeChild(input.parentNode.querySelector(".text-danger"));
            }
    return true;
        }
    }

    // Hàm kiểm tra form trước khi submit
    function validateForm(event) {
        event.preventDefault(); // Ngăn submit mặc định

    let isValid = true;
    const inputs = document.querySelectorAll("input[required]");
        inputs.forEach(input => {
            const value = input.value.trim();
    const minLength = parseInt(input.getAttribute("minlength")) || 0;
    const maxLength = parseInt(input.getAttribute("maxlength")) || 0;
    const name = input.name;

    if (!value) {
        showError(input, `${name === "SoDT" ? "Số điện thoại" : name} không được để trống.`);
    isValid = false;
            } else if (value.length < minLength || (maxLength > 0 && value.length > maxLength)) {
        showError(input, `${name === "SoDT" ? "Số điện thoại" : name} phải có từ ${minLength} đến ${maxLength} ký tự.`);
    isValid = false;
            } else {
        hideError(input);
            }

    // Kiểm tra email đặc biệt
    if (name === "Email" && !validateEmail(input)) {
        isValid = false;
            }

    // Kiểm tra tài khoản
    if (name === "TKhoan" && !validateAccount(input)) {
        isValid = false;
            }

    // Kiểm tra mật khẩu và xác nhận mật khẩu
    if (name === "MKhau" || name === "ConfirmPass") {
                const password1 = document.getElementById("password1").value.trim();
    const password2 = document.getElementById("password2").value.trim();
    if (name === "ConfirmPass" && password1 !== password2) {
        showError(input, "Mật khẩu không khớp.");
    isValid = false;
                } else if (name === "MKhau" && !validatePassword(input)) {
        isValid = false;
                } else if (name === "ConfirmPass" && !validatePassword(input)) {
        isValid = false;
                }
            }

    // Kiểm tra số điện thoại (chỉ số và độ dài 11)
    if (name === "SoDT") {
                const phoneRegex = /^[0-9]{11}$/;
    if (!phoneRegex.test(value)) {
        showError(input, "Số điện thoại phải là 11 số (chỉ chứa số).");
    isValid = false;
                }
            }
        });

    // Nếu hợp lệ, submit form
    if (isValid) {
        event.target.submit();
        }

    return isValid;
    }

    // Hàm hiển thị lỗi
    function showError(input, message) {
        let errorElement = input.parentNode.querySelector(".text-danger");
    if (!errorElement) {
        errorElement = document.createElement("p");
    errorElement.className = "text-danger";
    errorElement.style.fontSize = "13px";
    input.parentNode.appendChild(errorElement);
        }
    errorElement.textContent = message;
    errorElement.style.display = "block";
    }

    // Hàm ẩn lỗi
    function hideError(input) {
        const errorElement = input.parentNode.querySelector(".text-danger");
    if (errorElement) {
        errorElement.style.display = "none";
        }
    }

    // Hàm hiện/ẩn mật khẩu
    function togglePasswordVisibility(passwordId) {
        const password = document.getElementById(passwordId);
    const toggle = document.getElementById(`toggle${passwordId === "password1" ? "password1" : "password2"}`);
        if (password.type === "password") {
            password.type = "text";
            toggle.textContent = "Ẩn";
        } else {
            password.type = "password";
            toggle.textContent = "Hiện";
        }
    }

