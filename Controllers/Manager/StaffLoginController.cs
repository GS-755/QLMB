using QLMB.Models;
using System.Linq;
using System.Web.Mvc;

namespace QLMB.Controllers.Manager
{
    public class StaffLoginController : Controller
    {
        private database db = new database();

        //GET: StaffLogin/
        public ActionResult Index()
        {
            return View();
        }
        //POST: StaffLogin/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string MaNV, string MatKhau)
        {
            if (ManagerCheckLogin(MaNV, MatKhau).Item2.Trim() == "SKUD")
                return RedirectToAction("EventMain", "Event");
            else if (ManagerCheckLogin(MaNV, MatKhau).Item2.Trim() == "NS")
                return RedirectToAction("HumanResourceMain", "HumanResource");
            else
            {
                ViewBag.StaffLoginStatus = "Tên đăng nhập hoặc Mật khẩu KHÔNG đúng!";

                return View("Index");
            }
                
        }
        private (bool, string) ManagerCheckLogin(string MaNV, string MatKhau)
        {
            int error = 0;
            if (MaNV == "")
            {
                ModelState.AddModelError("inputUsername", "* Xin hãy điền tên đăng nhập");
                error++;
            }
            if (MatKhau == "")
            {
                ModelState.AddModelError("inputPassword", "* Xin hãy điền mật khẩu");
                error++;
            }
            if (error == 0)
            {
                string authTmp = SHA256.ToSHA256(MatKhau);
                NhanVien check = db.NhanViens.Where(
                    a => a.MaNV.Trim() == MaNV.Trim()
                    && a.MatKhau == authTmp
                ).FirstOrDefault();

                //Thấy thông tin => Thông tin đúng
                if (check != null)
                {
                    var data = db.ThongTinNDs.Where(a => a.CMND == check.CMND).FirstOrDefault();
                    string[] name = check.ThongTinND.HoTen.Split(' ');
                    Session["AccountName"] = name[name.Length - 2] + " " + name[name.Length - 1];
                    Session["Role"] = check.ChucVu.TenCV.ToString();
                    Session["RoleID"] = check.ChucVu.MaChucVu.Trim();

                    return (true, check.MaChucVu.ToString());
                }

                else
                {
                    ModelState.AddModelError("Error", "* Tài khoản hoặc mật khẩu không đúng");

                    return (false, "");
                }
            }
            //Thông tin sai
            else
            {
                return (false, "");
            }
        }
        //Đăng xuất
        public ActionResult Logout()
        {
            Session.Abandon();

            return RedirectToAction("Index", "Home");
        }
    }
}