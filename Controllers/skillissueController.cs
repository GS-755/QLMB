using System.Web.Mvc;

namespace QLMB.Controllers
{
    public class SkillIssueController : Controller
    {
        // GET: SkillIssue
        public ActionResult Index() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(string height)
        {
            ViewBag.DisplayHeight = height;

            return View();
        }
    }
}