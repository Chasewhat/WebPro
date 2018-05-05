using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebPro.Models;

namespace WebPro.Controllers
{
    public class TimeLineController : Controller
    {
        private WebEntities db = new WebEntities();
        //
        // GET: /TimeLine/

        public ActionResult Index(int pageIndex = 1)
        {
            ViewBag.skin = GlobalVar.skin;
            int pageSize = 20;
            ViewBag.countFlag = 1;
            //从数据库在取得数据，并返回总记录数
            var temp = from d in db.Essays
                       orderby d.publishTime descending
                       select d;
            int count = temp.Count();
            PagerInfo pager = new PagerInfo();
            pager.CurrentPageIndex = pageIndex;
            pager.PageSize = pageSize;
            pager.RecordCount = count;
            PagerTimeQuery<PagerInfo, IQueryable<Essays>> query
                = new PagerTimeQuery<PagerInfo, IQueryable<Essays>>(pager, temp.Skip<Essays>((pageIndex - 1) * pageSize).Take<Essays>(pageSize));
            return View(query);
        }

    }
}
