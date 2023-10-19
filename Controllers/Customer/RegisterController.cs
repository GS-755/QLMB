using QLMB.Models;
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
        public ActionResult rentalInfo() => View();



        //Xử lý thông tin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RentalInfo(ThongTinND thongTin)
        {
            try
            {    
                if (checkInfo(thongTin))
                {
                    var exist = db.ThongTinNDs.Any(s => s.CMND == thongTin.CMND || s.HoTen == thongTin.HoTen.ToUpper());
                    if (!exist)
                    {
                        exist = db.NguoiThues.Any(s => s.TenDangNhap == thongTin.username);
                        if (!exist)
                        {
                            AddDatabase(thongTin);
                            TempData["msg"] = "<script>alert('Đăng ký thành công');</script>";
                            return RedirectToAction("LoginPage", "Login");
                        }
                        else
                            ModelState.AddModelError("TrungTaiKhoan", "* Tài khoản đã tồn tại");
                        
                    }
                    else
                        ModelState.AddModelError("TrungCMND", "* Bạn đã đăng ký tài khoản !");
                } 
                
                return View();

            }
            catch
            {
                ViewBag.CitizenStatus = "Xác minh thất bại! - Xin hãy thử lại !";
                return View();
            }
        }



        //Kiểm tra thông tin
        private bool checkInfo(ThongTinND thongTin)
        {
            (bool, string) CMND = Validation.CMND(thongTin.CMND);
            (bool, string) NgayCap = Validation.NgayCap(thongTin.NgayCap);
            (bool, string) HoTen = Validation.HoTen(thongTin.HoTen);
            (bool, string) GioiTinh = Validation.Gender(thongTin.GioiTinh);
            (bool, string) NgaySinh = Validation.Birthday_25(thongTin.NgaySinh);
            (bool, string) DiaChi = Validation.Address(thongTin.DiaChi);

            (bool, string) TenDangNhap = Validation.Username_8(thongTin.username);
            (bool, string) MatKhau = Validation.Password(thongTin.password);
            (bool, string) NhapLaiMatKhau = Validation.rePassword(thongTin.password, thongTin.rePassword);
            
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
        }

        //Thêm dữ liệu
        private void AddDatabase(ThongTinND thongTin)
        {
            string authTmp = SHA256.ToSHA256(thongTin.password);
           
            ThongTinND info = new ThongTinND();
            info.CMND = thongTin.CMND.Trim();
            info.NgayCap = thongTin.NgayCap;
            
            info.HoTen = thongTin.HoTen.ToString();
            info.GioiTinh = thongTin.GioiTinh.ToString();
            info.NgaySinh = thongTin.NgaySinh;
            info.DiaChi = thongTin.DiaChi.ToString();

            NguoiThue account = new NguoiThue();
            account.CMND = thongTin.CMND.Trim();
            account.TenDangNhap = thongTin.username.Trim();
            account.MatKhau = authTmp.Trim();

            db.ThongTinNDs.Add(info);
            db.NguoiThues.Add(account);
            db.SaveChanges();
        }  
    }
}