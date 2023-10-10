using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using QLMB.Models;

namespace QLMB.Controllers.Test
{
    public class PropertyController : Controller
    {
        private database db = new database();

        // GET: Property
        public ActionResult Index()
        {
            try
            {
                var matBangs = db.MatBangs.Include(m => m.TinhTrang);

                return View(matBangs.ToList());
            }
            catch 
            {
                return RedirectToAction("Index", "SkillIssue");
            }
        }
        // GET: Property/Details/5
        public ActionResult Details(string id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                MatBang matBang = db.MatBangs.Find(id);
                if (matBang == null)
                {
                    return HttpNotFound();
                }
                return View(matBang);
            }
            catch
            {
                return RedirectToAction("Index", "SkillIssue");
            }
        }
        // GET: Property/Create
        public ActionResult Create()
        {
            try
            {
                List<TinhTrang> dsTinhTrang = db.TinhTrangs.Where(k => k.MATT >= 7).ToList();
                ViewBag.MATT = new SelectList(dsTinhTrang, "MaTT", "TenTT");

                return View();
            }
            catch
            {
                return RedirectToAction("Index", "SkillIssue");
            }
        }
        // POST: Property/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MaMB,Lau,DienTich,Khu,TienThue,MATT")] MatBang matBang)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.MatBangs.Add(matBang);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                ViewBag.MATT = new SelectList(db.TinhTrangs, "MATT", "TenTT", matBang.MATT);

                return View(matBang);
            }
            catch
            {
                return RedirectToAction("Index", "SkillIssue");
            }
        }
        // GET: Property/Edit/5
        public ActionResult Edit(string id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                MatBang matBang = db.MatBangs.Find(id);
                if (matBang == null)
                {
                    return HttpNotFound();
                }
                List<TinhTrang> dsTinhTrang = db.TinhTrangs.Where(k => k.MATT >= 7).ToList();
                ViewBag.MATT = new SelectList(dsTinhTrang, "MaTT", "TenTT", matBang.MATT);

                return View(matBang);
            }
            catch
            {
                return RedirectToAction("Index", "SkillIssue");
            }
        }
        // POST: Property/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MaMB,Lau,DienTich,Khu,TienThue,MATT")] MatBang matBang)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Entry(matBang).State = EntityState.Modified;
                    db.SaveChanges();

                    return RedirectToAction("Index");
                }
                List<TinhTrang> dsTinhTrang = db.TinhTrangs.Where(k => k.MATT >= 7).ToList();
                ViewBag.MATT = new SelectList(dsTinhTrang, "MaTT", "TenTT", matBang.MATT);

                return View(matBang);
            }
            catch
            {
                return RedirectToAction("Index", "SkillIssue");
            }
        }
        // GET: Property/Delete/5
        public ActionResult Delete(string id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                MatBang matBang = db.MatBangs.Find(id);
                if (matBang == null)
                {
                    return HttpNotFound();
                }

                return View(matBang);
            }
            catch
            {
                return RedirectToAction("Index", "SkillIssue");
            }
        }
        // POST: Property/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            try
            {
                MatBang matBang = db.MatBangs.Find(id);
                db.MatBangs.Remove(matBang);
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            catch
            {
                return RedirectToAction("Index", "SkillIssue");
            }
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
