--Server name: (local)
--Server account: use Windows Authentication
--In case of losing Windows Account because of Microsoft Account Authentication:
--Username: sa
--Password: (leave blank)

USE MASTER
GO
IF EXISTS (
	SELECT * 
	FROM sys.databases 
	WHERE name = 'DoanHK5'
) BEGIN
	DROP DATABASE DoanHK5
END;
CREATE DATABASE DoanHK5;
USE DoanHK5;

--Chức vụ
CREATE TABLE ChucVu
(
  MaChucVu CHAR(10) NOT NULL PRIMARY KEY,
  TenCV NVARCHAR(100) NOT NULL,
  HeSoLuong FLOAT NOT NULL,
);

INSERT INTO ChucVu VALUES('MB',N'Mặt bằng',1)
INSERT INTO ChucVu VALUES('NS',N'Nhân sự',1)
INSERT INTO ChucVu VALUES('SKUD',N'Sự kiện - Ưu đãi',1)

--Tình trạng
CREATE TABLE TinhTrang
(
  MATT SMALLINT NOT NULL PRIMARY KEY,
  TenTT NVARCHAR(100) NOT NULL,
);

INSERT INTO TinhTrang VALUES(1,N'Đang chờ duyệt')
INSERT INTO TinhTrang VALUES(2,N'Được duyệt')
INSERT INTO TinhTrang VALUES(3,N'Bị từ chối')
INSERT INTO TinhTrang VALUES(4,N'Đang làm')
INSERT INTO TinhTrang VALUES(5,N'Nghỉ việc')
INSERT INTO TinhTrang VALUES(6,N'Được tuyển')
INSERT INTO TinhTrang VALUES(7,N'Còn trống')
INSERT INTO TinhTrang VALUES(8,N'Đang thuê')
INSERT INTO TinhTrang VALUES(9,N'Nợ tiền thuê')

--Thông tinND
CREATE TABLE ThongTinND
(
  CMND CHAR(12) NOT NULL PRIMARY KEY,
  NgayCap DATE NOT NULL,
  HoTen NVARCHAR(100) NOT NULL,
  GioiTinh NVARCHAR(3) NOT NULL,
  NgaySinh DATE NOT NULL,
  DiaChi NVARCHAR(200) NOT NULL,
);

--Mặt bằng
CREATE TABLE MatBang
(
  MaMB CHAR(6) NOT NULL PRIMARY KEY,
  Lau INT NOT NULL,
  DienTich FLOAT NOT NULL,
  Khu INT NOT NULL,
  TienThue FLOAT NOT NULL,
  MATT SMALLINT NOT NULL ,
  FOREIGN KEY (MATT) REFERENCES TinhTrang(MATT)
);

--Hợp đồng
CREATE TABLE HopDong
(
  MaHD CHAR(20) NOT NULL PRIMARY KEY,
  ThoiGianDuyet INT NOT NULL,
);

--Danh mục
CREATE TABLE DanhMuc
(
  MaDM CHAR(4) NOT NULL PRIMARY KEY,
  TenDM NVARCHAR(50) NOT NULL,
);

INSERT INTO DanhMuc VALUES('SK',N'Sự kiện')
INSERT INTO DanhMuc VALUES('UD',N'Ưu đãi')

--Tiền thuê
CREATE TABLE TienThue
(
  MaTienThue CHAR(40) NOT NULL PRIMARY KEY,
  SoTienNo CHAR(20) NOT NULL,
  MaHD CHAR(20) NOT NULL,
  FOREIGN KEY (MaHD) REFERENCES HopDong(MaHD)
);

--Người thuê
CREATE TABLE NguoiThue
(
  TenDangNhap CHAR(50) NOT NULL PRIMARY KEY,
  MatKhau CHAR(64) NOT NULL,
  CMND CHAR(12) NOT NULL,
  FOREIGN KEY (CMND) REFERENCES ThongTinND(CMND)
);

--Nhân viên
CREATE TABLE NhanVien
(
  MaNV CHAR(6) NOT NULL PRIMARY KEY,
  MatKhau CHAR(64) NOT NULL,
  MaChucVu CHAR(10) NOT NULL,
  MATT SMALLINT NOT NULL,
  CMND CHAR(12) NOT NULL,
  FOREIGN KEY (MaChucVu) REFERENCES ChucVu(MaChucVu),
  FOREIGN KEY (MATT) REFERENCES TinhTrang(MATT),
  FOREIGN KEY (CMND) REFERENCES ThongTinND(CMND)
);

--Đơn xin thuê
CREATE TABLE DonXinThue
(
  ThoiGianXin DATETIME NOT NULL PRIMARY KEY,
  MucDich NVARCHAR(500) NOT NULL,
  ThoiHan INT NOT NULL,
  TenDangNhap CHAR(50) NOT NULL,
  MaMB CHAR(6) NOT NULL,
  MaNV CHAR(6),
  MATT SMALLINT NOT NULL,
  MaHD CHAR(20) NOT NULL,

  FOREIGN KEY (TenDangNhap) REFERENCES NguoiThue(TenDangNhap),
  FOREIGN KEY (MaMB) REFERENCES MatBang(MaMB),
  FOREIGN KEY (MaNV) REFERENCES NhanVien(MaNV),
  FOREIGN KEY (MATT) REFERENCES TinhTrang(MATT),
  FOREIGN KEY (MaHD) REFERENCES HopDong(MaHD)
);

--Huỷ hợp đồng
CREATE TABLE HuyHopDong
(
  NgayLamDon DATETIME NOT NULL PRIMARY KEY,
  LyDo NVARCHAR(500) NOT NULL,
  NgayDuyet DATE,
  MaHD CHAR(20) NOT NULL,
  MaNV CHAR(6) NOT NULL,
  MATT SMALLINT NOT NULL,
  FOREIGN KEY (MaHD) REFERENCES HopDong(MaHD),
  FOREIGN KEY (MaNV) REFERENCES NhanVien(MaNV),
  FOREIGN KEY (MATT) REFERENCES TinhTrang(MATT)
);

--Sự kiện | Ưu đãi
CREATE TABLE SuKienUuDai
(
  NgayLamDon DATETIME NOT NULL,
  MaDM CHAR(4) NOT NULL,
  TenDangNhap CHAR(50) NOT NULL,
  TieuDe NVARCHAR(100) NOT NULL,
  MoTa NVARCHAR(500),
  HinhBia CHAR(200),
  NgayBatDau DATE NOT NULL,
  NgayKetThuc DATE,
  MaNV CHAR(6),
  NgayDuyet DATE,
  MATT SMALLINT NOT NULL,

  PRIMARY KEY(NgayLamDon, TieuDe, MaDM),
  FOREIGN KEY (MaDM) REFERENCES DanhMuc(MaDM),
  FOREIGN KEY (TenDangNhap) REFERENCES NguoiThue(TenDangNhap),
  FOREIGN KEY (MaNV) REFERENCES NhanVien(MaNV),
  FOREIGN KEY (MATT) REFERENCES TinhTrang(MATT),
);

--Lương
CREATE TABLE Luong
(
  MaLuong INT NOT NULL PRIMARY KEY,
  MaNV CHAR(6) NOT NULL,
  FOREIGN KEY (MaNV) REFERENCES NhanVien(MaNV)
);

--Thu chi
CREATE TABLE ThuChi
(
  ThoiGian DATETIME NOT NULL PRIMARY KEY,
  HinhThuc CHAR(3) NOT NULL,
  SoTien FLOAT NOT NULL,
  NoiDung NVARCHAR(200) NOT NULL,
  MaLuong INT NOT NULL,
  MaNV CHAR(6) NOT NULL,
  MATT SMALLINT NOT NULL,

  FOREIGN KEY (MaLuong) REFERENCES Luong(MaLuong),
  FOREIGN KEY (MaNV) REFERENCES NhanVien(MaNV),
  FOREIGN KEY (MATT) REFERENCES TinhTrang(MATT)
);

SET DATEFORMAT DMY;
--Tài khoản người thuê
INSERT INTO ThongTinND VALUES('098123564889', '20/01/1972',	N'Phạm Đức Thành', N'NAM', '01/01/1954', N'490 CMT8');
INSERT INTO NguoiThue VALUES('phamducthanh2005', '8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92', '098123564889');
--Tài khoản nhân viên
INSERT INTO ThongTinND VALUES('072203008516', '20/09/2022', N'Nguyễn Minh Trí', N'Nam', '17/09/1992', N'42/17 Hồ Thị Kỷ, Phường 1, Quận 10, TP.Hồ Chí Minh');
INSERT INTO NhanVien 
	VALUES('NS1', '8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92', 'NS', 4, '072203008516');
INSERT INTO NhanVien      
	VALUES('SKUD1', '8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92', 'SKUD', 4, '072203008516');

SET DATEFORMAT MDY;
INSERT INTO SuKienUuDai VALUES(GETDATE(),'SK','phamducthanh2005',N'Thử thách “ÁO CHUẨN MAY ĐO”: DETERMINANT',N'Hàng loạt hoạt động thú vị và ưu đãi cho VIP của DETERMINANT mới tại Tầng 3','https://vanhanhmall.com/wp-content/uploads/2023/06/Determinant2-400x400.png','07/26/2023','07/28/2023',NULL,NULL,1)
INSERT INTO SuKienUuDai VALUES(GETDATE(),'SK','phamducthanh2005',N'SỰ KIỆN RA MẮT SON LỲ BLUR MATTE LIPSTICK 3CE – STYLENANDA',N'Sự kiện ra mắt dòng sản phẩm son lỳ mới BLUR MATTE LIPSTICK đến từ thương hiệu nổi tiếng Hàn Quốc 3CE – STYLENANDA','https://vanhanhmall.com/wp-content/uploads/2023/07/Banner-thay-facebook-768x432.png','07/26/2023','07/28/2023',NULL,NULL,1)
INSERT INTO SuKienUuDai VALUES(GETDATE(),'SK','phamducthanh2005',N'HAPPY VIETNAMESE FAMILY DAY – MỪNG NGÀY GIA ĐÌNH VIỆT NAM – 28/6/2023',N'Tình cảm gia đình là món quà tuyệt vời nhất trong cuộc sống. Nơi chia sẻ những buồn vui, đồng hành cùng ta vượt qua những chông gai thử thách.','https://vanhanhmall.com/wp-content/uploads/2023/06/Family-Day-Facebook-Post-400x400.jpg','07/26/2023','07/28/2023',NULL,NULL,1)

INSERT INTO SuKienUuDai VALUES(GETDATE(),'SK','phamducthanh2005',N'HAPPY FATHER’S DAY – MỪNG NGÀY CỦA CHA – 18/6/2023',N'Cha là điểm tựa vững chắc, soi đường để vượt qua mọi khó khăn.','https://vanhanhmall.com/wp-content/uploads/2023/06/Father_s-day-2023-400x400.jpg','07/26/2023','07/28/2023',NULL,NULL,1)
INSERT INTO SuKienUuDai VALUES(GETDATE(),'SK','phamducthanh2005',N'NGÀY MÔI TRƯỜNG THẾ GIỚI – 5.6.2023',N'NGÀY MÔI TRƯỜNG THẾ GIỚI – 5.6.2023 ','https://vanhanhmall.com/wp-content/uploads/2023/06/Environment-Day-instagram-post-400x400.png','07/26/2023','07/28/2023',NULL,NULL,1)
INSERT INTO SuKienUuDai VALUES(GETDATE(),'SK','phamducthanh2005',N'MỪNG ĐẠI LỄ, MỞ TIỆC SALE “HAPPY HOLIDAYS SALE”',N'Mừng đại lễ, Vạn Hạnh Mall mở gian hàng sale đặc biệt tưng bừng với hàng loạt sản phẩm giảm giá lên đến 50% diễn ra từ ngày 14/4 – 23/4/2023 ngay tại sảnh sự kiện tầng trệt.','https://vanhanhmall.com/wp-content/uploads/2023/04/IMG_7119-1067x800.jpg','07/26/2023','07/28/2023',NULL,NULL,1)
INSERT INTO SuKienUuDai VALUES(GETDATE(),'SK','phamducthanh2005',N'Thử thách “ÁO CHUẨN MAY ĐO”: DETERMINANT',N'Hàng loạt hoạt động thú vị và ưu đãi cho VIP của DETERMINANT mới tại Tầng 3','https://vanhanhmall.com/wp-content/uploads/2023/06/Determinant2-400x400.png','07/26/2023','07/28/2023',NULL,NULL,1)
-----------------
INSERT INTO SuKienUuDai VALUES(GETDATE(),'UD','phamducthanh2005',N'HÈ RỰC RỠ CÙNG LỄ HỘI MUA SẮM MÙA HÈ TẠI VẠN HẠNH MALL (16.6 – 23.06.2023)',NULL,'https://vanhanhmall.com/wp-content/uploads/2023/06/Determinant2-400x400.png','07/26/2023','07/28/2023',NULL,NULL,1)
INSERT INTO SuKienUuDai VALUES(GETDATE(),'UD','phamducthanh2005',N'FOREVER SIÊU ƯU ĐÃI KHỦNG THÁNG 11',NULL,'https://vanhanhmall.com/wp-content/uploads/2021/11/Forever-400x400.jpg','07/26/2023','07/28/2023',NULL,NULL,1)
INSERT INTO SuKienUuDai VALUES(GETDATE(),'UD','phamducthanh2005',N'GAVANI SIÊU SAL UP TO 60%',NULL,'https://vanhanhmall.com/wp-content/uploads/2021/11/Gavani-400x400.jpg','07/26/2023','07/28/2023',NULL,NULL,1)
INSERT INTO SuKienUuDai VALUES(GETDATE(),'UD','phamducthanh2005',N'ƯU ĐÃI BLACK FRIDAY TỪ VANS',NULL,'https://vanhanhmall.com/wp-content/uploads/2021/11/Vans-400x400.jpg','07/26/2023','07/28/2023',NULL,NULL,1)
INSERT INTO SuKienUuDai VALUES(GETDATE(),'UD','phamducthanh2005',N'THỂ LỆ CHƯƠNG TRÌNH KHUYẾN MÃI MIỄN PHÍ GIỮ XE THÁNG 2/2023',NULL,'https://vanhanhmall.com/wp-content/uploads/2023/01/Shop-to-park-free-Website-Post-1-400x400.jpg','07/26/2023','07/28/2023',NULL,NULL,1)
INSERT INTO SuKienUuDai VALUES(GETDATE(),'UD','phamducthanh2005',N'LUG.VN KHỞI ĐỘNG MÙA SALE CỰC ĐẬM LÊN ĐẾN 75%',NULL,'https://vanhanhmall.com/wp-content/uploads/2021/11/Lug-1-1.jpg','07/26/2023','07/28/2023',NULL,NULL,1)