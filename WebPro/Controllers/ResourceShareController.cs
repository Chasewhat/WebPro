using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebPro.Models;
using System.Data;
using System.Data.Entity;

namespace WebPro.Controllers
{
    public class SourceCodeController : Controller
    {
        private WebEntities db = new WebEntities();
        //
        // GET: /SourceCode/

        public ActionResult Index(int pageIndex = 1, string searchKey = "", int searchId = 0, int searchFlag = 0)
        {
            ViewBag.skin = GlobalVar.skin;
            int pageSize = 5;
            ViewBag.countFlag = 1;
            //从数据库在取得数据，并返回总记录数
            var temp = from d in db.Sources
                       where (searchFlag == 1 ? d.keyword.Contains(searchKey) :
                       (searchFlag == 2 ? d.id == searchId : 
                       d.title.Contains(searchKey) || d.keyword.Contains(searchKey) ||
                       d.author.Contains(searchKey) || d.content.Contains(searchKey)))
                       orderby d.orderNum descending
                       select d;
            int count = temp.Count();
            var tag = from d in db.Sources
                      select d.keyword;
            List<string> list = new List<string>();
            foreach (var item in tag)
            {
                list.AddRange(item.Split('|'));
            }
            var best = from d in db.Sources
                       orderby d.downloadCount descending
                       select d;
            BlogRight<IEnumerable<string>, IQueryable<Sources>> blogright =
                new BlogRight<IEnumerable<string>, IQueryable<Sources>>(list.Distinct<string>(), best.Take(5));
            PagerInfo pager = new PagerInfo();
            pager.CurrentPageIndex = pageIndex;
            pager.PageSize = pageSize;
            pager.RecordCount = count;
            PagerQuery<PagerInfo, IQueryable<Sources>, BlogRight<IEnumerable<string>, IQueryable<Sources>>> query
                = new PagerQuery<PagerInfo, IQueryable<Sources>, BlogRight<IEnumerable<string>,
                    IQueryable<Sources>>>(pager, temp.Skip<Sources>((pageIndex - 1) * pageSize).Take<Sources>(pageSize), blogright);
            return View(query);
        }

        public int Dowload(int id = 0)
        {
            Sources source = db.Sources.Find(id);
            if (source == null)
            {
                return 0;
            }
            source.downloadCount += 1;
            db.Entry(source).State = EntityState.Modified;
            db.SaveChanges();
            return source.downloadCount;
        }
    }
}
