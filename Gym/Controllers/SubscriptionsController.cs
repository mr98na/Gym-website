using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Gym.Models;

namespace Gym.Controllers
{
    public class SubscriptionsController : Controller
    {
        private OurFitnessDBEntities db = new OurFitnessDBEntities();

        // GET: Subscriptions
        public ActionResult Index()
        {
            if (Session["Type"].ToString() == "admin")
            {
                var subscriptions2 = db.Subscriptions.Include(s => s.Package).Include(s => s.User);
                return View(subscriptions2.ToList());
            }
            int userid = Convert.ToInt32(Session["ID"].ToString());
            var subscriptions = db.Subscriptions.Include(s => s.Package).Include(s => s.User).Where(a => a.UserID == userid);
            return View(subscriptions.ToList());
        }

        // GET: Subscriptions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Subscription subscription = db.Subscriptions.Find(id);
            if (subscription == null)
            {
                return HttpNotFound();
            }
            return View(subscription);
        }

        // GET: Subscriptions/Create
        public ActionResult Create()
        {
            ViewBag.PackageID = new SelectList(db.Packages, "ID", "Title");
            ViewBag.UserID = new SelectList(db.Users, "ID", "Name");
            return View();
        }

        // POST: Subscriptions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(int? id, DateTime date)
        {
            Subscription subscription = new Subscription();
            int userid = Convert.ToInt32(Session["ID"].ToString());
            subscription.UserID = userid;
            subscription.PackageID = id;
            subscription.JoinDate = DateTime.Now;
            subscription.StartDate = date;
            db.Subscriptions.Add(subscription);
            db.SaveChanges();
            return RedirectToAction("Index");

        }

        // GET: Subscriptions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Subscription subscription = db.Subscriptions.Find(id);
            if (subscription == null)
            {
                return HttpNotFound();
            }
            ViewBag.PackageID = new SelectList(db.Packages, "ID", "Title", subscription.PackageID);
            ViewBag.UserID = new SelectList(db.Users, "ID", "Name", subscription.UserID);
            return View(subscription);
        }

        // POST: Subscriptions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(int? id)
        {

            Subscription subscription = db.Subscriptions.Find(id);
            subscription.IsPay = true;
            db.Entry(subscription).State = EntityState.Modified;
            db.SaveChanges();
            Session["sub"] = true;
            return RedirectToAction("Index");

        }

        // GET: Subscriptions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Subscription subscription = db.Subscriptions.Find(id);
            if (subscription == null)
            {
                return HttpNotFound();
            }
            return View(subscription);
        }

        // POST: Subscriptions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Subscription subscription = db.Subscriptions.Find(id);
            db.Subscriptions.Remove(subscription);
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
