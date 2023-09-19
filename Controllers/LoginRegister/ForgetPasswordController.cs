using QLMB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace QLMB.Controllers.LoginRegister
{
    public class ForgetPasswordController : Controller
    {
        private database db = new database();
        public ActionResult ForgetPasswordPage()
        {
            return View();
        }

        //Cài lại mật khẩu - Người thuê
        public ActionResult rePasswordNguoiThue()
        {
            if (Session["TenDangNhap"] != null)
                return View();
            else
                return RedirectToAction("ForgetPasswordPage", "ForgetPassword");
        }

        public ActionResult rePasswordNhanVien()
        {
            return View();
        }

        //Lấy CMND
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ForgetPasswordPage(string CMND)
        {

            try
            {
                //Nếu tên đăng nhập > 8 ký tự ==> Người thuê
                if (checkInfo(CMND) == true)
                {
                    var nt = db.NguoiThues.Where(s => s.CMND == CMND).FirstOrDefault();
                    if(nt != null)
                    {
                        Session["CMND"] = nt.CMND.Trim();
                        Session["TenDangNhap"] = nt.TenDangNhap;
                        return RedirectToAction("rePasswordNguoiThue", "ForgetPassword");
                    }
                    else
                    {
                        var nv = db.NhanViens.Where(s => s.CMND == CMND).FirstOrDefault();
                        if(nv != null)
                        {
                            Session["CMND"] = nv.CMND.Trim();
                            Session["TenDangNhap"] = nv.MaNV;
                            return RedirectToAction("rePasswordNhanVien", "ForgetPassword");
                        }
                        else
                        {
                            ModelState.AddModelError("forgetError", "* Không tồn tại CMND/CCCD này trong hệ thống !");
                            return View();
                        }
                    }
                }

                else
                {
                    return View();
                }
            }
            catch
            {
                ModelState.AddModelError("forgetError", "* Lỗi hệ thống - Xin vui lòng thử lại !");
                return View();
            }
        }


        //Cập nhật mật khẩu
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult rePasswordNguoiThue(NguoiThue nguoiThue, String choice)
        {
            switch (choice)
            {
                case "Quay lại":
                    return RedirectToAction("ForgetPasswordPage", "ForgetPassword");
                default:
                    try
                    {
                        if (checkRePassword(nguoiThue) == true)
                        {
                            updateDatabaseNguoiThue(nguoiThue);
                            TempData["msg"] = "<script>alert('Đổi mật khẩu thành công');</script>";
                            return RedirectToAction("LoginPage", "Login");
                        }
                        return View();
                    }
                    catch
                    {
                        ModelState.AddModelError("updateError", "* Lỗi hệ thống - Xin vui lòng thử lại !");
                        return View();
                    }
            }
           
        }
        private bool checkInfo(string CMND)
        {
            int error = 0;
            //CMND chưa nhập
            if (CMND == "")
            {
                ModelState.AddModelError("inputCMND", "* Xin hãy điền CMND/CCCD");
                error++;
            }

            //CMND Chưa đủ 12 số
            else if (CMND.Trim().Length != 12)
            {
                ModelState.AddModelError("inputCMND", "* CMND/CCCD phải đủ 12 số");
                error++;
            }
            else
            {
                //CMND có chứa chữ
                char[] numberCheck = CMND.Trim().ToCharArray();

                for (int i = 0; i < CMND.Trim().Length; i++)
                {
                    if (!char.IsNumber(numberCheck[i]))
                    {
                        error++;
                        ModelState.AddModelError("inputCMND", "* CMND/CCCD không hợp lệ");
                        break;
                    }
                }
            }

            if (error == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool checkRePassword(NguoiThue nguoiThue)
        {
            int error = 0;
            //Mật khẩu
            if (nguoiThue.MatKhau == null)
            {
                ModelState.AddModelError("resetPassword", "* Xin hãy điền mật khẩu");
                error++;
            }


            //Nhập lại mật khẩu
            if (nguoiThue.NhapLaiMatKhau == null)
            {
                ModelState.AddModelError("reReserPassword", "* Xin hãy điền lại mật khẩu");
                error++;
            }
            else if (nguoiThue.MatKhau != nguoiThue.NhapLaiMatKhau)
            {
                ModelState.AddModelError("reReserPassword", "* Mật khẩu không khớp - Xin hãy điền lại");
                error++;
            }


            if (error == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
            
        }

        private void updateDatabaseNguoiThue(NguoiThue nguoiThue)
        {
            string authTmp = SHA256.ToSHA256(nguoiThue.MatKhau);
            nguoiThue.CMND = Session["CMND"].ToString();
            nguoiThue.TenDangNhap = Session["TenDangNhap"].ToString();
            nguoiThue.MatKhau = authTmp;

            db.Entry(nguoiThue).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
        }
    }
}