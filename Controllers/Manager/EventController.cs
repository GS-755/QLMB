using QLMB.Models;
using System;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Web.Mvc;
using System.Collections.Generic;
using System.Web.UI.WebControls;

namespace QLMB.Controllers.Manager
{
    public class EventController : Controller
    {
        database db = new database();
        // GET: Event
        public ActionResult EventMain(string NameSearch)
        {
            try
            {
                if (Session["RoleID"].ToString() == "SKUD")
                {
                    Session["Page"] = "SK";
                    List<SuKienUuDai> data = db.SuKienUuDais.Where(s => s.MaDM.Trim() == "SK").ToList();
                    if (NameSearch != null)
                    {
                        data = data.Where(s => s.NgayLamDon.ToString().Contains(NameSearch) ||
                                               s.NgayBatDau.ToString().Contains(NameSearch) ||
                                               s.NgayKetThuc.ToString().Contains(NameSearch) ||
                                               s.NguoiThue.ThongTinND.HoTen.ToUpper().Contains(NameSearch.ToUpper()) ||
                                               s.TieuDe.ToUpper().Contains(NameSearch.ToUpper()) ||
                                               s.TinhTrang.TenTT.ToUpper().Contains(NameSearch.ToUpper())).ToList();
                    }

                    return View(data);
                }

                return RedirectToAction("LoginPage", "Login");
            }
            catch 
            {
                return RedirectToAction("Index", "SkillIssue");
            }
            

        }

        public ActionResult SaleMain(string NameSearch)
        {
            try
            {
                if (Session["RoleID"].ToString() == "SKUD")
                {
                    Session["Page"] = "UD";
                    List<SuKienUuDai> data = db.SuKienUuDais.
                                       Where(s => s.MaDM.Trim() == "UD").ToList();
                    if (NameSearch != null)
                    {
                        data = data.Where(s => s.NgayLamDon.ToString().Contains(NameSearch) ||
                                               s.NgayBatDau.ToString().Contains(NameSearch) ||
                                               s.NgayKetThuc.ToString().Contains(NameSearch) ||
                                               s.NguoiThue.ThongTinND.HoTen.ToUpper().Contains(NameSearch.ToUpper()) ||
                                               s.TieuDe.ToUpper().Contains(NameSearch.ToUpper()) ||
                                               s.TinhTrang.TenTT.ToUpper().Contains(NameSearch.ToUpper())).ToList();
                    }

                    return View(data);
                }

                return RedirectToAction("LoginPage", "Login");
            }
            catch
            {
                return RedirectToAction("Index", "SkillIssue");
            }
            
           
        }

        public ActionResult Detail(DateTime NgayLamDon, string NguoiLamDon, string tieuDe)
        {
            try
            {
                if (Session["RoleID"].ToString() == "SKUD")
                {
                    if (Session["Page"] == null)
                        return RedirectToAction("EventMain");


                    //EntityFunctions.TruncateTime([DateTime]) để lấy DateTime vì LingQ không hỗ trợ
                    SuKienUuDai skud = db.SuKienUuDais.Where(
                            s => EntityFunctions.TruncateTime(s.NgayLamDon) == EntityFunctions.TruncateTime(NgayLamDon)
                              && s.NguoiThue.ThongTinND.HoTen == NguoiLamDon
                              && s.TieuDe == tieuDe
                    ).FirstOrDefault();

                    return View(skud);
                }

                return RedirectToAction("LoginPage", "Login");
            }
            catch 
            {
                return RedirectToAction("Index", "SkillIssue");
            }
        }
    }
}