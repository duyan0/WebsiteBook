$(document).ready(function () {
    // Gọi AJAX khi người dùng thay đổi từ khóa tìm kiếm hoặc sắp xếp
    $('#searchString, #sortOrder').on('change', function () {
        loadProducts(1); // Tải lại trang đầu tiên khi thay đổi
    });

    // Xử lý sự kiện khi người dùng phân trang
    $(document).on('click', '.pagedListPager a', function (e) {
        e.preventDefault();
        var page = $(this).attr('href').split('=')[1]; // Lấy trang từ URL
        loadProducts(page); // Gọi lại AJAX để tải lại sản phẩm
    });
});

// Tự động ẩn thông báo thành công sau 2 giây
setTimeout(function () {
    var successAlert = document.getElementById("successAlert");
    if (successAlert) {
        $(successAlert).alert('close');
    }
}, 2000); // 2000ms = 2 giây

// Tự động ẩn thông báo lỗi sau 2 giây
setTimeout(function () {
    var errorAlert = document.getElementById("errorAlert");
    if (errorAlert) {
        $(errorAlert).alert('close');
    }
}, 2000); // 2000ms = 2 giây