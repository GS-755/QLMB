using System;
using System.Linq;

namespace QLMB.Models
{
    public class Validation
    {
        //*--- Kiểm 1 trường ---*//

        //Tên đăng nhập
        public static (bool,string) Username(string value)
        {
            if (value == null || value == "")
                return (false, "* Xin hãy điền tên đăng nhập");
            
            return (true,null);
        }

        //Tên đăng nhập (Trên 8 ký tự)
        public static (bool, string) Username_8(string value)
        {
            if (value == null || value == "")
                return (false, "* Xin hãy điền tên đăng nhập");

            if(value.Trim().Length < 8)
                return (false, "* Tên đăng nhập phải dài hơn 8 ký tự");

            return (true, null);
        }

        //Mật khẩu
        public static (bool,string) Password(string value)
        {
            if (value == null || value == "") 
                return (false, "* Xin hãy điền mật khẩu");
            
            return (true,null);
        }

        //Chứng minh nhân dân
        public static (bool, string) CMND(string value)
        {
            if (value == null || value == "")
                return (false, "* Xin hãy điền CMND/CCCD");

            //CMND không đủ 12 số
            if (value.Trim().Length != 12)
                return (false, "* CMND/CCCD phải đủ 12 số");

            //CMND có chứa chữ
            char[] numberCheck = value.Trim().ToCharArray();

            for (int i = 0; i < value.Trim().Length; i++)
            {
                if (!char.IsNumber(numberCheck[i]))
                    return (false, "* CMND/CCCD không hợp lệ");
            }

            return (true, null);
        }

        //Ngày cấp
        public static (bool,string) NgayCap(DateTime value)
        {
            if (value == null)
                return (false, "* Xin hãy nhập ngày cấp");
            if (value.Year < 1900 || value > DateTime.Now) 
                return (false, "* Ngày cấp không hợp lệ");

            return (true, null);
        }

        //Họ tên
        public static (bool,string) HoTen(string value)
        {
            if (value == null || value == "")
                return (false, "* Xin hãy điền họ tên");

            return (true, null);
        }

        //Giới tính
        public static (bool, string) Gender(string value)
        {
            if (value == null || value == "")
                return (false, "* Xin hãy chọn giới tính");

            return (true, null);
        }

        //Ngày sinh
        public static (bool, string) Birthday(DateTime value)
        {
            if (value == null)
                return (false, "* Xin hãy nhập ngày sinh");
            if (value.Year < 1800)
                return (false, "* Ngày sinh không hợp lệ");

            return (true, null);
        }

        //Ngày sinh (Lớn hơn 25 tuổi)
        public static (bool, string) Birthday_25(DateTime value)
        {
            (bool, string) checkBirthday = Birthday(value);

            if (!checkBirthday.Item1)
                return (false, checkBirthday.Item2);

            if (value.Year > DateTime.Now.Year - 25)
                return (false, "* Bạn chưa đủ tuổi để đăng ký");

            return (true, null);
        }

        //Địa chỉ
        public static (bool, string) Address(string value) 
        {
            if (value == null || value == "")
                return (false, "* Xin hãy nhập địa chỉ");

            return (true, null);
        }

        //Chức vụ
        public static (bool, string) Role(string value)
        {
            if (value == null || value == "" || value.Trim() == "Default")
                return (false, "* Xin hãy chọn chức vụ");
           
            return (true, null);
        }




        //*--- Kiểm 2 trường trở lên ---*//
        //Nhập lại mật khẩu
        public static (bool, string) rePassword(string pass, string rePass)
        {
            if (rePass == null || rePass == "")
                return (false, "* Xin hãy điền lại mật khẩu");
            
            if (!Password(pass).Item1)
                return (false, " ");

            if (pass != rePass)
                return (false, "* Mật khẩu không khớp - Xin hãy điền lại");

            return (true, null);
        }


        //*--- Cần database ---*//
        //Check đăng nhập - Người thuê
        public static (bool,string,NguoiThue) checkLoginRental(string username, string password)
        {
            database db = new database();
            string authTmp = SHA256.ToSHA256(password.Trim());
            NguoiThue info = db.NguoiThues.Where(a => a.TenDangNhap.Trim() == username.Trim() && a.MatKhau.Trim() == authTmp).FirstOrDefault();

            if (info == null)
                return (false, "* Tài khoản hoặc mật khẩu không đúng", null);

            return (true, null, info);
        }

        //Check đăng nhập - Nhân viên
        public static (bool, string, NhanVien) checkLoginEmployee(string username, string password)
        {
            database db = new database();
            string authTmp = SHA256.ToSHA256(password);
            NhanVien info = db.NhanViens.Where(a => a.MaNV.Trim() == username.Trim() && a.MatKhau.Trim() == authTmp).FirstOrDefault();

            if (info == null)
                return (false, "* Tài khoản hoặc mật khẩu không đúng", null);

            return (true, null, info);
        }

        //Check Chứng minh nhân dân đã tồn tại - Nhân viên
        public static (bool, string) ExistCMND(string value)
        {
            (bool, string) checkCMND = CMND(value);
            if (!checkCMND.Item1)
                return (false, checkCMND.Item2);

            database db = new database();
            NhanVien info = db.NhanViens.Where(a => a.CMND.Trim() == value.Trim()).FirstOrDefault();

            if (info != null)
                return (false, "* Số CMND/CCCD này đã tồn tại trên hệ thống !");

            return (true, null);
        }

        //Check mật khẩu cũ - Nhân viên
        public static (bool, string) CurrentPasswordEmployee(string MANV, string password)
        {
            if (!Password(password).Item1)
                return (false, "* Xin hãy điền mật khẩu trước đó của bạn");

            database db = new database();

            string authTmp = SHA256.ToSHA256(password.Trim());
            NhanVien info = db.NhanViens.Where(a => a.MaNV.Trim() == MANV.Trim() && a.MatKhau == authTmp).FirstOrDefault();

            if (info == null)
                return (false, "* Mật khẩu không đúng - Xin mời nhập lại");

            return (true, null);
        }
    }
}