using QLMB.Models;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;

namespace QLMB.Controllers.Customer
{
    public class ForgetPasswordController : Controller
    {
        private database db = new database();
        
        //Trang quên mật khẩu
        public ActionResult ForgetPassword()
        {
            return View();
        }


        //Xử lý lấy CMND
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ForgetPassword(string CMND)
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
                        return RedirectToAction("rePasswordNguoiThue","ForgetPassword");
                    }
                    else
                    {
                        var nv = db.NhanViens.Where(s => s.CMND == CMND).FirstOrDefault();
                        if(nv != null)
                        {
                            Session["CMND"] = nv.CMND.Trim();
                            Session["TenDangNhap"] = nv.MaNV;
                            return RedirectToAction("rePasswordNhanVien","ForgetPassword");
                        }
                        else
                        {
                            ModelState.AddModelError("forgetError", "* Không tồn tại CMND/CCCD này trong hệ thống !");
                            return View();
                        }
                    }
                }
                return View();
            }
            catch
            {
                return RedirectToAction("Index", "SkillIssue");
            }
        }



        //Check CMND người thuê
        private bool checkInfo(string CMND)
        {
            (bool, string) checkCMND = Validation.CMND(CMND);

            if (checkCMND.Item1)
                return true;

            ModelState.AddModelError("inputCMND", checkCMND.Item2);
            return false;
        }




        //----------- Người thuê -----------//
        //Cài lại mật khẩu
        public ActionResult rePasswordNguoiThue()
        {
            if (Session["TenDangNhap"] != null)
                return View();
            else
                return RedirectToAction("ForgetPassword", "ForgetPassword");
        }



        //Cập nhật mật khẩu
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult rePasswordNguoiThue(NguoiThue nguoiThue, string choice)
        {
            switch (choice)
            {
                case "Quay lại":
                    return RedirectToAction("ForgetPassword", "ForgetPassword");
                
                default:
                    try
                    {
                        if (checkRePassword(nguoiThue) == true)
                        {
                            updateDatabaseNguoiThue(nguoiThue);
                            TempData["msg"] = "<script>alert('Đổi mật khẩu thành công');</script>";

                            //Xoá session
                            Session.Remove("CMND");
                            Session.Remove("TenDangNhap");

                            return RedirectToAction("Login", "Login");
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


        //Check mật khẩu mới
        private bool checkRePassword(NguoiThue nguoiThue)
        {
            (bool, string) password = Validation.Password(nguoiThue.MatKhau);
            (bool, string) rePassword = Validation.rePassword(nguoiThue.MatKhau, nguoiThue.rePassword);

            if(password.Item1 && rePassword.Item1)
                return true;
            

            //Mật khẩu
            if (!password.Item1)
                ModelState.AddModelError("resetPassword", password.Item2);


            //Nhập lại mật khẩu
            if (!rePassword.Item1)
                ModelState.AddModelError("reResetPassword", rePassword.Item2);

            return false;
        }


        //Cập nhật dữ liệu
        private void updateDatabaseNguoiThue(NguoiThue nguoiThue)
        {
            string authTmp = SHA256.ToSHA256(nguoiThue.MatKhau);
            nguoiThue.CMND = Session["CMND"].ToString();
            nguoiThue.TenDangNhap = Session["TenDangNhap"].ToString();
            nguoiThue.MatKhau = authTmp;

            db.Entry(nguoiThue).State = EntityState.Modified;
            db.SaveChanges();
        }
    }
}