using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebPro.Models;

namespace WebPro.Controllers
{
    public class ConsoleController : Controller
    {
        private WebEntities db = new WebEntities();
        //
        // GET: /Console/

        public ActionResult Index()
        {
            var blog = from d in db.Blogs
                       select d;
            var source = from d in db.Sources
                         select d;
            var photo = from d in db.Photos
                        select d;
            var essay = from d in db.Essays
                        select d;
            ViewBag.bsum = blog.Count<Blogs>();
            ViewBag.ssum = source.Count<Sources>();
            ViewBag.psum = photo.Count<Photos>();
            ViewBag.tsum = essay.Count<Essays>();
            return View();
        }
    }
}
