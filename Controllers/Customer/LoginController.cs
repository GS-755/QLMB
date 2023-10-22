using QLMB.Models;
using System.Linq;
using System.Web.Mvc;

namespace QLMB.Controllers.Customer
{
    public class LoginController : Controller
    {
        private database db = new database();

        //Trang đăng nhập (Chung)
        public ActionResult Login()
        {
            return View();
        }

        //Trang đăng nhập (Nhân viên)
        public ActionResult StaffLogin()
        {
            return View();
        }

        
        //Đăng xuất
        public ActionResult Logout()
        {
            Session.Abandon();
            return RedirectToAction("Index", "Home");
        }


        //POST đăng nhập (Chung)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string username, string password)
        {
            try
            {
                //Nếu tên đăng nhập > 8 ký tự ==> Người thuê
                if (rentalCheckLogin(username, password))
                    return RedirectToAction("Index", "Home");

                //Còn lại ==> Nhân viên
                (bool, NhanVien) result = ManagerCheckLogin(username, password);
                if (result.Item1)
                {
                    switch (result.Item2.MATT)
                    {
                        case 5:
                            return RedirectToAction("Banned", "Account");
                        case 6:
                            return RedirectToAction("FirstLogin", "Account", new { MANV = result.Item2.MaNV });
                        default:
                            return RedirectToAction("Manager", "Account");
                    }
                }
                return View("Login");
            }
            catch
            {
                return RedirectToAction("Index", "SkillIssue");
            }
        }


        //POST đăng nhập (Nhân viên)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult StaffLogin(string username, string password)
        {
            try
            {
                (bool, NhanVien) result = ManagerCheckLogin(username, password);
                if (result.Item1)
                {
                    switch (result.Item2.MATT)
                    {
                        case 5:
                            return RedirectToAction("Banned", "Account");
                        case 6:
                            return RedirectToAction("FirstLogin", "Account", new { MANV = result.Item2.MaNV });
                        default:
                            return RedirectToAction("Manager", "Account");
                    }
                }
                return View("StaffLogin");
            }
            catch
            {
                return RedirectToAction("Index", "SkillIssue");
            }
        }


        //Kiểm tra thông tin đăng nhập
        //Người thuê
        private bool rentalCheckLogin(string TenDangNhap, string MatKhau)
        {
            (bool, string) username = Validation.Username(TenDangNhap);
            (bool, string) password = Validation.Password(MatKhau);

            if (username.Item1 && password.Item1)
            {
                (bool, string, NguoiThue) checkLogin = Validation.checkLoginRental(TenDangNhap, MatKhau);

                if (checkLogin.Item1)
                {
                    ThongTinND data = db.ThongTinNDs.Where(a => a.CMND == checkLogin.Item3.CMND).First();
                    Session["AccountName"] = data.HoTen;
                    return true;
                }
                ModelState.AddModelError("Error", checkLogin.Item2);
                return false;
            }

            if (!username.Item1)
                ModelState.AddModelError("inputUsername", username.Item2);
                
            if (!password.Item1)
                ModelState.AddModelError("inputPassword", password.Item2);

            //Thông tin sai
            return false;
        }

        //Nhân viên
        private (bool, NhanVien) ManagerCheckLogin(string MaNV, string MatKhau)
        {
            (bool, string) username = Validation.Username(MaNV);
            (bool, string) password = Validation.Password(MatKhau);

            if (username.Item1 && password.Item1)
            {
                (bool, string, NhanVien) checkLogin = Validation.checkLoginEmployee(MaNV, MatKhau);

                //Thấy thông tin => Thông tin đúng
                if (checkLogin.Item1)
                {
                    string[] name = checkLogin.Item3.ThongTinND.HoTen.Split(' ');

                    //Xử lý độ dài tên: Độ dài lớn hơn 1 mới bị cắt 2 tên cuối
                    if (name.Length == 1)
                        Session["AccountName"] = name[0];
                    else
                        Session["AccountName"] = name[name.Length - 2] + " " + name[name.Length - 1];

                    ThongTinND employeeInfo = db.ThongTinNDs.Where(s => s.CMND == checkLogin.Item3.CMND).FirstOrDefault();

                    Session["EmployeeInfo"] = checkLogin.Item3;
                    Session["UserInfo"] = employeeInfo;

                    return (true, checkLogin.Item3);
                }
                else
                {
                    ModelState.AddModelError("Error", "* Tài khoản hoặc mật khẩu không đúng");
                }
            }


            if (!username.Item1)
                ModelState.AddModelError("inputUsername", username.Item2);

            if (!password.Item1)
                ModelState.AddModelError("inputPassword", password.Item2);
              

            //Thông tin sai
            return (false, null);
        }
    }
}

/*
Mã tình trạng
    1: Đang chờ duyệt
    2: Được duyệt
    3: Bị từ chối
    4: Đang làm   *
    5: Nghỉ việc  *
    6: Được tuyển *

    *: Được sử dụng 
 */