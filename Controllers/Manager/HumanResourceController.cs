using QLMB.Models;
using System;
using System.Linq;
using System.Web.Mvc;

namespace QLMB.Controllers.Manager
{
    public class HumanResourceController : Controller
    {
        database db = new database();
        public ActionResult Index()
        {
            try
            {
                return Session["RoleID"].ToString() == "NS"
                        ? RedirectToAction("HumanResourceMain", "HumanResource")
                        : Session["RoleID"].ToString() == "SKUD"
                            ? RedirectToAction("EventMain", "Event")
                            : RedirectToAction("LoginPage", "Login");
            } catch 
            {
                return RedirectToAction("Index", "SkillIssue");
            }
        }
        // GET: HumanResource
        public ActionResult HumanResourceMain(string NameSearch)
        {
            Session["Page"] = "NS";
            var data = db.NhanViens.ToList();
            if (NameSearch != null)
            {
                data = data.Where(s => s.MaNV.ToString().Contains(NameSearch) ||
                                       s.ChucVu.TenCV.ToUpper().Contains(NameSearch) ||
                                       s.CMND.Trim().Contains(NameSearch) ||
                                       s.ThongTinND.HoTen.ToUpper().Contains(NameSearch.ToUpper()) ||
                                       s.TinhTrang.TenTT.ToUpper().Contains(NameSearch.ToUpper())).ToList();
            }
            return View(data);
        }

        public ActionResult Detail(string CMND)
        {
            if (Session["Page"] == null)
            {
                return RedirectToAction("HumanResourceMain");
            }
            else
            {
                return View(db.NhanViens.Where(s => s.CMND == CMND).FirstOrDefault());
            }
        }
    }
}