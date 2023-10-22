using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Gym.Models;

namespace Gym.Controllers
{
    public class PackagesController : Controller
    {
        private OurFitnessDBEntities db = new OurFitnessDBEntities();

        // GET: Packages
        public ActionResult Index()
        {
            return View(db.Packages.ToList());
        }

        // GET: Packages/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Package package = db.Packages.Find(id);
            if (package == null)
            {
                return HttpNotFound();
            }
            return View(package);
        }

        // GET: Packages/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Packages/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Title,Description,Photo,Duration,Price")] Package package, HttpPostedFileBase Photo)
        {
            if (ModelState.IsValid)
            {
                if (Photo != null && Photo.ContentLength > 0)
                {
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".gif", ".png" };
                    var checkextension = Path.GetExtension(Photo.FileName).ToLower();
                    foreach (var itm in allowedExtensions)
                    {
                        if (itm.Contains(checkextension))
                        {
                            Random _r = new Random();
                            string NewName = _r.Next().ToString();
                            string Pathatt = "/Images/" + NewName + checkextension;
                            Photo.SaveAs(Server.MapPath(Pathatt));
                            package.Photo = Pathatt;
                            break;
                        }
                    }

                }
                db.Packages.Add(package);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(package);
        }

        // GET: Packages/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Package package = db.Packages.Find(id);
            if (package == null)
            {
                return HttpNotFound();
            }
            return View(package);
        }

        // POST: Packages/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Title,Description,Photo,Duration,Price")] Package package, HttpPostedFileBase Photo)
        {
            if (ModelState.IsValid)
            {
                if (Photo != null && Photo.ContentLength > 0)
                {
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".gif", ".png" };
                    var checkextension = Path.GetExtension(Photo.FileName).ToLower();
                    foreach (var itm in allowedExtensions)
                    {
                        if (itm.Contains(checkextension))
                        {
                            Random _r = new Random();
                            string NewName = _r.Next().ToString();
                            string Pathatt = "/Images/" + NewName + checkextension;
                            Photo.SaveAs(Server.MapPath(Pathatt));
                            package.Photo = Pathatt;
                            break;
                        }
                    }

                }
                db.Entry(package).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(package);
        }

        // GET: Packages/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Package package = db.Packages.Find(id);
            if (package == null)
            {
                return HttpNotFound();
            }
            return View(package);
        }

        // POST: Packages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Package package = db.Packages.Find(id);
            db.Packages.Remove(package);
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
