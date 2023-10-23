using QLMB.Models;
using QLMB.Models.Process;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Web.Mvc;
using System.Xml.Linq;

namespace QLMB.Controllers
{
    public class AccountController : Controller
    {
        private database db = new database();
        
        //Quản lý chuyển trang
        public ActionResult Manager()
        {
            if (Session["EmployeeInfo"] == null)
                return RedirectToAction("Index", "Home");
            if (((NhanVien)Session["EmployeeInfo"]).MATT == 5)
                return Redirect("Banned");

            switch (((NhanVien)Session["EmployeeInfo"]).MaChucVu.Trim())
            {
                case "NS":
                    return RedirectToAction("Main", "HumanResource");
                case "SKUD":
                    return RedirectToAction("EventMain", "Event");
                default:
                    return RedirectToAction("Index", "Home");
            }
        }

        //Trả về
        public ActionResult Return()
        {
            if (Session["Page"] == null)
                return Manager();
            switch (Session["Page"].ToString().Trim())
            {
                //Trang sự kiện
                case "EventMain":
                    return RedirectToAction("EventMain", "Event");

                //Trang ưu đãi
                case "SaleMain":
                    return RedirectToAction("EventMain", "Event");

                //Trang chi tiết (Sự kiện ưu đãi)
                case "EventDetail":
                    return RedirectToAction("Detail", "Event");
                
                //Trang quản lý nhân viên
                case "EmployeeMain":
                    return RedirectToAction("Main", "HumanResource");

                //Trang thêm tài khoản nhân viên
                case "EmployeeRegister":
                    return RedirectToAction("Register", "HumanResource");

                //Trang xem chi tiết (Nhân viên)
                case "EmployeeDetail":
                    return RedirectToAction("Detail", "HumanResource");

                //Bị ban
                case "Banned":
                    return Redirect("Banned");

                //Đăng nhập lần đầu
                case "FirstLogin":
                    return Redirect("FirstLogin");
                default:
                    return Manager();
            }
        }

        //Ban tài khoản
        public ActionResult Banned()
        {
            //Dùng để xử lý về lại trang trước đó
            Session["Page"] = "Banned";
            return View();
        }


        //Đổi mật khẩu (Đăng nhập lần đầu)
        public ActionResult FirstLogin(string MANV)
        {
            if(Session["EmployeeInfo"]  == null)
                return Manager();

            if ( ((NhanVien)Session["EmployeeInfo"]).MATT == 6)
            {
                //Dùng để xử lý về lại trang trước đó
                Session["Page"] = "FirstLogin";
                return View(db.NhanViens.Where(s => s.MaNV == MANV).FirstOrDefault());
            }
            return Manager();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult FirstLogin(NhanVien info, string rePass)
        {
            if (checkFirstTime(info, rePass))
            {
                if (Edit.EployeeFirstLogin(db, info))
                {
                    Session["EmployeeInfo"] = db.NhanViens.Where(s => s.MaNV.Trim() == info.MaNV.Trim()).FirstOrDefault();
                    return Manager();
                }
                    

                ModelState.AddModelError("changeError", "* Đổi mật khẩu không thành công - Vui lòng thử lại");
            }
            return View();
        }



        //Tổng quan
        public ActionResult General()
        {
            if (Session["UserInfo"] == null || Session["EmployeeInfo"] == null)
                return Manager();

            string CMND = ((ThongTinND)Session["UserInfo"]).CMND.Trim();
            return View(db.ThongTinNDs.Where(s => s.CMND == CMND.Trim()).FirstOrDefault());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult General(ThongTinND info)
        {
            if (checkGeneral(info))
            {
                (bool, string) saveProfile = Edit.EmployeeProfile(db, info);
                
                if (saveProfile.Item1)
                {
                    Session["UserInfo"] = info;

                    string[] name = info.HoTen.Split(' ');

                    //Xử lý độ dài tên: Độ dài lớn hơn 1 mới bị cắt 2 tên cuối
                    if (name.Length == 1)
                        Session["AccountName"] = name[0];
                    else
                        Session["AccountName"] = name[name.Length - 2] + " " + name[name.Length - 1];

                    TempData["msg"] = $"<script>alert('{saveProfile.Item2}');</script>";
                }
                else
                    ModelState.AddModelError("ProfileFaield", saveProfile.Item2);
            }
            
            return View(info);
        }


        //Đổi mật khẩu
        public ActionResult ChangePassword()
        {
            if (Session["EmployeeInfo"] == null || ((NhanVien)Session["EmployeeInfo"]).MATT != 4)
                return Manager();

            string MANV = ((NhanVien)Session["EmployeeInfo"]).MaNV.Trim();
            return View(db.NhanViens.Where(s => s.MaNV == MANV.Trim()).FirstOrDefault());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(NhanVien info, string currentPass, string rePass)
        {
            if (checkChangePassword(info, currentPass, rePass))
            {
                (bool, string) savePassword = Edit.EmployeePassword(db, info);

                if(savePassword.Item1)
                    TempData["msg"] = "<script>alert('Đổi mật khẩu thành công');</script>";
                else
                    ModelState.AddModelError("passwordFailed", savePassword.Item2);
            }

            return View(info);
        }



        //*-- Kiểm tra Đăng nhập lần đầu --*//
        private bool checkFirstTime(NhanVien info, string rePass)
        {
            (bool, string) MatKhau = Validation.Password(info.MatKhau);
            (bool, string) NhapLaiMatKhau = Validation.rePassword(info.MatKhau, rePass);

            if (MatKhau.Item1 && NhapLaiMatKhau.Item1)
                return true;

            //Mật khẩu
            if (!MatKhau.Item1)
                ModelState.AddModelError("employeePass", MatKhau.Item2);

            //Nhập lại mật khẩu
            if (!NhapLaiMatKhau.Item1)
                ModelState.AddModelError("reEmployeePass", NhapLaiMatKhau.Item2);

            return false;
        }

        //*-- Kiểm tra Tổng quát --*//
        private bool checkGeneral(ThongTinND info)
        {
            (bool, string) HoTen = Validation.HoTen(info.HoTen);
            (bool, string) NgayCap = Validation.NgayCap(info.NgayCap);
            (bool, string) DiaChi = Validation.Address(info.DiaChi);

            if (HoTen.Item1 && NgayCap.Item1 && DiaChi.Item1)
                return true;

            if (!HoTen.Item1)
                ModelState.AddModelError("generalName", HoTen.Item2);

            if (!NgayCap.Item1)
                ModelState.AddModelError("generalNgayCap", NgayCap.Item2);

            if (!DiaChi.Item1)
                ModelState.AddModelError("generalAddress", DiaChi.Item2);

            return false;
        }

        //*-- Kiểm tra Đổi mật khẩu --*//
        private bool checkChangePassword(NhanVien info, string current, string rePass)
        {
            (bool, string) currentPassword = Validation.CurrentPasswordEmployee(info.MaNV, current);
            (bool, string) password = Validation.Password(info.MatKhau);
            (bool, string) rePassword = Validation.Password(rePass);

            if(currentPassword.Item1 && password.Item1 && rePassword.Item1)
                 return true;

            if(!currentPassword.Item1)
                ModelState.AddModelError("changeCurrentPassword", currentPassword.Item2);

            if(!password.Item1)
                ModelState.AddModelError("changePassword", password.Item2);
            
            if(!rePassword.Item1)
                ModelState.AddModelError("changeRePassword", rePassword.Item2);

            return false;
        }
    }
}

/*
Mã tình trạng (* = Được sử dụng)
    1: Đang chờ duyệt
    2: Được duyệt
    3: Bị từ chối
    4: Đang làm   *
    5: Nghỉ việc  *
    6: Được tuyển *
 */