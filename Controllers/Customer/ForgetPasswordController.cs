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
                            return RedirectToAction("rePasswordNhanVien", "ForgetPassword");
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
            bool error = true;
            //CMND chưa nhập
            if (CMND == "")
            {
                ModelState.AddModelError("inputCMND", "* Xin hãy điền CMND/CCCD");
                error = false;
            }

            //CMND Chưa đủ 12 số
            else if (CMND.Trim().Length != 12)
            {
                ModelState.AddModelError("inputCMND", "* CMND/CCCD phải đủ 12 số");
                error = false;
            }
            else
            {
                //CMND có chứa chữ
                char[] numberCheck = CMND.Trim().ToCharArray();

                for (int i = 0; i < CMND.Trim().Length; i++)
                {
                    if (!char.IsNumber(numberCheck[i]))
                    {
                        error = false;
                        ModelState.AddModelError("inputCMND", "* CMND/CCCD không hợp lệ");
                        break;
                    }
                }
            }

            return error;
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


        //Check mật khẩu mới
        private bool checkRePassword(NguoiThue nguoiThue)
        {
            bool error = true;
            //Mật khẩu
            if (nguoiThue.MatKhau == null)
            {
                ModelState.AddModelError("resetPassword", "* Xin hãy điền mật khẩu");
                error = false;
            }

            //Nhập lại mật khẩu
            if (nguoiThue.rePassword == null)
            {
                ModelState.AddModelError("reReserPassword", "* Xin hãy điền lại mật khẩu");
                error = false;
            }
            else if (nguoiThue.MatKhau != nguoiThue.rePassword)
            {
                ModelState.AddModelError("reReserPassword", "* Mật khẩu không khớp - Xin hãy điền lại");
                error = false;
            }

            return error;
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