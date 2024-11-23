USE master;
GO
IF EXISTS (SELECT name FROM sys.databases WHERE name = 'Sach')
BEGIN
    DROP DATABASE Sach;
END
GO
CREATE DATABASE Sach;
GO
USE Sach;
GO
--Bảng Admin
CREATE TABLE Admin
( 
	ID INT IDENTITY (1, 1) NOT NULL,
	HoTen NVARCHAR(50) NOT NULL,
	Email	   NCHAR(50)  NULL,
	DiaChi   NVARCHAR(MAX) NULL,
	SoDT	   NCHAR(15) NOT NULL,
	VaiTro	NCHAR(50) NOT NULL,
	TKhoan	   NCHAR(50) NOT NULL,
	MKhau	   NCHAR(50) NOT NULL,
	PRIMARY KEY CLUSTERED(ID ASC)
);
go
--Bang Danh Muc san pham
CREATE TABLE DanhMuc
(
	ID INT IDENTITY (1,1) NOT NULL,
	DanhMuc NCHAR(50) NOT NULL,
	TheLoai NVARCHAR(50) NOT NULL,
	PRIMARY KEY CLUSTERED(ID ASC)
);
go
--Bang Khach Hang
CREATE TABLE KhachHang
(
    IDkh INT IDENTITY (1, 1) NOT NULL,
    TenKH NVARCHAR(50) NULL,
    SoDT NVARCHAR(11) NULL,
    Email NVARCHAR(255) NULL,
    TKhoan NVARCHAR(50) NULL,
    MKhau NVARCHAR(50) NULL,
	IsActive NVARCHAR(50) NOT NULL DEFAULT N'Hoạt động',
	[OTP] NVARCHAR(6),        
    [OTPExpiry] DATETIME
    PRIMARY KEY CLUSTERED (IDkh ASC)
);
go
--Bảng tác giả:
CREATE TABLE TacGia
(
    IDtg INT IDENTITY(1,1) NOT NULL,           
    TenTacGia NVARCHAR(100) NOT NULL,          
    NgaySinh DATE NULL,                        
    QuocGia NVARCHAR(50) NULL,                
    TieuSu NVARCHAR(MAX) NULL,                 
    PRIMARY KEY CLUSTERED (IDtg ASC)           
);
GO
--Bảng NhaXuatBan:
CREATE TABLE NhaXuatBan
(
    IDnxb INT IDENTITY(1,1) NOT NULL,          
    Tennxb NVARCHAR(100) NOT NULL,      
    DiaChi NVARCHAR(max) NULL,                 
    SoDienThoai NVARCHAR(11) NULL,             
    Email NVARCHAR(100) NULL,                  
    PRIMARY KEY CLUSTERED (IDnxb ASC)          
);
GO
-- Bảng Khuyến Mãi
CREATE TABLE KhuyenMai
(
    IDkm INT IDENTITY(1, 1) NOT NULL,
    TenKhuyenMai NVARCHAR(100) NOT NULL,
    NgayBatDau DATE  NULL,
    NgayKetThuc DATE  NULL,
    MucGiamGia INT NULL, 
    MoTa NVARCHAR(MAX) NULL,
	NgayTao Datetime null,
    PRIMARY KEY CLUSTERED (IDkm ASC)
);
go
--Bang San Pham
CREATE TABLE SanPham
(
    IDsp INT IDENTITY (1, 1) NOT NULL,        
    TenSP NVARCHAR(255) NOT NULL,             
    MoTa NVARCHAR(max) NULL,                  
    TheLoai INT NOT NULL,                     
    GiaBan DECIMAL(18,2) NOT NULL,            
    HinhAnh NVARCHAR(255) NOT NULL,           
    IDtg INT NOT NULL,                        
    IDnxb INT  NOT NULL,                      
    IDkm INT not null,                      
    SoLuong INT NOT NULL,                     
    TrangThaiSach NVARCHAR(50) NULL,          
    PRIMARY KEY CLUSTERED (IDsp ASC),         
    CONSTRAINT FK_SanPham_DanhMuc FOREIGN KEY (TheLoai) REFERENCES DanhMuc (ID),
    CONSTRAINT FK_SanPham_TacGia FOREIGN KEY (IDtg) REFERENCES TacGia (IDtg),
    CONSTRAINT FK_SanPham_NhaXuatBan FOREIGN KEY (IDnxb) REFERENCES NhaXuatBan (IDnxb),
	CONSTRAINT FK_SanPham_KhuyenMai FOREIGN KEY (IDkm) REFERENCES KhuyenMai (IDkm)
);

GO
--Bang Don Hang
CREATE TABLE DonHang
(
    IDdh INT IDENTITY (1, 1) NOT NULL,
    NgayDatHang DATE NULL,
    IDkh INT NULL,
    DiaChi NVARCHAR(255) NULL,
    NgayNhanHang DATE NULL,
    TrangThai NVARCHAR(30) NULL,
    PRIMARY KEY CLUSTERED (IDdh ASC),
    FOREIGN KEY (IDkh) REFERENCES KhachHang (IDkh)
);
go
--Bang Don Hang Chi Tiet
CREATE TABLE DonHangCT
( 
	IDdh INT IDENTITY(1,1) NOT NULL,
	IDSanPham INT NULL,
	IDDonHang INT NULL,
	SoLuong INT NULL,
	Gia FLOAT (53) NULL,
	DanhGia NVARCHAR(MAX) NULL,
	PRIMARY KEY CLUSTERED (IDdh ASC),
	FOREIGN KEY (IDSanPham) REFERENCES SanPham (IDsp),
	FOREIGN KEY (IDDonHang) REFERENCES DonHang (IDdh)
);
go
CREATE TRIGGER trg_UpdateTrangThaiSach
ON [Sach].[dbo].[SanPham]
AFTER INSERT, UPDATE
AS
BEGIN
        UPDATE [Sach].[dbo].[SanPham]
    SET [TrangThaiSach] = 'Hết hàng'
    WHERE [SoLuong] = 0
      AND [IDsp] IN (SELECT [IDsp] FROM inserted)
    
    UPDATE [Sach].[dbo].[SanPham]
    SET [TrangThaiSach] = 'Còn hàng'
    WHERE [SoLuong] > 0
      AND [IDsp] IN (SELECT [IDsp] FROM inserted)
END
go
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
END

