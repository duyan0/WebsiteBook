USE master;
GO
IF EXISTS (SELECT name FROM sys.databases WHERE name = 'Sach')
BEGIN
    ALTER DATABASE Sach SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE Sach;
END
GO
-- Tạo cơ sở dữ liệu mới
CREATE DATABASE Sach;
GO
-- Chuyển sang cơ sở dữ liệu mới tạo
USE Sach;
GO
-- Bảng Admin
CREATE TABLE Admin
( 
    ID INT IDENTITY (1, 1) NOT NULL,
    HoTen NVARCHAR(50) NOT NULL,
    Email VARCHAR(100) NULL,
    DiaChi NVARCHAR(255) NULL,
    SoDT VARCHAR(11) NOT NULL,
    VaiTro NVARCHAR(50) NOT NULL,
    TKhoan VARCHAR(50) NOT NULL,
    MKhau VARCHAR(50) NOT NULL,
    PRIMARY KEY CLUSTERED(ID ASC)
);
GO
-- Bảng Category
CREATE TABLE DanhMuc
(
    ID INT IDENTITY(1,1) NOT NULL,
    TenDanhMuc NVARCHAR(50) NOT NULL,
    PRIMARY KEY CLUSTERED(ID ASC)
);

GO

-- Bảng TheLoai
CREATE TABLE TheLoai
(
    ID INT IDENTITY(1,1) NOT NULL,
    TenTheLoai NVARCHAR(50) NOT NULL,
    PRIMARY KEY CLUSTERED(ID ASC)
);
GO

-- Bảng DanhMuc_TheLoai
CREATE TABLE DanhMuc_TheLoai
(
    ID INT IDENTITY(1,1) NOT NULL,
    DanhMuc_ID INT NOT NULL,
    TheLoai_ID INT NOT NULL,
    HinhAnh NVARCHAR(255) NOT NULL,
    PRIMARY KEY CLUSTERED(ID ASC),
    FOREIGN KEY (DanhMuc_ID) REFERENCES DanhMuc(ID),
    FOREIGN KEY (TheLoai_ID) REFERENCES TheLoai(ID)
);
GO

-- Bảng KhachHang
CREATE TABLE KhachHang
(
    IDkh INT IDENTITY(1,1) NOT NULL,
    TenKH NVARCHAR(50) NULL,
    SoDT VARCHAR(11) NULL,
    Email VARCHAR(255) NULL,
    TKhoan VARCHAR(50) NULL,
    MKhau VARCHAR(50) NULL,
    TrangThaiTaiKhoan NVARCHAR(50) NOT NULL DEFAULT N'Hoạt động',
    OTP VARCHAR(6),
    OTPExpiry DATETIME,
	create_date DATETIME,
    PRIMARY KEY CLUSTERED (IDkh ASC),
    CONSTRAINT UQ_KhachHang_Email UNIQUE (Email),
    CONSTRAINT UQ_KhachHang_SoDT UNIQUE (SoDT)
);
GO

-- Bảng TacGia
CREATE TABLE TacGia
(
    IDtg INT IDENTITY(1,1) NOT NULL,
    TenTG NVARCHAR(100) NOT NULL,
    NgaySinh DATE NULL,
    QuocGia NVARCHAR(50) NULL,
    TieuSu NVARCHAR(1000) NULL,
    PRIMARY KEY CLUSTERED (IDtg ASC)
);
GO

-- Bảng NhaXuatBan
CREATE TABLE NhaXuatBan
(
    IDnxb INT IDENTITY(1,1) NOT NULL,
    Tennxb NVARCHAR(100) NOT NULL,
    DiaChi NVARCHAR(255) NULL,
    SDT VARCHAR(11) NULL,
    Email NVARCHAR(100) NULL,
    PRIMARY KEY CLUSTERED (IDnxb ASC),
    CONSTRAINT UQ_NhaXuatBan_Email UNIQUE (Email)
);
GO

-- Bảng KhuyenMai
CREATE TABLE KhuyenMai
(
    IDkm INT IDENTITY(1,1) NOT NULL,
    TenKhuyenMai NVARCHAR(100) NOT NULL,
    NgayBatDau DATE NULL,
    NgayKetThuc DATE NULL,
    MucGiamGia DECIMAL(5, 2) NULL,
    MoTa NVARCHAR(MAX) NULL,
    NgayTao DATETIME,
    PRIMARY KEY CLUSTERED (IDkm ASC),
    CONSTRAINT CK_NgayKetThuc CHECK (NgayKetThuc >= NgayBatDau),
    CONSTRAINT CK_MucGiamGia CHECK (MucGiamGia >= 0 AND MucGiamGia <= 100)
);
GO

-- Bảng SanPham
CREATE TABLE SanPham
(
    IDsp INT IDENTITY(1,1) NOT NULL,
    TenSP NVARCHAR(255) NOT NULL,
    MoTa NVARCHAR(MAX) NULL,
    IDtl INT NOT NULL,
    GiaBan DECIMAL(18,2) NOT NULL,
    HinhAnh NVARCHAR(255) NOT NULL,
    IDtg INT NOT NULL,
    IDnxb INT NOT NULL,
    IDkm INT NOT NULL,
    SoLuong INT NOT NULL,
    TrangThaiSach NVARCHAR(50) NULL,
    PRIMARY KEY CLUSTERED (IDsp ASC),
    CONSTRAINT FK_SanPham_TheLoai FOREIGN KEY (IDtl) REFERENCES TheLoai(ID),
    CONSTRAINT FK_SanPham_TacGia FOREIGN KEY (IDtg) REFERENCES TacGia(IDtg),
    CONSTRAINT FK_SanPham_NhaXuatBan FOREIGN KEY (IDnxb) REFERENCES NhaXuatBan(IDnxb),
    CONSTRAINT FK_SanPham_KhuyenMai FOREIGN KEY (IDkm) REFERENCES KhuyenMai(IDkm),
    CONSTRAINT CK_SoLuong CHECK (SoLuong > -1),
    CONSTRAINT CK_Gia CHECK (GiaBan > 0)
);
GO

-- Bảng DonHang
CREATE TABLE DonHang
(
    IDdh INT IDENTITY (1, 1) NOT NULL,
    NgayDatHang DATETIME NULL,
    IDkh INT NULL,
    DiaChi NVARCHAR(255) NULL,
    NgayNhanHang DATETIME NULL,
    TrangThai NVARCHAR(30) NULL,
    PRIMARY KEY CLUSTERED (IDdh ASC),
    FOREIGN KEY (IDkh) REFERENCES KhachHang(IDkh)
);
GO

-- Bảng DonHangCT
CREATE TABLE DonHangCT
(
    ID_ctdh INT IDENTITY(1,1) NOT NULL,
    IDSanPham INT NULL,
    IDDonHang INT NULL,
    SoLuong INT NULL,
    Gia FLOAT(53) NULL,
    DanhGia NVARCHAR(MAX) NULL,
    PRIMARY KEY CLUSTERED (ID_ctdh ASC),
    FOREIGN KEY (IDSanPham) REFERENCES SanPham(IDsp),
    FOREIGN KEY (IDDonHang) REFERENCES DonHang(IDdh)
);
GO

-- Bảng Slide
CREATE TABLE Slide
(
    Slide_ID INT IDENTITY(1,1) NOT NULL,
    HinhAnh NVARCHAR(255) NULL,
    MoTa NVARCHAR(MAX) NULL,
    Link VARCHAR(100) NULL,
    ThuTu INT NULL,
    PRIMARY KEY CLUSTERED (Slide_ID ASC)
);
GO

-- Bảng Banner
CREATE TABLE Banner
(
    Banner_ID INT IDENTITY(1,1) NOT NULL,
    HinhAnh NVARCHAR(255) NULL,
    MoTa NVARCHAR(MAX) NULL,
    Link VARCHAR(100) NULL,
    ThuTu INT NULL,
    PRIMARY KEY CLUSTERED (Banner_ID ASC)
);
GO

-- Trigger Update TrangThaiSach
CREATE TRIGGER trg_UpdateTrangThaiSach
ON [Sach].[dbo].[SanPham]
AFTER INSERT, UPDATE
AS
BEGIN
    UPDATE [Sach].[dbo].[SanPham]
    SET [TrangThaiSach] = N'Hết hàng'
    WHERE [SoLuong] = 0
      AND [IDsp] IN (SELECT [IDsp] FROM inserted)
    
    UPDATE [Sach].[dbo].[SanPham]
    SET [TrangThaiSach] = N'Còn hàng'
    WHERE [SoLuong] > 0
      AND [IDsp] IN (SELECT [IDsp] FROM inserted)
END;
GO

-- Trigger Set NgayTao
CREATE TRIGGER trg_SetNgayTao
ON [Sach].[dbo].[KhuyenMai]
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE [Sach].[dbo].[KhuyenMai]
    SET NgayTao = GETDATE()
    FROM [Sach].[dbo].[KhuyenMai] km
    INNER JOIN inserted i ON km.IDkm = i.IDkm;
END;
GO

CREATE TRIGGER trg_UpdateTotalQuantityDonHangCT
ON [Sach].[dbo].[DonHangCT]
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    DECLARE @IDdh INT;
    -- Lấy ID của đơn hàng đã thay đổi
    SELECT @IDdh = IDDonHang FROM inserted;

    -- Tính tổng số lượng cho đơn hàng
    UPDATE [Sach].[dbo].[DonHang]
    SET totalquantity = (SELECT SUM(SoLuong) FROM [Sach].[dbo].[DonHangCT] WHERE IDDonHang = @IDdh)
    WHERE IDdh = @IDdh;
END;
GO

CREATE TRIGGER trg_UpdateOTPExpiry
ON [Sach].[dbo].[KhachHang]
AFTER INSERT, UPDATE
AS
BEGIN
    -- Cập nhật trường OTPExpiry bằng thời gian hiện tại khi có sự thay đổi mã OTP
    UPDATE [Sach].[dbo].[KhachHang]
    SET OTPExpiry = GETDATE() -- Gán thời gian hiện tại
    WHERE IDkh IN (SELECT IDkh FROM inserted)
      AND OTP IS NOT NULL;  -- Chỉ cập nhật khi có mã OTP mới
END;
GO

CREATE TRIGGER trg_SetCreateDate
ON [Sach].[dbo].[KhachHang]
AFTER INSERT
AS
BEGIN
    -- Cập nhật trường create_date bằng thời gian hiện tại khi có bản ghi mới được chèn vào
    UPDATE [Sach].[dbo].[KhachHang]
    SET create_date = GETDATE() -- Gán thời gian hiện tại
    WHERE IDkh IN (SELECT IDkh FROM inserted);
END;
GO

--Bảng Admins
INSERT INTO Admin(HoTen, Email, DiaChi, SoDT, VaiTro, TKhoan, MKhau)
VALUES (N'Võ Duy Ân', N'admin@gmail.com', N'Số 123, Đường ABC', N'0912345678', N'Admin', N'admin', N'1');
--Bảng DanhMuc
INSERT INTO DanhMuc (TenDanhMuc)
VALUES (N'Sách Tiếng Việt'), (N'Sách Tiếng Anh'), (N'Sách Kinh Tế');
--Bảng TheLoai
INSERT INTO TheLoai (TenTheLoai)
VALUES (N'Văn học'), (N'Giáo dục'), (N'Sách thiếu nhi');
--Bảng DanhMuc_TheLoai
INSERT INTO DanhMuc_TheLoai (DanhMuc_ID, TheLoai_ID, HinhAnh)
VALUES (1, 1, N'./images/sach_tieng_viet.jpg'), (2, 2, N'./images/sach_tieng_anh.jpg');
--Bảng KhachHang
INSERT INTO KhachHang (TenKH, SoDT, Email, TKhoan, MKhau, TrangThaiTaiKhoan, OTP, OTPExpiry, create_date)
VALUES (N'Nguyễn Thị B', N'0934567890', N'nguyenb@example.com', N'voduyan', N'voduyan', N'Hoạt động', N'123456', DATEADD(HOUR, 1, GETDATE()), GETDATE());
--Bảng TacGia
INSERT INTO TacGia (TenTG, NgaySinh, QuocGia, TieuSu)
VALUES (N'Nguyễn Nhật Ánh', '1955-12-01', N'Việt Nam', N'Nhà văn nổi tiếng của Việt Nam.');
--Bảng NhaXuatBan
INSERT INTO NhaXuatBan (Tennxb, DiaChi, SDT, Email)
VALUES (N'Nhà xuất bản Trẻ', N'123 Đường ABC, Quận 1, TP.HCM', N'0901234567', N'nxb@tre.com.vn');
--Bảng KhuyenMai
INSERT INTO KhuyenMai (TenKhuyenMai, NgayBatDau, NgayKetThuc, MucGiamGia, MoTa, NgayTao)
VALUES (N'Giảm giá mùa hè', '2025-06-01', '2025-06-30', 20.00, N'Giảm giá 20% cho tất cả sách', GETDATE());
--Bảng SanPham
INSERT INTO SanPham (TenSP, MoTa, IDtl, GiaBan, HinhAnh, IDtg, IDnxb, IDkm, SoLuong, TrangThaiSach)
VALUES (N'Sách học lập trình C#', N'Sách hướng dẫn lập trình C# cho người mới bắt đầu', 1, 250000, N'./images/sach_csharp.jpg', 1, 1, 1, 0, N'Hết hàng');
--Bảng Slide
INSERT INTO Slide (HinhAnh, MoTa, Link, ThuTu)VALUES 
(N'anh_slide1.jpg', N'Mô tả cho Slide 1', 'https://example.com/slide1', 1)
--Bảng Banner
INSERT INTO Banner (HinhAnh, MoTa, Link, ThuTu)VALUES
(N'anh_banner1.jpg', N'Mô tả cho Banner 1', 'https://example.com/banner1', 1),
(N'anh_banner2.jpg', N'Mô tả cho Banner 2', 'https://example.com/banner2', 2),
(N'anh_banner3.jpg', N'Mô tả cho Banner 3', 'https://example.com/banner3', 3);

--Select tất cả các trigger
SELECT 
    t.name AS TênTrigger,                    -- Lấy tên của trigger và gọi là "TênTrigger"
    OBJECT_NAME(t.parent_id) AS TênBảng,     -- Lấy tên bảng mà trigger gắn vào, gọi là "TênBảng"
    t.create_date AS NgàyTạo,                -- Lấy ngày tạo trigger, gọi là "NgàyTạo"
    t.is_disabled AS TrạngTháiTắt,           -- Lấy trạng thái tắt/bật của trigger, gọi là "TrạngTháiTắt"
    t.is_instead_of_trigger AS TriggerThayThế  -- Lấy xem trigger có phải loại thay thế không, gọi là "TriggerThayThế"
FROM 
    sys.triggers t                           -- Từ bảng hệ thống sys.triggers, đặt tên ngắn là "t"
INNER JOIN 
    sys.objects o ON t.parent_id = o.object_id  -- Kết hợp với bảng sys.objects dựa trên parent_id
WHERE 
    t.type = 'TR'                            -- Chỉ lấy những thứ là trigger thôi
ORDER BY 
    t.name;                                  -- Sắp xếp theo tên trigger

