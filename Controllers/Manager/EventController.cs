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
        private database db = new database();

        // GET: Event
        public ActionResult EventMain(string NameSearch)
        {
            try
            {
                //Nếu RoleID != null --> Đã đăng nhập
                if (Session["RoleID"] != null)
                {
                    //Đúng Role --> Vào
                    if(Session["RoleID"].ToString() == "SKUD")
                    {
                        //Dùng để xử lý về lại trang trước đó trong Detail
                        Session["Page"] = "SK";

                        List<SuKienUuDai> data = db.SuKienUuDais.Where(s => s.MaDM.Trim() == "SK").ToList();

                        //Xử lý tìm kiếm
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
                }
                //Không thoả --> Về trang xử lý chuyển trang
                return RedirectToAction("PageRedirect", "PageChange");
            }

            //Lỗi xử lý --> Skill Issue :))
            catch 
            {
                return RedirectToAction("Index", "SkillIssue");
            }
        }

        public ActionResult SaleMain(string NameSearch)
        {
            try
            {
                //Nếu RoleID != null --> Đã đăng nhập
                if (Session["RoleID"] != null)
                {
                    //Đúng Role --> Vào
                    if (Session["RoleID"].ToString() == "SKUD")
                    {
                        //Dùng để xử lý về lại trang trước đó trong Detail
                        Session["Page"] = "UD";

                        List<SuKienUuDai> data = db.SuKienUuDais.
                                           Where(s => s.MaDM.Trim() == "UD").ToList();
                        
                        //Xử lý tìm kiếm
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
                }
                //Không thoả --> Về trang xử lý chuyển trang
                return RedirectToAction("PageRedirect", "PageChange");
            }

            //Lỗi xử lý --> Skill Issue :))
            catch
            {
                return RedirectToAction("Index", "SkillIssue");
            }
            
           
        }

        public ActionResult Detail(DateTime NgayLamDon, string NguoiLamDon, string tieuDe)
        {
            try
            {
                //Nếu RoleID != null --> Đã đăng nhập
                if (Session["RoleID"] != null)
                {
                    //Đúng Role --> Vào
                    if (Session["RoleID"].ToString() == "SKUD")
                    {
                        if (Session["Page"] == null)
                            return RedirectToAction("EventMain");


                        //EntityFunctions.TruncateTime([DateTime]) để lấy DateTime vì LingQ không hỗ trợ
                        SuKienUuDai info = db.SuKienUuDais.Where(
                                     s => EntityFunctions.TruncateTime(s.NgayLamDon) == EntityFunctions.TruncateTime(NgayLamDon)
                                  && s.NguoiThue.ThongTinND.HoTen == NguoiLamDon
                                  && s.TieuDe == tieuDe
                                  ).FirstOrDefault();

                        return View(info);
                    }
                }

                //Không thoả --> Về trang xử lý chuyển trang
                return RedirectToAction("PageRedirect", "PageChange");
            }

            //Lỗi xử lý --> Skill Issue :))
            catch
            {
                return RedirectToAction("Index", "SkillIssue");
            }
        }
    }
}