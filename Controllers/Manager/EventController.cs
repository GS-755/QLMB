using QLMB.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using System.Web.WebPages;

namespace QLMB.Controllers.Manager
{
    public class EventController : Controller
    {
        database db = new database();
        // GET: Event
        public ActionResult EventMain(string NameSearch)
        {
            Session["Page"] = "SK";
            var data = db.SuKienUuDais.Where(s => s.MaDM.Trim() == "SK").ToList();
            if (NameSearch != null)
            {
                data = data.Where(s => s.NgayLamDon.ToString().Contains(NameSearch) ||
                                       s.NgayBatDau.ToString().Contains(NameSearch) ||
                                       s.NgayKetThuc.ToString().Contains(NameSearch) ||
                                       s.NguoiThue.ThongTinND.HoTen.ToUpper().Contains(NameSearch.ToUpper()) ||
                                       s.TieuDe.ToUpper().Contains(NameSearch.ToUpper())||
                                       s.TinhTrang.TenTT.ToUpper().Contains(NameSearch.ToUpper())).ToList();
            }
            return View(data);
        }

        public ActionResult SaleMain(string NameSearch)
        {
            Session["Page"] = "UD";
            var data = db.SuKienUuDais.Where(s => s.MaDM.Trim() == "UD").ToList();
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

        public ActionResult Detail(DateTime NgayLamDon, string NguoiLamDon, string tieuDe)
        {
            if(Session["Page"] == null)
            {
                return RedirectToAction("EventMain");
            }
            else
            {
                //EntityFunctions.TruncateTime([DateTime]) để lấy DateTime vì LingQ không hỗ trợ 
                return View(db.SuKienUuDais.Where(s => EntityFunctions.TruncateTime(s.NgayLamDon) == EntityFunctions.TruncateTime(NgayLamDon) && 
                                                  s.NguoiThue.ThongTinND.HoTen == NguoiLamDon && 
                                                  s.TieuDe == tieuDe).FirstOrDefault());
            }
           
        }
    }
}