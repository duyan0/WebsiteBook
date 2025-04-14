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

-- Bảng Admin (không có khóa ngoại)
CREATE TABLE Admin
( 
    ID INT IDENTITY (1, 1) NOT NULL,
    HoTen NVARCHAR(50) NOT NULL,
    Email VARCHAR(100) NULL,
    SoDT VARCHAR(11) NOT NULL,
    VaiTro NVARCHAR(50) NOT NULL,
    TKhoan VARCHAR(50) NOT NULL,
    MKhau VARCHAR(50) NOT NULL,
    PRIMARY KEY CLUSTERED(ID ASC)
);
GO

-- Bảng DanhMuc (không có khóa ngoại)
CREATE TABLE DanhMuc
(
    ID INT IDENTITY(1,1) NOT NULL,
    TenDanhMuc NVARCHAR(50) NOT NULL,
    PRIMARY KEY CLUSTERED(ID ASC)
);
GO

-- Bảng TheLoai (không có khóa ngoại)
CREATE TABLE TheLoai
(
    ID INT IDENTITY(1,1) NOT NULL,
    TenTheLoai NVARCHAR(50) NOT NULL,
    PRIMARY KEY CLUSTERED(ID ASC)
);
GO

-- Bảng TacGia (không có khóa ngoại)
CREATE TABLE TacGia
(
    IDtg INT IDENTITY(1,1) NOT NULL,
    TenTG NVARCHAR(100) NOT NULL,
    NgaySinh DATE NULL,
    QuocGia NVARCHAR(50) NULL,
    TieuSu NVARCHAR(1000) NULL,
    HinhAnh NVARCHAR(MAX),
    PRIMARY KEY CLUSTERED (IDtg ASC)
);
GO

-- Bảng NhaXuatBan (không có khóa ngoại)
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

-- Bảng KhuyenMai (không có khóa ngoại)
CREATE TABLE KhuyenMai
(
    IDkm INT IDENTITY(1,1) NOT NULL,
    TenKhuyenMai NVARCHAR(100) NOT NULL,
    NgayBatDau DATE NULL,
    NgayKetThuc DATE NULL,
    MucGiamGia DECIMAL(5, 2) NULL,
    MoTa NVARCHAR(MAX) NULL,
    NgayTao DATETIME,
    NgayCapNhat DATETIME,
    PRIMARY KEY CLUSTERED (IDkm ASC),
    CONSTRAINT CK_NgayKetThuc CHECK (NgayKetThuc >= NgayBatDau),
    CONSTRAINT CK_MucGiamGia CHECK (MucGiamGia >= 0 AND MucGiamGia <= 100)
);
GO

-- Bảng KhachHang (không có khóa ngoại)
CREATE TABLE KhachHang
(
    IDkh INT IDENTITY(1,1) NOT NULL,
    TenKH NVARCHAR(50) NULL,
    SoDT VARCHAR(11) NULL,
    Email VARCHAR(255) NULL,
    TKhoan VARCHAR(50) NULL,
    MKhau VARCHAR(50) NULL,
    DiaChi NVARCHAR(255),
    TrangThaiTaiKhoan NVARCHAR(50) NOT NULL DEFAULT N'Hoạt động',
    OTP VARCHAR(6),
    OTPExpiry DATETIME,
    NgayTao DATETIME,
    NgayCapNhat DATETIME,
    PRIMARY KEY CLUSTERED (IDkh ASC),
    CONSTRAINT UQ_KhachHang_Email UNIQUE (Email),
    CONSTRAINT UQ_KhachHang_SoDT UNIQUE (SoDT)
);
GO

-- Bảng Slide (không có khóa ngoại)
CREATE TABLE Slide
(
    Slide_ID INT IDENTITY(1,1) NOT NULL,
    HinhAnh NVARCHAR(255) NULL,
    MoTa NVARCHAR(MAX) NULL,
    Link VARCHAR(100) NULL,
    ThuTu INT NULL,
    TrangThai NVARCHAR(30),
    PRIMARY KEY CLUSTERED (Slide_ID ASC)
);
GO

-- Bảng Banner (không có khóa ngoại)
CREATE TABLE Banner
(
    Banner_ID INT IDENTITY(1,1) NOT NULL,
    HinhAnh NVARCHAR(255) NULL,
    MoTa NVARCHAR(MAX) NULL,
    Link VARCHAR(100) NULL,
    ThuTu INT NULL,
    TrangThai NVARCHAR(30),
    PRIMARY KEY CLUSTERED (Banner_ID ASC)
);
GO

-- Bảng DanhMuc_TheLoai (phụ thuộc DanhMuc, TheLoai)
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

-- Bảng SanPham (phụ thuộc TheLoai, TacGia, NhaXuatBan, KhuyenMai)
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
    NgayPhatHanh DATE NULL,
    ISBN VARCHAR(13) NULL,
    SoTrang INT NULL,
    NgonNgu NVARCHAR(50) NULL,
    LuotXem INT  NULL DEFAULT 0,
    KichThuoc NVARCHAR(50) NULL,
    TrongLuong INT NULL,
    NgayTao DATETIME NULL DEFAULT GETDATE(),
    NgayCapNhat DATETIME NULL,
    DiemDanhGiaTrungBinh DECIMAL(3,1) NULL,
    PRIMARY KEY CLUSTERED (IDsp ASC),
    CONSTRAINT FK_SanPham_TheLoai FOREIGN KEY (IDtl) REFERENCES TheLoai(ID),
    CONSTRAINT FK_SanPham_TacGia FOREIGN KEY (IDtg) REFERENCES TacGia(IDtg),
    CONSTRAINT FK_SanPham_NhaXuatBan FOREIGN KEY (IDnxb) REFERENCES NhaXuatBan(IDnxb),
    CONSTRAINT FK_SanPham_KhuyenMai FOREIGN KEY (IDkm) REFERENCES KhuyenMai(IDkm),
    CONSTRAINT UQ_SanPham_ISBN UNIQUE (ISBN),
    CONSTRAINT CK_SoLuong CHECK (SoLuong > -1),
    CONSTRAINT CK_Gia CHECK (GiaBan > 0),
    CONSTRAINT CK_SoTrang CHECK (SoTrang > 0),
    CONSTRAINT CK_TrongLuong CHECK (TrongLuong > 0),
    CONSTRAINT CK_DiemDanhGiaTrungBinh CHECK (DiemDanhGiaTrungBinh BETWEEN 0 AND 5)
);
GO

-- Bảng DonHang (phụ thuộc KhachHang)
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

-- Bảng DonHangCT (phụ thuộc SanPham, DonHang)
CREATE TABLE DonHangCT
(
    ID_ctdh INT IDENTITY(1,1) NOT NULL,
    IDSanPham INT NOT NULL,
    IDDonHang INT NOT NULL,
    SoLuong INT NOT NULL,
    Gia DECIMAL(18,2) NOT NULL,
    PRIMARY KEY CLUSTERED (ID_ctdh ASC),
    FOREIGN KEY (IDSanPham) REFERENCES SanPham(IDsp),
    FOREIGN KEY (IDDonHang) REFERENCES DonHang(IDdh),
    CONSTRAINT CK_SoLuongDonHang CHECK (SoLuong > 0),
    CONSTRAINT CK_GiaDonHang CHECK (Gia >= 0)
);
GO

-- Bảng DanhGiaSanPham (phụ thuộc KhachHang, SanPham, DonHang)
CREATE TABLE DanhGiaSanPham
(
    IDdgsp INT IDENTITY(1,1) NOT NULL,
    IDkh INT NOT NULL,
    IDsp INT NOT NULL,
    IDDonHang INT NULL,
    DiemDanhGia INT NOT NULL,
    NhanXet NVARCHAR(MAX) NULL,
    NgayDanhGia DATETIME DEFAULT GETDATE(),
    PhanHoi NVARCHAR(MAX) NULL,
    PRIMARY KEY CLUSTERED (IDdgsp ASC),
    FOREIGN KEY (IDkh) REFERENCES KhachHang(IDkh),
    FOREIGN KEY (IDsp) REFERENCES SanPham(IDsp),
    FOREIGN KEY (IDDonHang) REFERENCES DonHang(IDdh),
    CONSTRAINT CK_DiemDanhGia CHECK (DiemDanhGia BETWEEN 1 AND 5),
    CONSTRAINT UQ_DanhGiaSanPham UNIQUE (IDkh, IDsp, IDDonHang)
);
GO

-- Trigger Update TrangThaiSach (phụ thuộc SanPham)
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


INSERT INTO TacGia (TenTG, NgaySinh, QuocGia, TieuSu, HinhAnh)
VALUES (N'Nguyễn Nhật Ánh', '1955-05-07', N'Việt Nam', N'Nhà văn nổi tiếng với các tác phẩm dành cho thanh thiếu niên.', N'/images/authors/nguyennhatanh.jpg');
INSERT INTO NhaXuatBan (Tennxb, DiaChi, SDT, Email)
VALUES (N'NXB Trẻ', N'161B Lý Chính Thắng, Quận 3, TP.HCM', '02839316289', 'nxbtre@gmail.com');
INSERT INTO KhuyenMai (TenKhuyenMai, NgayBatDau, NgayKetThuc, MucGiamGia, MoTa, NgayTao, NgayCapNhat)
VALUES (N'Khuyến mãi mùa hè', '2025-06-01', '2025-06-30', 20.00, N'Giảm giá 20% cho tất cả sách văn học.', GETDATE(), GETDATE());
INSERT INTO Slide (HinhAnh, MoTa, Link, ThuTu, TrangThai)
VALUES (N'/images/slides/slide1.jpg', N'Slide quảng cáo sách mới', '/sanpham/moi', 1, N'Hiển thị');
INSERT INTO Banner (HinhAnh, MoTa, Link, ThuTu, TrangThai)
VALUES (N'/images/banners/banner1.jpg', N'Banner sách văn học', '/danhmuc/vanhoc', 1, N'Hiển thị');
INSERT INTO SanPham (TenSP, MoTa, IDtl, GiaBan, HinhAnh, IDtg, IDnxb, IDkm, SoLuong, TrangThaiSach, NgayPhatHanh, ISBN, SoTrang, NgonNgu, LuotXem, KichThuoc, TrongLuong, NgayTao, NgayCapNhat, DiemDanhGiaTrungBinh)
VALUES (
    N'Mắt Biếc',
    N'Tiểu thuyết lãng mạn của Nguyễn Nhật Ánh kể về mối tình đơn phương của Ngạn.',
    1,
    120000.00,
    N'/images/books/matbiec.jpg',
    1,
    1,
    1,
    100,
    N'Còn hàng',
    '2020-01-15',
    '9786041234567',
    300,
    N'Tiếng Việt',
    1500,
    N'13.5x20.5 cm',
    350,
    GETDATE(),
    GETDATE(),
    4.5
);

INSERT INTO DanhMuc (TenDanhMuc)
VALUES 
(N'Sách Học Tập'),
(N'Sách Văn Học'),
(N'Thiếu Nhi');
INSERT INTO TheLoai (TenTheLoai)
VALUES 
(N'Toán Học'),
(N'Tiểu Thuyết'),
(N'Truyện Cổ Tích'),
(N'Văn Học Nước Ngoài');
INSERT INTO DanhMuc_TheLoai (DanhMuc_ID, TheLoai_ID, HinhAnh)
VALUES
(1, 1, N'toanhoc.jpg'),         -- Sách Học Tập - Toán Học
(1, 2, N'tieuthuyet.jpg'),      -- Sách Học Tập - Tiểu Thuyết
(2, 2, N'vanhocvn.jpg'),        -- Sách Văn Học - Tiểu Thuyết
(2, 4, N'vanhocnuocngoai.jpg'),-- Sách Văn Học - Văn Học Nước Ngoài
(3, 3, N'truyencotich.jpg'),    -- Thiếu Nhi - Truyện Cổ Tích
(3, 1, N'toanchotre.jpg');      -- Thiếu Nhi - Toán Học

select * from DanhMuc
