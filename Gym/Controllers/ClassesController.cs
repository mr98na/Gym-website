using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using Gym.Models;

namespace Gym.Controllers
{
    public class ClassesController : Controller
    {
        private OurFitnessDBEntities db = new OurFitnessDBEntities();

        // GET: Classes
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Details()
        {
            return View();
        }
        public ActionResult MyClass()
        {
            int userid = Convert.ToInt32(Session["ID"].ToString());
            return View(db.ClassesUsers.Where(a=>a.UserID== userid).ToList());
        }

        public ActionResult TrainerClass()
        {
            int userid = Convert.ToInt32(Session["ID"].ToString());       
            return View(db.Classes.Include(a=>a.ClassesUsers).Where(a => a.TrainerID == userid).ToList());
        }

        public JsonResult GetClasses()
        {
            var classes = db.Classes
                .Select(y => new { text = y.Text, id = y.ID, start = y.Start, end = y.End, seats = y.Seats, trainerId = y.TrainerID.ToString(), trainer = y.User.Name, barColor = y.BarColor });
            return Json(classes, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Post([Bind(Include = "TrainerID,Text,BarColor,Seats,Start,End")] Class @class)
        {
            db.Classes.Add(@class);
            db.SaveChanges();
            return Json(new { success = "ok" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeleteClass(int id)
        {
            Class @class = db.Classes.Include(a=>a.User).Where(a=> a.ID== id).SingleOrDefault();

            foreach (var user in @class.ClassesUsers) {
                MailMessage message = new MailMessage();
                SmtpClient smtp = new SmtpClient();
                message.From = new MailAddress("Discoverksa123@hotmail.com");
                message.To.Add(new MailAddress(user.User.Email));
                message.Subject = "Class cancelled";
                var date = String.Format("{0:yyyy-MM-dd}", Convert.ToDateTime(user.Class.Start));
                var time = String.Format("{0:HH:MM}", Convert.ToDateTime(user.Class.Start));
                message.Body = user.Class.Text +  " class has been cancelled (" + date + " - " + time + ")";
                smtp.Port = 587;
                smtp.Host = "smtp-mail.outlook.com";
                smtp.EnableSsl = true;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential("Discoverksa123@hotmail.com", "Discover123");
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Send(message);
            }


            db.Classes.Remove(@class);
            db.SaveChanges();
            return Json(new { success = "ok" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Edit([Bind(Include = "ID,TrainerID,Text,Notes,BarColor,Seats,Start,End")] Class @class)
        {
            db.Entry(@class).State = EntityState.Modified;
            db.SaveChanges();
            return Json(new { success = "ok" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult JoinClass(int id)
        {

            int userid = Convert.ToInt32(Session["ID"].ToString());
            ClassesUser cu = new ClassesUser()
            {
                UserID = userid,
                Date = DateTime.Now,
                ClassID = id
            };
            db.ClassesUsers.Add(cu);
            db.SaveChanges();

            Class @class = db.Classes.Find(id);
            @class.Seats = @class.Seats - 1;
            db.SaveChanges();
            return Json(new { success = "ok" }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult DeleteMyClass(int id)
        {
            int userid = Convert.ToInt32(Session["ID"].ToString());
            var cu = db.ClassesUsers.Where(a => a.UserID == userid && a.ClassID == id).SingleOrDefault();
            db.ClassesUsers.Remove(cu);
            db.SaveChanges();

            Class @class = db.Classes.Find(id);
            @class.Seats = @class.Seats + 1;
            db.SaveChanges();

            return RedirectToAction("MyClass");
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
