using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using QLMB.Models;

namespace QLMB.Controllers.Customer.RentProperty
{
    public class PropertyRentController : Controller
    {
        private database db = new database();

        // GET: PropertyRent
        public ActionResult Index()
        {
            var donXinThues = db.DonXinThues.
                Include(d => d.NguoiThue).
                Include(d => d.HopDong).
                Include(d => d.MatBang).
                Include(d => d.NhanVien).
                Include(d => d.TinhTrang);

            return View(donXinThues.ToList());
        }

        // GET: PropertyRent/Details/5
        public ActionResult Details(string id)
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

        // GET: PropertyRent/Create
        public ActionResult Create()
        {
            ViewBag.TenDangNhap = new SelectList(db.NguoiThues, "TenDangNhap", "TenDangNhap");
            ViewBag.MaHD = new SelectList(db.HopDongs, "MaHD", "MaHD");
            ViewBag.MaMB = new SelectList(db.MatBangs, "MaMB", "MaMB");
            ViewBag.MaNV = new SelectList(db.NhanViens, "MaNV", "MaNV");
            ViewBag.MATT = new SelectList(db.TinhTrangs, "MATT", "TenTT");

            return View();
        }

        // POST: PropertyRent/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(DonXinThue donXinThue)
        {
            if (ModelState.IsValid)
            {
                db.DonXinThues.Add(donXinThue);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.TenDangNhap = new SelectList(db.NguoiThues, "TenDangNhap", "TenDangNhap", donXinThue.TenDangNhap);
            ViewBag.MaHD = new SelectList(db.HopDongs, "MaHD", "MaHD", donXinThue.MaHD);
            ViewBag.MaMB = new SelectList(db.MatBangs, "MaMB", "MaMB", donXinThue.MaMB);
            ViewBag.MaNV = new SelectList(db.NhanViens, "MaNV", "MaNV", donXinThue.MaNV);
            ViewBag.MATT = new SelectList(db.TinhTrangs, "MATT", "TenTT", donXinThue.MATT);
            return View(donXinThue);
        }

        // GET: PropertyRent/Edit/5
        public ActionResult Edit(string id)
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
            ViewBag.TenDangNhap = new SelectList(db.NguoiThues, "TenDangNhap", "TenDangNhap", donXinThue.TenDangNhap);
            ViewBag.MaHD = new SelectList(db.HopDongs, "MaHD", "MaHD", donXinThue.MaHD);
            ViewBag.MaMB = new SelectList(db.MatBangs, "MaMB", "MaMB", donXinThue.MaMB);
            ViewBag.MaNV = new SelectList(db.NhanViens, "MaNV", "MaNV", donXinThue.MaNV);
            ViewBag.MATT = new SelectList(db.TinhTrangs, "MATT", "TenTT", donXinThue.MATT);
            return View(donXinThue);
        }

        // POST: PropertyRent/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(DonXinThue donXinThue)
        {
            if (ModelState.IsValid)
            {
                db.Entry(donXinThue).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.TenDangNhap = new SelectList(db.NguoiThues, "TenDangNhap", "TenDangNhap", donXinThue.TenDangNhap);
            ViewBag.MaHD = new SelectList(db.HopDongs, "MaHD", "MaHD", donXinThue.MaHD);
            ViewBag.MaMB = new SelectList(db.MatBangs, "MaMB", "MaMB", donXinThue.MaMB);
            ViewBag.MaNV = new SelectList(db.NhanViens, "MaNV", "MaNV", donXinThue.MaNV);
            ViewBag.MATT = new SelectList(db.TinhTrangs, "MATT", "TenTT", donXinThue.MATT);
            return View(donXinThue);
        }

        // GET: PropertyRent/Delete/5
        public ActionResult Delete(string id)
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

        // POST: PropertyRent/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            DonXinThue donXinThue = db.DonXinThues.Find(id);
            db.DonXinThues.Remove(donXinThue);
            db.SaveChanges();
            return RedirectToAction("Index");
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
