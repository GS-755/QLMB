using QLMB.Models;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;
using QLMB.Models.Process;

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
                    (bool,string,NguoiThue) customer = Validation.ExistAccountCustomer(db, CMND);
                    (bool, string, NhanVien) employee = Validation.ExistAccountEmployee(db, CMND);

                    if (customer.Item1)
                    {
                        Session["CMND"] = customer.Item3.CMND.Trim();
                        Session["TenDangNhap"] = customer.Item3.TenDangNhap;
                        return RedirectToAction("rePasswordNguoiThue","ForgetPassword");
                    }

                    //Chưa làm
                    //if (employee.Item1)
                    //{
                    //    Session["CMND"] = employee.Item3.CMND.Trim();
                    //    Session["TenDangNhap"] = employee.Item3.MaNV.Trim();
                    //    return RedirectToAction("rePasswordNhanVien", "ForgetPassword");
                    //}
                    else 
                        ModelState.AddModelError("forgetError", customer.Item2);
                }
                return View();
            }
            catch { return RedirectToAction("Index", "SkillIssue"); }
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
        public ActionResult rePasswordNguoiThue(NguoiThue nguoiThue, string choice, string rePass)
        {
            switch (choice)
            {
                case "Quay lại":
                    return RedirectToAction("ForgetPassword", "ForgetPassword");
                
                default:
                    if (checkRePassword(nguoiThue, rePass) == true)
                    {
                        nguoiThue.CMND = Session["CMND"].ToString();
                        nguoiThue.TenDangNhap = Session["TenDangNhap"].ToString();
                        (bool, string) changePassword = Edit.CustomerPassword(db, nguoiThue);

                        if (changePassword.Item1)
                        {
                            TempData["msg"] = $"<script>alert('{changePassword.Item2}');</script>";

                            //Xoá session
                            Session.Remove("CMND");
                            Session.Remove("TenDangNhap");
                            return RedirectToAction("Login", "Login");
                        }
                        ModelState.AddModelError("updateError", "* Lỗi hệ thống - Xin vui lòng thử lại !");
                    }
                    return View();
            }
        }


        //Check mật khẩu mới
        private bool checkRePassword(NguoiThue nguoiThue, string rePass)
        {
            (bool, string) password = Validation.Password(nguoiThue.MatKhau);
            (bool, string) rePassword = Validation.rePassword(nguoiThue.MatKhau, rePass);

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
    }
}