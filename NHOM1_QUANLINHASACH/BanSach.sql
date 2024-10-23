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
	Email	   NCHAR(50) NOT NULL,
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
	TenKH NVARCHAR (255) NULL,
	SoDT NVARCHAR(15) NULL,
	Email NVARCHAR(255) NUll,
	TKhoan	   NCHAR(50) NULL,
	MKhau	   NCHAR(50) NULL,
	PRIMARY KEY CLUSTERED (IDkh ASC)
)
go
--Bảng tác giả:
CREATE TABLE TacGia
(
    IDtg INT IDENTITY(1,1) NOT NULL,           
    TenTacGia NVARCHAR(255) NOT NULL,          
    NgaySinh DATE NULL,                        
    QuocGia NVARCHAR(100) NULL,                
    TieuSu NVARCHAR(MAX) NULL,                 
    PRIMARY KEY CLUSTERED (IDtg ASC)           
);
GO
--Bảng NhaXuatBan:
CREATE TABLE NhaXuatBan
(
    IDnxb INT IDENTITY(1,1) NOT NULL,          
    Tennxb NVARCHAR(255) NOT NULL,      
    DiaChi NVARCHAR(255) NULL,                 
    SoDienThoai NVARCHAR(20) NULL,             
    Email NVARCHAR(255) NULL,                  
    PRIMARY KEY CLUSTERED (IDnxb ASC)          
);
GO
--Bang San Pham
CREATE TABLE SanPham
(
    IDsp INT IDENTITY (1, 1) NOT NULL,        -- Mã sản phẩm
    TenSP NVARCHAR(255) NOT NULL,             -- Tên sản phẩm (sách)
    MoTa NVARCHAR(MAX) NULL,                  -- Mô tả sản phẩm
    TheLoai INT NOT NULL,                     -- Thể loại (liên kết với DanhMuc)
    GiaBan DECIMAL(18,2) NOT NULL,            -- Giá bán
    HinhAnh NVARCHAR(MAX) NOT NULL,           -- Hình ảnh của sản phẩm
    IDtg INT NOT NULL,                        -- Mã tác giả (liên kết với TacGia)
    IDnxb INT  NOT NULL,                      -- Mã nhà xuất bản (liên kết với NhaXuatBan)
    NamXB DATE NOT NULL,                      -- Năm xuất bản
    SoLuong INT NOT NULL,                     -- Số lượng tồn kho
    TrangThaiSach NVARCHAR(50) NULL,          -- Trạng thái sách (mới, cũ, còn hàng, hết hàng,...)
    PRIMARY KEY CLUSTERED (IDsp ASC),         -- Khóa chính là IDsp
    -- Khóa ngoại tham chiếu đến bảng DanhMuc (cho thể loại)
    CONSTRAINT FK_SanPham_DanhMuc FOREIGN KEY (TheLoai) REFERENCES DanhMuc (ID),
    -- Khóa ngoại tham chiếu đến bảng TacGia (cho tác giả)
    CONSTRAINT FK_SanPham_TacGia FOREIGN KEY (IDtg) REFERENCES TacGia (IDtg),
    -- Khóa ngoại tham chiếu đến bảng NhaXuatBan (cho nhà xuất bản)
    CONSTRAINT FK_SanPham_NhaXuatBan FOREIGN KEY (IDnxb) REFERENCES NhaXuatBan (IDnxb)
);
GO
--Bang Don Hang
CREATE TABLE DonHang
(
	IDdh INT IDENTITY (1, 1) NOT NULL,
	NgayDatHang DATE NULL,
	IDkh INT NULL,
	DiaChi NVARCHAR (255) NULL,
	NgayNhanHang DATE NULL,
	TrangThai nvarchar(30) null,
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
SELECT * FROM TacGia;
