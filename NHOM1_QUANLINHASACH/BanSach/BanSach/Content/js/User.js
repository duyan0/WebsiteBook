function validateAccountInput(event) {
    var input = event.target.value;
    // Loại bỏ dấu (accent) và chỉ cho phép nhập chữ cái, số và ký tự '@'
    input = input.normalize("NFD").replace(/[\u0300-\u036f]/g, ""); // Xóa dấu
    input = input.replace(/[^a-zA-Z0-9@]/g, ""); // Chỉ cho phép chữ, số và ký tự '@'
    event.target.value = input;
}
