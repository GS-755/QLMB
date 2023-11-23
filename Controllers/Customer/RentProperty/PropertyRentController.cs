using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using QLMB.Models;

namespace QLMB.Controllers
{
    public class PropertyRentController : Controller
    {
        private database db = new database();
        private readonly string ROLE = "MB";

        /*
            Boolean condition(s)
            1.  Users that have logged in
            2.  Users with a valid role
        */
        public bool IsValidRole()
        {
            if (Session["EmployeeInfo"] == null)
            {
                return false;
            }
            //Đúng Role --> Vào
            if (((NhanVien)Session["EmployeeInfo"]).MaChucVu.Trim() == ROLE)
                return true;

            return false;
        }
        // GET: PropertyRent
        public ActionResult Index(string keyword)
        {
            if (IsValidRole())
            {
                if (keyword != null)
                {
                    var donXinThues = db.DonXinThues.
                        Include(d => d.NguoiThue).
                        Include(d => d.HopDong).
                        Include(d => d.MatBang).
                        Include(d => d.NhanVien).
                        Include(d => d.TinhTrang);

                    return View(donXinThues.Where(k => k.MaHD.Contains(keyword)).ToList());
                }
                else
                {
                    var donXinThues = db.DonXinThues.
                        Include(d => d.NguoiThue).
                        Include(d => d.HopDong).
                        Include(d => d.MatBang).
                        Include(d => d.NhanVien).
                        Include(d => d.TinhTrang);

                    return View(donXinThues.ToList());
                }
            }

            return RedirectToAction("Login", "Login");

        }
        // GET: PropertyRent/Details/5
        public ActionResult Details(string id)
        {
            if (IsValidRole())
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                DonXinThue donXinThue = db.DonXinThues.Find(id);
                if (donXinThue == null)
                {
                    return HttpNotFound();
                }
                return View(donXinThue);
            }

            return RedirectToAction("Login", "Login");
        }
        public ActionResult Qualify(string id)
        {
            if (IsValidRole())
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                DonXinThue donXinThue = db.DonXinThues.Find(id);
                if (donXinThue == null)
                {
                    ViewBag.ServerError = "Lỗi tham số!";
                }

                donXinThue.MATT = 2;
                db.Entry(donXinThue).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("Index", "PropertyRent");
            }

            return RedirectToAction("Login", "Login");
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
