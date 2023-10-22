using Gym.Models;
using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web.Mvc;

namespace Gym.Controllers
{
    public class HomeController : Controller
    {
        private OurFitnessDBEntities db = new OurFitnessDBEntities();
        public ActionResult Index()
        {
            return View(db.Packages.ToList());
        }
    }
}