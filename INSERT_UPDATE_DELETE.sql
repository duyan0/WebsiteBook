USE Sach
GO

--slide
INSERT INTO Slide (HinhAnh, MoTa, Link, ThuTu, TrangThai) VALUES 
(N'a6a188f2-f271-4fb5-9432-3694be328731.jpg', N'Slide đầu tiên', 'https://example.com/slide1', 1, N'Hiển thị'),
(N'a6a188f2-f271-4fb5-9432-3694be328731.jpg', N'Slide thứ hai', 'https://example.com/slide2', 2, N'Hiển thị');

-- banner
INSERT INTO Banner (HinhAnh, MoTa, Link, ThuTu, TrangThai) VALUES 
(N'banner_4.jpg', N'Banner khuyến mãi 1', 'https://example.com/banner1', 1, N'Hiển thị'),
(N'banner_4.jpg', N'Banner khuyến mãi 2', 'https://example.com/banner2', 2, N'Hiển thị'),
(N'banner_4.jpg', N'Banner sản phẩm mới', 'https://example.com/banner3', 3, N'Hiển thị'),
(N'banner_4.jpg', N'Banner sự kiện', 'https://example.com/banner4', 4, N'Hiển thị');
--Admin
INSERT INTO Admin 
(HoTen, Email, SoDT, VaiTro, TKhoan, MKhau) VALUES 
(N'Phạm Huỳnh Như', 'admin@example.com', '0987654321', N'Admin', 'Admin2104', 'Admin2104');
--Khách hàng
INSERT INTO KhachHang 
(TenKH, SoDT, Email, TKhoan, MKhau, DiaChi, TrangThaiTaiKhoan, OTP, OTPExpiry, NgayTao, NgayCapNhat) VALUES 
(N'Võ Duy Ân', '0912345678', 'nguyenvana@example.com', 'Vda2104', 'Vda2104', N'123 Đường A, TP.HCM', N'Hoạt động', NULL, NULL, GETDATE(), null);
--TacGia 
INSERT INTO TacGia (TenTG, NgaySinh, QuocGia, TieuSu, HinhAnh) VALUES 
(N'Phạm Huỳnh Như', '2004-09-01', N'Việt Nam', N'Nhà văn nổi tiếng với truyện thiếu nhi.', NULL);
--Khách hàng
INSERT INTO NhaXuatBan (Tennxb, DiaChi, SDT, Email) VALUES
(N'NXB Trẻ', N'161B Lý Chính Thắng, Quận 3, TP.HCM', '02838439317', 'nxbtre@example.com'),
(N'NXB Kim Đồng', N'55 Quang Trung, Hà Nội', '02438225026', 'nxbkimdong@example.com'),
(N'NXB Văn Học', N'20 Lý Thường Kiệt, Hà Nội', '02439439223', 'nxbvanhoc@example.com'),
(N'NXB Lao Động', N'175 Giảng Võ, Hà Nội', '02437223829', 'nxblaodong@example.com'),
(N'NXB Tổng Hợp', N'62 Nguyễn Thị Minh Khai, TP.HCM', '02838293022', 'nxbthtphcm@example.com');
--Khách hàng
INSERT INTO KhuyenMai (TenKhuyenMai, NgayBatDau, NgayKetThuc, MucGiamGia, MoTa, NgayTao, NgayCapNhat)
VALUES
(N'Giảm giá hè', '2025-06-01', '2025-06-30', 18.00, N'Khuyến mãi mùa hè', GETDATE(), GETDATE()),
(N'Black Friday', '2025-11-28', '2025-11-29', 13.00, N'Siêu giảm giá Black Friday', GETDATE(), GETDATE()),
(N'Tết Nguyên Đán', '2026-01-15', '2026-02-15', 17.00, N'Ưu đãi dịp Tết', GETDATE(), GETDATE()),
(N'Back to School', '2025-08-01', '2025-09-01', 21.00, N'Khuyến mãi mùa tựu trường', GETDATE(), GETDATE()),
(N'Giáng sinh yêu thương', '2025-12-01', '2025-12-25', 12.00, N'Ưu đãi Noel', GETDATE(), GETDATE());

--Khách hàng
INSERT INTO DanhMuc (TenDanhMuc)
VALUES
(N'Sách văn học'),
(N'Sách thiếu nhi'),
(N'Sách kinh tế'),
(N'Sách kỹ năng sống'),
(N'Sách ngoại ngữ'),
(N'Sách tiếng việt');

INSERT INTO TheLoai (TenTheLoai)
VALUES
(N'Tiểu thuyết'),
(N'Khoa học viễn tưởng'),
(N'Tình cảm'),
(N'Tâm lý - xã hội'),
(N'Kỹ năng mềm'),
(N'Chính trị');

INSERT INTO DanhMuc_TheLoai (DanhMuc_ID, TheLoai_ID, HinhAnh)
VALUES
(1, 1, N'banner_4.jpg'),
(2, 3, N'banner_4.jpg'),
(3, 5, N'banner_4.jpg'),
(4, 4, N'banner_4.jpg'),
(5, 2, N'banner_4.jpg'),
(6, 6, N'banner_4.jpg');


select * from KhuyenMai
select * from NhaXuatBan
select * from TacGia
select * from TheLoai


-- Kiểm tra FlashSale đang hoạt động
SELECT * FROM FlashSale 
WHERE NgayApDung = CAST(GETDATE() AS DATE)
  AND GioBatDau <= CAST(GETDATE() AS TIME)
  AND GioKetThuc >= CAST(GETDATE() AS TIME)
  AND TrangThai = N'Hoạt động';

-- Kiểm tra sản phẩm trong FlashSale
SELECT * FROM FlashSale_SanPham WHERE IDsp = 23; -- Thay 1 bằng IDsp của sản phẩm bạn đang kiểm tra







INSERT INTO SanPham
(
    TenSP, MoTa, IDtl, GiaBan, HinhAnh, IDtg, IDnxb, IDkm,
    SoLuong, TrangThaiSach, NgayPhatHanh, ISBN, SoTrang,
    NgonNgu, LuotXem, KichThuoc, TrongLuong, NgayCapNhat,
    DiemDanhGiaTrungBinh
)
VALUES
(N'Sách 1', N'Mô tả sách 1', 1, 100000, 'AvatarAnNhu20250428141359.jpg', 1, 1, 1, 10, N'Còn Hàng', '2023-01-01', '9780000000001', 200, N'Tiếng Việt', 0, N'14x20cm', 300, GETDATE(), 4.5),
(N'Sách 2', N'Mô tả sách 2', 1, 120000, 'AvatarAnNhu20250428141359.jpg', 1, 1, 1, 5, N'Còn Hàng', '2023-02-01', '9780000000002', 150, N'Tiếng Anh', 10, N'13x19cm', 250, GETDATE(), 4.0),
(N'Sách 3', N'Mô tả sách 3', 1, 95000,  'AvatarAnNhu20250428141359.jpg', 1, 1, 1, 8, N'Còn Hàng', '2023-03-01', '9780000000003', 180, N'Tiếng Việt', 5, N'15x21cm', 320, GETDATE(), 4.3),
(N'Sách 4', N'Mô tả sách 4', 1, 105000, 'AvatarAnNhu20250428141359.jpg', 1, 1, 1, 12, N'Còn Hàng', '2023-04-01', '9780000000004', 210, N'Tiếng Việt', 15, N'14x21cm', 310, GETDATE(), 4.7),
(N'Sách 5', N'Mô tả sách 5', 1, 98000,  'AvatarAnNhu20250428141359.jpg', 1, 1, 1, 6, N'Còn Hàng', '2023-05-01', '31231231231', 170, N'Tiếng Anh', 3, N'13x18cm', 280, GETDATE(), 4.0),
(N'Sách 5', N'Mô tả sách 5', 1, 98000,  'AvatarAnNhu20250428141359.jpg', 1, 1, 1, 6, N'Còn Hàng', '2023-05-01', '123123123', 170, N'Tiếng Anh', 3, N'13x18cm', 280, GETDATE(), 4.0),
(N'Sách 5', N'Mô tả sách 5', 1, 98000,  'AvatarAnNhu20250428141359.jpg', 1, 1, 1, 6, N'Còn Hàng', '2023-05-01', '3123129312312', 170, N'Tiếng Anh', 3, N'13x18cm', 280, GETDATE(), 4.0),
(N'Sách 5', N'Mô tả sách 5', 1, 98000,  'AvatarAnNhu20250428141359.jpg', 1, 1, 1, 6, N'Còn Hàng', '2023-05-01', '312312312', 170, N'Tiếng Anh', 3, N'13x18cm', 280, GETDATE(), 4.0),
(N'Sách 5', N'Mô tả sách 5', 1, 98000,  'AvatarAnNhu20250428141359.jpg', 1, 1, 1, 6, N'Còn Hàng', '2023-05-01', '31312311', 170, N'Tiếng Anh', 3, N'13x18cm', 280, GETDATE(), 4.0),
(N'Sách 5', N'Mô tả sách 5', 1, 98000,  'AvatarAnNhu20250428141359.jpg', 1, 1, 1, 6, N'Còn Hàng', '2023-05-01', '454546', 170, N'Tiếng Anh', 3, N'13x18cm', 280, GETDATE(), 4.0),
(N'Sách 5', N'Mô tả sách 5', 1, 98000,  'AvatarAnNhu20250428141359.jpg', 1, 1, 1, 6, N'Còn Hàng', '2023-05-01', '46545664', 170, N'Tiếng Anh', 3, N'13x18cm', 280, GETDATE(), 4.0),
(N'Sách 5', N'Mô tả sách 5', 1, 98000,  'AvatarAnNhu20250428141359.jpg', 1, 1, 1, 6, N'Còn Hàng', '2023-05-01', '23432432', 170, N'Tiếng Anh', 3, N'13x18cm', 280, GETDATE(), 4.0),
(N'Sách 5', N'Mô tả sách 5', 1, 98000,  'AvatarAnNhu20250428141359.jpg', 1, 1, 1, 6, N'Còn Hàng', '2023-05-01', '34534535', 170, N'Tiếng Anh', 3, N'13x18cm', 280, GETDATE(), 4.0),
(N'Sách 5', N'Mô tả sách 5', 1, 98000,  'AvatarAnNhu20250428141359.jpg', 1, 1, 1, 6, N'Còn Hàng', '2023-05-01', '534534534', 170, N'Tiếng Anh', 3, N'13x18cm', 280, GETDATE(), 4.0),
(N'Sách 5', N'Mô tả sách 5', 1, 98000,  'AvatarAnNhu20250428141359.jpg', 1, 1, 1, 6, N'Còn Hàng', '2023-05-01', '534534', 170, N'Tiếng Anh', 3, N'13x18cm', 280, GETDATE(), 4.0),
(N'Sách 5', N'Mô tả sách 5', 1, 98000,  'AvatarAnNhu20250428141359.jpg', 1, 1, 1, 6, N'Còn Hàng', '2023-05-01', '53453453', 170, N'Tiếng Anh', 3, N'13x18cm', 280, GETDATE(), 4.0),
(N'Sách 5', N'Mô tả sách 5', 1, 98000,  'AvatarAnNhu20250428141359.jpg', 1, 1, 1, 6, N'Còn Hàng', '2023-05-01', '535345344', 170, N'Tiếng Anh', 3, N'13x18cm', 280, GETDATE(), 4.0),
(N'Sách 5', N'Mô tả sách 5', 1, 98000,  'AvatarAnNhu20250428141359.jpg', 1, 1, 1, 6, N'Còn Hàng', '2023-05-01', '534', 170, N'Tiếng Anh', 3, N'13x18cm', 280, GETDATE(), 4.0),
(N'Sách 5', N'Mô tả sách 5', 1, 98000,  'AvatarAnNhu20250428141359.jpg', 1, 1, 1, 6, N'Còn Hàng', '2023-05-01', '543511', 170, N'Tiếng Anh', 3, N'13x18cm', 280, GETDATE(), 4.0),
(N'Sách 5', N'Mô tả sách 5', 1, 98000,  'AvatarAnNhu20250428141359.jpg', 1, 1, 1, 6, N'Còn Hàng', '2023-05-01', '3123154345', 170, N'Tiếng Anh', 3, N'13x18cm', 280, GETDATE(), 4.0);

