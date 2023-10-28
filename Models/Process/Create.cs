namespace QLMB.Models.Process
{
    public class Create
    {
        private static string DefaultPassword = "123456";
        //Thêm dữ liệu
        public static (bool,string) Employee(database db, ThongTinND thongTin, ListChucVu chucVu)
        {
            try
            {
                string authTmp = SHA256.ToSHA256(DefaultPassword);

                ThongTinND info = new ThongTinND();
                info.CMND = thongTin.CMND.Trim();
                info.NgayCap = thongTin.NgayCap;

                info.HoTen = thongTin.HoTen.ToString();
                info.GioiTinh = thongTin.GioiTinh.ToString();
                info.NgaySinh = thongTin.NgaySinh;
                info.DiaChi = thongTin.DiaChi.ToString();

                NhanVien account = new NhanVien();
                account.CMND = thongTin.CMND.Trim();
                account.MaNV = chucVu.MaChucVu.Trim() + Shared.CreateID(db, chucVu.MaChucVu).ToString();
                account.MatKhau = authTmp.Trim();
                account.MaChucVu = chucVu.MaChucVu.Trim();
                account.MATT = 6;

                db.ThongTinNDs.Add(info);
                db.NhanViens.Add(account);

                db.SaveChanges();

                return (true, "Đăng ký thành công");
            }
            catch { return (false, "Tạo tài khoản thất bại - Xin vui lòng thử lại !"); }
        }

        public static (bool,string) Customer(database db, ThongTinND thongTin, string username, string password)
        {
            try
            {
                string authTmp = SHA256.ToSHA256(password);

                ThongTinND info = new ThongTinND();
                info.CMND = thongTin.CMND.Trim();
                info.NgayCap = thongTin.NgayCap;

                info.HoTen = thongTin.HoTen.ToString();
                info.GioiTinh = thongTin.GioiTinh.ToString();
                info.NgaySinh = thongTin.NgaySinh;
                info.DiaChi = thongTin.DiaChi.ToString();

                NguoiThue account = new NguoiThue();
                account.CMND = thongTin.CMND.Trim();
                account.TenDangNhap = username.Trim();
                account.MatKhau = authTmp.Trim();

                db.ThongTinNDs.Add(info);
                db.NguoiThues.Add(account);
                db.SaveChanges();

                return (true, "Đăng ký thành công");
            }
            catch { return (false, "* Bạn đã có tài khoản ! Vui lòng đăng nhập"); }
        }
    }
}