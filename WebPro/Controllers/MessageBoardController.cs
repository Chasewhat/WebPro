using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebPro.Models;

namespace WebPro.Controllers
{
    public class MessageBoardController : Controller
    {
        //
        // GET: /MessageBoard/

        public ActionResult Index()
        {
            ViewBag.skin = GlobalVar.skin;
            return View();
        }

    }
}
