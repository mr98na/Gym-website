using Gym.Models;
using System;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Gym.Controllers
{
    public class UsersController : Controller
    {
        private OurFitnessDBEntities db = new OurFitnessDBEntities();

        // GET: Users
        public ActionResult Index()
        {
            return View(db.Users.ToList());
        }

        public JsonResult GetTrainer()
        {
            var users = db.Users.Where(a => a.Type == "trainer")
                .Select(y => new { name = y.Name, id = y.ID });
            return Json(users, JsonRequestBehavior.AllowGet);
            //return Json(new { users = users }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult MyProfile()
        {
            User user = db.Users.Find(Convert.ToInt32(Session["ID"]));
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MyProfile([Bind(Include = "ID,Name,Email,Mobile,Password,Type")] User user, string Certificates, string Experiences)
        {
            if (ModelState.IsValid)
            {
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                Session["success"] = "Update Successfully";
                return RedirectToAction("MyProfile");
            }
            return View();
        }



        public ActionResult Logout()
        {
            Session["ID"] = null;
            Session["Type"] = null;
            Session["Name"] = null;
            return RedirectToAction("index", "Home");
        }
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login([Bind(Include = "Email,Password")] Login login)
        {
            if (ModelState.IsValid)
            {
                var user = db.Users.Where(a => a.Email == login.Email).FirstOrDefault();
                if (user == null)
                {
                    Session["error"] = "There is no account for this mail";
                }
                else
                {

                    if (user.Password == login.Password)
                    {
                        Session["ID"] = user.ID;
                        Session["Type"] = user.Type;
                        Session["Mobile"] = user.Mobile;
                        Session["Email"] = user.Email;
                        Session["Name"] = user.Name;
                        Session["success"] = "Login Successfully - Welcome:" + user.Name;
                     
                        if (user.Type == "user")
                        {
                            Session["sub"] = false;
                            var subscriptions = db.Subscriptions.Include(s => s.Package).Include(s => s.User).Where(a => a.UserID == user.ID
                            && EntityFunctions.AddMonths(a.StartDate, a.Package.Duration) > DateTime.Now && a.IsPay);
                            if (subscriptions.Count() > 0) {
                                Session["sub"] = true;
                            }
                        }
                        return RedirectToAction("index", "Home");
                    }
                    else
                    {
                        Session["error"] = "The password is incorrect please try again";
                    }
                }
                return View();
            }
            return View();
        }

        public ActionResult Signup()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Signup(Signup user, HttpPostedFileBase Logo, string Details, string WebSite)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    User us = new User();
                    us.Name = user.Name;
                    us.Email = user.Email;
                    us.Mobile = user.Mobile;
                    us.Password = user.Password;
                    us.Type = "user";

                    db.Users.Add(us);
                    db.SaveChanges();

                    Session["success"] = "Please login to continue";

                }
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                //if (ex.Message.Contains("IX_Users"))
                //{
                Session["error"] = "This email is used";
                //}
                return View(user);
            }

        }

        // GET: Users/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // GET: Users/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Name,Email,Mobile,Password,Type")] User user, string Certificates, string Experiences)
        {
            if (ModelState.IsValid)
            {
                db.Users.Add(user);
                db.SaveChanges();

                if (user.Type == "trainer")
                {
                    Trainer st = new Trainer();
                    st.UserID = user.ID;
                    st.Certificates = Certificates;
                    st.Experiences = Experiences;
                    db.Trainers.Add(st);
                    db.SaveChanges();
                }


                return RedirectToAction("Index");
            }

            return View(user);
        }

        // GET: Users/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Name,Email,Mobile,Password,Type")] User user)
        {
            if (ModelState.IsValid)
            {
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(user);
        }

        // GET: Users/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            User user = db.Users.Find(id);
            db.Users.Remove(user);
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
