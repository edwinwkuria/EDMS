using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EDMS.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Department()
        {
            return View("~/Views/Departments/Index.cshtml");
        }

       

        public ActionResult Division()
        {
            return View("~/Views/Divisions/Index.cshtml");
        }

        public ActionResult Category()
        {
            return View("~/Views/Categories/Index.cshtml");
        }
        public ActionResult Document()
        {
            return View("~/Views/Documents/Index.cshtml");
        }
        public ActionResult CreateDocument()
        {
            return View("~/Views/Documents/Create.cshtml");
        }
    }
}