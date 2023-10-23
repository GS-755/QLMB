using QLMB.Models;
using System;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Web.Mvc;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using System.Data.Entity;
using QLMB.Models.Process;

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
                    List<SuKienUuDai> data = Shared.listSKUD(db, NameSearch, "SK");

                    //Dùng để xử lý về lại trang trước đó
                    Session["Page"] = "EventMain";
                    Session["EventLocal"] = "EventMain";
                    Session.Remove("EventTemp");
                    return View(data);
                }
                //Không thoả --> Về trang xử lý chuyển trang
                return RedirectToAction("Manager", "Account");
            }

            //Lỗi xử lý --> Skill Issue :))
            catch { return RedirectToAction("Index", "SkillIssue"); }
        }

        public ActionResult SaleMain(string NameSearch)
        {
            try
            {
                //Kiểm tra hợp lệ
                if (checkRole())
                {
                    List<SuKienUuDai> data = Shared.listSKUD(db, NameSearch, "UD");
                    
                    //Dùng để xử lý về lại trang trước đó
                    Session["Page"] = "SaleMain";
                    Session["EventLocal"] = "SaleMain";
                    Session.Remove("EventTemp");
                    return View(data);
                }

                //Không thoả --> Về trang xử lý chuyển trang
                return RedirectToAction("Manager", "Account");
            }

            //Lỗi xử lý --> Skill Issue :))
            catch { return RedirectToAction("Index", "SkillIssue"); }
        }

        public ActionResult Detail(string MaDon)
        {
            try
            {
                //Kiểm tra hợp lệ
                if (checkRole() || Session["Page"] != null)
                {
                    SuKienUuDai info = new SuKienUuDai();

                    if ((MaDon == null || MaDon == "") && Session["EventTemp"] != null)
                        info = (SuKienUuDai)Session["EventTemp"];

                    else
                    {
                        info = db.SuKienUuDais.Where(s => s.MaDon == MaDon).FirstOrDefault();
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
            catch { return RedirectToAction("Index", "SkillIssue"); }
        }

        [HttpPost]
        public ActionResult Detail(SuKienUuDai info, string btn)
        {
            string MaNV = ((NhanVien)Session["EmployeeInfo"]).MaNV;
            (bool, string, SuKienUuDai) saveVerified = Edit.EventVerified(db, info.MaDon, MaNV, btn);
            if (saveVerified.Item1)
            {
                TempData["msg"] = $"<script>alert('{saveVerified.Item2}');</script>";
                return RedirectToAction("Detail", "Event", new { MaDon = info.MaDon });
            }
                
            ModelState.AddModelError("VerifiedFaield", saveVerified.Item2);
            return View(saveVerified.Item3);

            
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
        public ActionResult returnLocal()
        {
            if(Session["EventLocal"] != null)
            {
                if (Session["EventLocal"].ToString().Trim() == "SaleMain")
                    return Redirect("SaleMain");
            }
            return Redirect("EventMain");
        }
    }
}

//EntityFunctions.TruncateTime([DateTime]) để lấy DateTime vì LingQ không hỗ trợ
//info = db.SuKienUuDais.Where(s => 
//             s.MaDM.Trim() == DanhMuc.Trim() &&
//             EntityFunctions.TruncateTime(s.NgayLamDon) == EntityFunctions.TruncateTime(NgayLamDon) &&
//             s.TenDangNhap.Trim() == NguoiLamDon.Trim() &&
//             s.TieuDe.Trim() == TieuDe.Trim()).FirstOrDefault();
