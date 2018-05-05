using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebPro.Models;

namespace WebPro.Controllers
{
    public class HomeController : Controller
    {
        private WebEntities db = new WebEntities();
        //
        // GET: /Home/
        public ActionResult Index()
        {
            var best = from d in db.Blogs
                       orderby d.viewCount descending
                       select d;
            var source = from d in db.Sources
                         orderby d.downloadCount descending
                         select d;
            var photo = from d in db.Photos
                        join g in db.PhotoGroups on d.photoGroup equals g.id
                        orderby d.orderNum,d.publishTime descending
                        select new PhotoForPage { id=d.id, 
                            photoGroup=d.photoGroup, imgurls=d.imgurls, imgFolder=g.imgFolder };
            ViewBag.index = 1;
            ViewBag.skin = GlobalVar.skin;
            PagerQuery<IQueryable<Blogs>, IQueryable<Sources>, IQueryable<PhotoForPage>> query = new PagerQuery<IQueryable<Blogs>, IQueryable<Sources>,IQueryable<PhotoForPage>>(best.Take(5), source.Take(10),photo.Take(30));
            return View(query);
        }

        public void SetConfig(string skin)
        {
            GlobalVar.skin = skin;
        }

        public void LogMaintenance()
        {
            Logs log = new Logs();
            log.logtype = "用户访问";
            log.logcontent = "用户访问";
            log.logtime = DateTime.Now;
            log.loguser = "";
            log.logip = IpSupport.GetClientIp();
            log.logfree = IpSupport.GetAdrByIp(log.logip);
            db.Logs.Add(log);
            db.SaveChanges();
        }
    }
}
