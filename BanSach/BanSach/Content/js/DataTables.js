$(document).ready(function () {
    $('#myTable').DataTable();
});
$('#myTable').DataTable({
    language: {
        processing: "Đang tải...",
        search: "Tìm kiếm:",
        lengthMenu: "Hiển thị _MENU_ dòng",
        info: "Hiển thị _START_ - _END_ trên _TOTAL_ bản ghi",
        infoEmpty: "Không có dữ liệu",
        infoFiltered: "(Lọc từ _MAX_ bản ghi)",
        zeroRecords: "Không tìm thấy kết quả",
        emptyTable: "Không có dữ liệu",
        paginate: {
            first: "Đầu",
            previous: "Trước",
            next: "Tiếp",
            last: "Cuối"
        },
        aria: {
            sortAscending: ": Sắp xếp tăng dần",
            sortDescending: ": Sắp xếp giảm dần"
        }
    }
});
