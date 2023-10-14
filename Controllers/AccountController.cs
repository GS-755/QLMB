using QLMB.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static System.Data.Entity.Infrastructure.Design.Executor;

namespace QLMB.Controllers
{
    public class AccountController : Controller
    {
        private database db = new database();
        public ActionResult Manager()
        {
            if (Session["RoleID"] != null)
            {
                switch (Session["RoleID"].ToString())
                {
                    case "NS":
                        return RedirectToAction("Main", "HumanResource");
                    case "SKUD":
                        return RedirectToAction("EventMain", "Event");
                    case "MB":
                        return RedirectToAction("Index", "Property");
                    default:
                        return RedirectToAction("Index", "Home");
                }
            }
            return RedirectToAction("Index","Home");
        }


        public ActionResult FirstLogin()
        {
            switch (Session["MANV"])
            {
                case null: return Manager();
                default: return View();
            }
        }


        [HttpPost]
        public ActionResult FirstLogin(NhanVien info)
        {
            switch (checkRePassword(info))
            {
                case true:

                    string infoNV = Session["MANV"].ToString().Trim();
                    NhanVien update = db.NhanViens.Where(s => s.MaNV == infoNV).FirstOrDefault();

                    update.MATT = 4;
                    update.MatKhau = SHA256.ToSHA256(info.MatKhau);
                    
                    db.Entry(update).State = EntityState.Modified;
                    db.SaveChanges();
                    
                    Session.Remove("MANV");
                    return Manager();

                default: return View();
            }
        }

        private bool checkRePassword(NhanVien info)
        {
            int error = 0;
            //Mật khẩu
            if (info.MatKhau == null)
            {
                ModelState.AddModelError("employeePass", "* Xin hãy điền mật khẩu");
                error++;
            }

            //Nhập lại mật khẩu
            if (info.rePassword == null)
            {
                ModelState.AddModelError("reEmployeePass", "* Xin hãy điền lại mật khẩu");
                error++;
            }
            else if (info.MatKhau != info.rePassword)
            {
                ModelState.AddModelError("reEmployeePass", "* Mật khẩu không khớp - Xin hãy điền lại");
                error++;
            }

            switch (error)
            {
                case 0: return true;
                default: return false;
            }
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