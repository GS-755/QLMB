using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using QLMB.Models;

namespace QLMB.Controllers.Test
{
    public class PropertyController : Controller
    {
        // Database & Constant configurations
        private database db = new database();
        private readonly string ROLE = "MB";

        /*
            Boolean condition(s)
            1.  Users that have logged in
            2.  Users with a valid role
        */
        public bool IsLoggedIn() => Session["RoleID"] != null;
        public bool IsValidRole() => Session["RoleID"].ToString() == ROLE;

        // GET: Property
        public ActionResult Index()
        {
            try
            {
                if (IsLoggedIn() && IsValidRole())
                {
                    IQueryable<MatBang> matBangs = db.MatBangs.Include(m => m.TinhTrang);

                    return View(matBangs.ToList());
                }

                return RedirectToAction("Login", "Login");
            }
            catch 
            {
                return RedirectToAction("Index", "SkillIssue");
            }
        }
        public ActionResult Create()
        {
            try
            {
                if (IsLoggedIn() && IsValidRole())
                {
                    List<TinhTrang> dsTinhTrang = db.TinhTrangs.Where(k => k.MATT >= 7).ToList();
                    ViewBag.MATT = new SelectList(dsTinhTrang, "MaTT", "TenTT");

                    return View();
                }

                return RedirectToAction("Login", "Login");
            }
            catch
            {
                return RedirectToAction("Index", "SkillIssue");
            }
        }
        public ActionResult Details(string id)
        {
            try
            {
                if (IsLoggedIn() && IsValidRole())
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

                return RedirectToAction("Login", "Login");
            }
            catch
            {
                return RedirectToAction("Index", "SkillIssue");
            }
        }
        public ActionResult Edit(string id)
        {
            try
            {
                if (IsLoggedIn() && IsValidRole())
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

                return RedirectToAction("Login", "Login");
            }
            catch
            {
                return RedirectToAction("Index", "SkillIssue");
            }
        }
        public ActionResult Delete(string id)
        {
            try
            {
                if (IsLoggedIn() && IsValidRole())
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

                return RedirectToAction("Login", "Login");
            }
            catch
            {
                return RedirectToAction("Index", "SkillIssue");
            }
        }

        // POST: Property
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(MatBang matBang)
        {
            try
            {
                if (matBang.UploadImage != null)
                {
                    string fileName = Path.GetFileNameWithoutExtension(matBang.UploadImage.FileName);
                    string extension = Path.GetExtension(matBang.UploadImage.FileName);
                    fileName += extension;
                    matBang.HinhMB = MatBang.SERVER_IMG_PATH + fileName;
                    matBang.UploadImage.SaveAs(Path.Combine(Server.MapPath(MatBang.SERVER_IMG_PATH), fileName));
                }
                db.MatBangs.Add(matBang);
                db.SaveChanges();
                ViewBag.MATT = new SelectList(db.TinhTrangs, "MATT", "TenTT", matBang.MATT);

                return RedirectToAction("Index");
            }
            catch
            {
                return RedirectToAction("Index", "SkillIssue");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(MatBang matBang)
        {
            try
            {
                if (matBang.UploadImage != null)
                {
                    string fileName = Path.GetFileNameWithoutExtension(matBang.UploadImage.FileName);
                    string extension = Path.GetExtension(matBang.UploadImage.FileName);
                    fileName += extension;
                    matBang.HinhMB = MatBang.SERVER_IMG_PATH + fileName;
                    matBang.UploadImage.SaveAs(Path.Combine(Server.MapPath(MatBang.SERVER_IMG_PATH), fileName));
                }
                db.Entry(matBang).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("Index");
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

        // Dispose
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
