using QLMB.Models;
using System.Linq;
using System.Web.Mvc;

namespace QLMB.Controllers.Customer
{
    public class LoginController : Controller
    {
        private database db = new database();

        //Trang đăng nhập
        public ActionResult Login()
        {
            return View();
        }


        //Đăng xuất
        public ActionResult Logout()
        {
            Session.Abandon();
            return RedirectToAction("Index", "Home");
        }


        //POST đăng nhập
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string TenDangNhap, string MatKhau)
        {
            try
            {
                //Nếu tên đăng nhập > 8 ký tự ==> Người thuê
                if (rentalCheckLogin(TenDangNhap, MatKhau) == true)
                    return RedirectToAction("Index", "Home");

                //Còn lại ==> Nhân viên
                else
                {
                    (bool, NhanVien) result = ManagerCheckLogin(TenDangNhap, MatKhau);
                    if (result.Item1)
                    {
                        switch (result.Item2.MATT)
                        {
                            case 5:
                                return RedirectToAction("Banned", "Account");
                            case 6:
                                Session["MANV"] = result.Item2.MaNV.Trim();
                                return RedirectToAction("FirstLogin", "Account");
                            default:
                                return RedirectToAction("Manager", "Account");
                        }
                    }
                    else
                        return View("Login");
                }
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
            bool error = true;
            if (TenDangNhap == "")
            {
                ModelState.AddModelError("inputUsername", "* Xin hãy điền tên đăng nhập");
                error = false;
            }
            if (MatKhau == "")
            {
                ModelState.AddModelError("inputPassword", "* Xin hãy điền mật khẩu");
                error = false;
            }

            if (error)
            {
                string authTmp = SHA256.ToSHA256(MatKhau);
                var check = db.NguoiThues.Where(a => a.TenDangNhap == TenDangNhap.Trim() && a.MatKhau == authTmp).FirstOrDefault();

                //Thấy thông tin => Thông tin đúng
                if (check != null)
                {
                    var data = db.ThongTinNDs.Where(a => a.CMND == check.CMND).FirstOrDefault();
                    Session["AccountName"] = data.HoTen;

                    return true;
                }
                else
                {
                    ModelState.AddModelError("Error", "* Tài khoản hoặc mật khẩu không đúng");
                    return false;
                }
            }
            //Thông tin sai
            else
            {
                return false;
            }
        }

        //Nhân viên
        private (bool, NhanVien) ManagerCheckLogin(string MaNV, string MatKhau)
        {
            bool error = true;
            if (MaNV == "")
            {
                ModelState.AddModelError("inputUsername", "* Xin hãy điền tên đăng nhập");
                error = false;
            }
            if (MatKhau == "")
            {
                ModelState.AddModelError("inputPassword", "* Xin hãy điền mật khẩu");
                error = false;
            }

            if (error)
            {
                string authTmp = SHA256.ToSHA256(MatKhau);
                NhanVien check = db.NhanViens.Where(a => a.MaNV.Trim() == MaNV.Trim() && a.MatKhau == authTmp).FirstOrDefault();

                //Thấy thông tin => Thông tin đúng
                if (check != null)
                {
                    var data = db.ThongTinNDs.Where(a => a.CMND == check.CMND).FirstOrDefault();
                    string[] name = check.ThongTinND.HoTen.Split(' ');

                    //Xử lý độ dài tên: Độ dài lớn hơn 1 mới bị cắt 2 tên cuối
                    switch (name.Length)
                    {
                        case 0:
                            Session["AccountName"] = name[0];
                            break;
                        default:
                            Session["AccountName"] = name[name.Length - 2] + " " + name[name.Length - 1];
                            break;
                    }
                    
                    Session["Role"] = check.ChucVu.TenCV.ToString();
                    Session["RoleID"] = check.ChucVu.MaChucVu.Trim();

                    return (true, check);
                }
                else
                {
                    ModelState.AddModelError("Error", "* Tài khoản hoặc mật khẩu không đúng");
                    return (false, null);
                }
            }
            //Thông tin sai
            else
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