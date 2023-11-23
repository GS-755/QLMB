using QLMB.Models;
using System.Linq;
using System.Web.Mvc;


namespace QLMB.Controllers
{
    public class HomeController : Controller
    {
        database db = new database();

        public ActionResult Index()
        {
            var suKienList = db.SuKienUuDais.OrderByDescending(s => s.NgayBatDau);
            
            return View(suKienList);
        }
        public ActionResult ClearSession()
        {
            Session.Abandon();

            return RedirectToAction("Index");
        }
        public ActionResult EventDetail(string id)
        {
            try
            {
                //Kiểm tra hợp lệ
                SuKienUuDai info = new SuKienUuDai();
                if (id == null || id == "")
                    return RedirectToAction("Index", "SkillIssue");
                else
                {
                    info = db.SuKienUuDais.Where(s => s.MaDon == id).FirstOrDefault();
                }

                return View(info);
            }

            //Lỗi xử lý --> Skill Issue :))
            catch { return RedirectToAction("Index", "SkillIssue"); }
        }
    }
}