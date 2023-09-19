using QLMB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QLMB.Controllers
{
    public class LoginController : Controller
    {
        private database db = new database();

        //Đăng nhập
        public ActionResult LoginPage()
        {
            return View();
        }


        //Đăng xuất
        public ActionResult Logout()
        {
            Session.Abandon();

            return RedirectToAction("Index", "Home");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string TenDangNhap, string MatKhau)
        {
            
            try
            {
                //Nếu tên đăng nhập > 8 ký tự ==> Người thuê
                if(rentalCheckLogin(TenDangNhap,MatKhau) == true)
                {
                    return RedirectToAction("Index", "Home");
                }

                //Còn lại ==> Nhân viên
                else if(managerCheckLogin(TenDangNhap,MatKhau).Item1)
                {
                    if (managerCheckLogin(TenDangNhap, MatKhau).Item2.Trim() == "SKUD")
                    {
                        return RedirectToAction("EventMain", "Event");
                    }
                    if (managerCheckLogin(TenDangNhap, MatKhau).Item2.Trim() == "NS")
                    {
                        return RedirectToAction("HumanResourceMain", "HumanResource");
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }

                }

                else
                {
                    Session["AccountName"] = null;
                    return View("LoginPage");
                }
            }
            catch
            {
                Session["renterStatus"] = null;
                return View("LoginPage");
            }
        }

        //Kiểm tra thông tin đăng nhập
        private bool rentalCheckLogin(string TenDangNhap, string MatKhau)
        {
            int error = 0;
            if(TenDangNhap == "")
            {
                ModelState.AddModelError("inputUsername", "* Xin hãy điền tên đăng nhập");
                error++;
            }

            if (MatKhau == "")
            {
                ModelState.AddModelError("inputPassword", "* Xin hãy điền mật khẩu");
                error++;
            }

            if(error == 0)
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


        private (bool,string) managerCheckLogin(string TenDangNhap, string MatKhau)
        {
            int error = 0;
            if (TenDangNhap == "")
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
                var check = db.NhanViens.Where(a => a.MaNV.Trim() == TenDangNhap.Trim() && a.MatKhau == authTmp).FirstOrDefault();

                //Thấy thông tin => Thông tin đúng
                if (check != null)
                {
                    var data = db.ThongTinNDs.Where(a => a.CMND == check.CMND).FirstOrDefault();
                    string[] name = check.ThongTinND.HoTen.Split(' ');
                    Session["AccountName"] = name[name.Length - 2] + " " + name[name.Length - 1];
                    Session["Role"] = check.ChucVu.TenCV.ToString();
                    Session["RoleID"] = check.ChucVu.MaChucVu.Trim();
                    return (true,check.MaChucVu.ToString());
                }

                else
                {
                    ModelState.AddModelError("Error", "* Tài khoản hoặc mật khẩu không đúng");
                    return (false,"");
                }
            }
            //Thông tin sai
            else
            {
                return (false,"");
            }
        }
    }
}