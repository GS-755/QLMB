using QLMB.Models;
using System.Web.Mvc;

namespace QLMB.Controllers.Manager
{
    public class StaffLoginController : Controller
    {
        //GET: StaffLogin/
        public ActionResult Login()
        {
            Session["Page"] = "Staff";
            return RedirectToAction("Login","Login");
        }
    }
}