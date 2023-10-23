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
                //Kiểm tra hợp lệ
                if (checkRole())
                {
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
                    //Dùng để xử lý về lại trang trước đó
                    Session["Page"] = "EventMain";
                    return View(data);
                }

                //Không thoả --> Về trang xử lý chuyển trang
                return RedirectToAction("Manager", "Account");
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
                //Kiểm tra hợp lệ
                if (checkRole())
                {
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
                    //Dùng để xử lý về lại trang trước đó
                    Session["Page"] = "SaleMain";
                    return View(data);
                }

                //Không thoả --> Về trang xử lý chuyển trang
                return RedirectToAction("Manager", "Account");
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
                //Kiểm tra hợp lệ
                if (checkRole())
                {
                    if (Session["Page"] == null)
                        return RedirectToAction("EventMain");

                    SuKienUuDai info;

                    if (NgayLamDon == null && NgayLamDon == null && tieuDe == null && Session["EventTemp"] != null)
                    {
                        info = (SuKienUuDai)Session["EventTemp"];
                    }
                    else
                    {
                        //EntityFunctions.TruncateTime([DateTime]) để lấy DateTime vì LingQ không hỗ trợ
                        info = db.SuKienUuDais.Where(
                                     s => EntityFunctions.TruncateTime(s.NgayLamDon) == EntityFunctions.TruncateTime(NgayLamDon)
                                  && s.NguoiThue.ThongTinND.HoTen == NguoiLamDon
                                  && s.TieuDe == tieuDe
                                  ).FirstOrDefault();

                        Session["EventTemp"] = info;
                    }
                    //Dùng để xử lý về lại trang trước đó
                    Session["Page"] = "EventDetail";
                    return View(info);
                }

                //Không thoả --> Về trang xử lý chuyển trang
                return RedirectToAction("Manager", "Account");
            }

            //Lỗi xử lý --> Skill Issue :))
            catch
            {
                return RedirectToAction("Index", "SkillIssue");
            }
        }

        //Kiểm tra hợp lệ
        private bool checkRole()
        {
            //Nếu EmployeeInfo == null --> Chưa đăng nhập
            if (Session["EmployeeInfo"] == null)
                return false;

            //Đúng Role --> Vào
            if (((NhanVien)Session["EmployeeInfo"]).MaChucVu.Trim() == "SKUD")
                return true;

            return false;
        }
    }
}