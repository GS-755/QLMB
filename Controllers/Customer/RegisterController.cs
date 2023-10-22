using QLMB.Models;
using QLMB.Models.Process;
using System.Linq;
using System.Web.Mvc;

namespace QLMB.Controllers.Customer
{
    public class RegisterController : Controller
    {
        private database db = new database();
        private ThongTinND info = new ThongTinND();

        //----------- Người thuê -----------//
        //Trang đăng ký
        public ActionResult rentalInfo()
        {
            return View();
        }


        //Xử lý thông tin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RentalInfo(ThongTinND thongTin, string username, string password, string rePassword)
        {
            if (checkInfo(thongTin, username, password, rePassword))
            {
                (bool, string) checkAccount = Validation.ExistAccount(db, thongTin.CMND, thongTin.HoTen);

                if (checkAccount.Item1)
                {
                    (bool, string) saveInfo = Create.Customer(db, thongTin, username, password);

                    if (saveInfo.Item1)
                    {
                        Session.Remove("PrevUsername");
                        TempData["msg"] = $"<script>alert('{saveInfo.Item2}');</script>";
                        return RedirectToAction("Login", "Login");
                    }
                    else
                        ModelState.AddModelError("TrungTaiKhoan", saveInfo.Item2);
                }
                else
                    ModelState.AddModelError("TrungCMND", checkAccount.Item2);
            }
            Session["PrevUsername"] = username;
            return View();
        }

        //Kiểm tra thông tin
        private bool checkInfo(ThongTinND thongTin, string username, string password, string rePassword)
        {
            (bool, string) CMND = Validation.CMND(thongTin.CMND);
            (bool, string) NgayCap = Validation.NgayCap(thongTin.NgayCap);
            (bool, string) HoTen = Validation.HoTen(thongTin.HoTen);
            (bool, string) GioiTinh = Validation.Gender(thongTin.GioiTinh);
            (bool, string) NgaySinh = Validation.Birthday_25(thongTin.NgaySinh);
            (bool, string) DiaChi = Validation.Address(thongTin.DiaChi);

            (bool, string) TenDangNhap = Validation.Username_8(username);
            (bool, string) MatKhau = Validation.Password(password);
            (bool, string) NhapLaiMatKhau = Validation.rePassword(password, rePassword);
            
            bool check = CMND.Item1 && NgayCap.Item1 && HoTen.Item1 && GioiTinh.Item1 &&
                         NgaySinh.Item1 && DiaChi.Item1 && TenDangNhap.Item1 && MatKhau.Item1 && NhapLaiMatKhau.Item1;

            if (check)
                return true;

            //CMND chưa nhập
            if (!CMND.Item1)
                ModelState.AddModelError("CMND", CMND.Item2);


            //Ngày cấp
            if (!NgayCap.Item1) 
                ModelState.AddModelError("NgayCapCMND", NgayCap.Item2);

            //Họ tên
            if (!HoTen.Item1)
                ModelState.AddModelError("HoTen", HoTen.Item2);


            //Giới tính
            if (!GioiTinh.Item1)
                ModelState.AddModelError("GioiTinh", GioiTinh.Item2);


            //Ngày sinh
            if (!NgaySinh.Item1)
                ModelState.AddModelError("NgaySinhND", NgaySinh.Item2);
            
            //Địa chỉ
            if(!DiaChi.Item1)
                ModelState.AddModelError("DiaChi", DiaChi.Item2);


            //Tên đăng nhập
            if (!TenDangNhap.Item1)
                ModelState.AddModelError("TenDangNhap", TenDangNhap.Item2);

            //Mật khẩu
            if (!MatKhau.Item1)
                ModelState.AddModelError("MatKhau", MatKhau.Item2);

            //Nhập lại mật khẩu
            if(!NhapLaiMatKhau.Item1)
                ModelState.AddModelError("MatKhauLai", NhapLaiMatKhau.Item2); 

            return false;
        }    }
}