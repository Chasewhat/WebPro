using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebPro.Models;

namespace WebPro.Controllers
{
    public class PhotoController : Controller
    {
        private WebEntities db = new WebEntities();

        //
        // GET: /Blog/

        public ActionResult Index(int pageIndex = 1)
        {
            ViewBag.skin = GlobalVar.skin;
            //从数据库在取得数据，并返回总记录数
            var temp = from d in db.PhotoGroups
                       orderby d.id
                       select d;
            
            return View(temp.ToList<PhotoGroups>());
        }
        //
        // GET: /Blog/Details/5

        public ActionResult Details(int id = 0, int pageIndex = 1,int showId=0)
        {
            ViewBag.skin = GlobalVar.skin;
            int pageSize = 200;
            var temp = from d in db.Photos
                       where d.photoGroup==id
                       orderby d.id
                       select d;
            int count = temp.Count();
            PagerInfo pager = new PagerInfo();
            pager.CurrentPageIndex = pageIndex;
            pager.PageSize = pageSize;
            pager.RecordCount = count;
            var group = from d in db.PhotoGroups
                        where d.id == id
                        select d;
            ViewBag.groupTitle = group.First<PhotoGroups>().title;
            ViewBag.imgFolder = group.First<PhotoGroups>().imgFolder;
            ViewBag.showId = showId;
            PagerTimeQuery<PagerInfo, IQueryable<Photos>> query
                = new PagerTimeQuery<PagerInfo, IQueryable<Photos>>(pager, temp.Skip<Photos>((pageIndex - 1) * pageSize).Take<Photos>(pageSize));
            return View(query);
        }
    }
}