using QLMB.Models;
using System.Data.Entity;
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
    }
}