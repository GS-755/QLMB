using QLMB.Models;
using System.Linq;
using System.Web.Mvc;

namespace QLMB.Controllers.Manager
{
    public class HumanResourceController : Controller
    {
        private database db = new database();

        public ActionResult HumanResourceMain(string NameSearch)
        {
            try
            {
                //Nếu RoleID != null --> Đã đăng nhập
                if (Session["RoleID"] != null)
                {
                    //Đúng Role --> Vào
                    if (Session["RoleID"].ToString() == "NS")
                    {
                        ////Dùng để xử lý về lại trang trước đó trong Detail
                        //Session["Page"] = "NS";

                        var data = db.NhanViens.ToList();

                        //Xử lý tìm kiếm
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
                }
                //Không thoả --> Về trang xử lý chuyển trang
                return RedirectToAction("Manager", "Redirect");
            }

            //Lỗi xử lý --> Skill Issue :))
            catch
            {
                return RedirectToAction("Index", "SkillIssue");
            }
        }

        public ActionResult Detail(string CMND)
        {
            try
            {
                //Nếu RoleID != null --> Đã đăng nhập
                if (Session["RoleID"] != null)
                {
                    //Đúng Role --> Vào
                    if (Session["RoleID"].ToString() == "NS")
                    {
                        return View(db.NhanViens.Where(s => s.CMND == CMND).FirstOrDefault());
                    }
                }
                //Không thoả --> Về trang xử lý chuyển trang
                return RedirectToAction("Manager", "Redirect");
            }

            //Lỗi xử lý --> Skill Issue :))
            catch
            {
                return RedirectToAction("Index", "SkillIssue");
            }

        }
    }
}