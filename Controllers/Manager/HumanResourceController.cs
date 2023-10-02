using QLMB.Models;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;

namespace QLMB.Controllers.Manager
{
    public class HumanResourceController : Controller
    {
        private database db = new database();
        public string CMNDtemp = null;
        public ActionResult Main(string NameSearch)
        {
            Session["tempInfo"] = null;
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
                        NhanVien info = db.NhanViens.Where(s => s.CMND == CMND).FirstOrDefault();
                        return View(info);
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

        [HttpPost]
        public ActionResult Detail(NhanVien info, string btn)
        {
            NhanVien update = db.NhanViens.Where(S => S.CMND == info.CMND).First();
            switch (btn)
            {
                case "Fired":
                    update.MATT = 5;
                    break;

                case "Hired":
                    update.MATT = 6;
                    update.MatKhau = SHA256.ToSHA256("123456");
                    break;

                case "ResetPassword":
                    update.MatKhau = SHA256.ToSHA256("123456");
                    break;
            }
            

            db.Entry(update).State = EntityState.Modified;

            db.SaveChanges();

            return View(update);
        }
    }
}

//Mã tình trạng
//    1: Đang chờ duyệt
//    2: Được duyệt
//    3: Bị từ chối
//    4: Đang làm
//    5: Nghỉ việc
//    6: Được tuyển