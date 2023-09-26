using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QLMB.Controllers
{
    public class RedirectController : Controller
    {
        // GET: Redirect
        public ActionResult Manager()
        {
            if (Session["RoleID"] != null)
            {
                switch (Session["RoleID"].ToString())
                {
                    case "NS":
                        return RedirectToAction("HumanResourceMain", "HumanResource");
                    case "SKUD":
                        return RedirectToAction("EventMain", "Event");
                    default:
                        return RedirectToAction("Index", "Home");

                }
            }
            return RedirectToAction("Index", "Home");
        }
    }
}