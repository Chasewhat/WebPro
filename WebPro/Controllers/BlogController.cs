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
    public class BlogController : Controller
    {
        private WebEntities db = new WebEntities();

        //
        // GET: /Blog/

        public ActionResult Index(int pageIndex = 1, string searchKey = "", int searchFlag = 0)
        {
            ViewBag.skin = GlobalVar.skin;
            int pageSize = (searchFlag != 0 ? 99999 : 5);
            //从数据库在取得数据，并返回总记录数
            var temp = from d in db.Blogs
                       where (searchFlag==0?1==1:(searchFlag == 1 ? d.keyword.Contains(searchKey) : d.title.Contains(searchKey) || d.keyword.Contains(searchKey) ||
                       d.author.Contains(searchKey) || d.content.Contains(searchKey)))
                       orderby d.id descending
                       select d;
            int count = temp.Count();
            var tag = from d in db.Blogs
                      select d.keyword;
            List<string> list = new List<string>();
            foreach (var item in tag)
            {
                list.AddRange(item.Split('|'));
            }
            var best = from d in db.Blogs
                       orderby d.viewCount descending
                       select d;
            BlogRight<IEnumerable<string>, IQueryable<Blogs>> blogright =
                new BlogRight<IEnumerable<string>, IQueryable<Blogs>>(list.Distinct<string>(), best.Take(5));
            PagerInfo pager = new PagerInfo();
            pager.CurrentPageIndex = pageIndex;
            pager.PageSize = pageSize;
            pager.RecordCount = count;
            PagerQuery<PagerInfo, IQueryable<Blogs>, BlogRight<IEnumerable<string>, IQueryable<Blogs>>> query
                = new PagerQuery<PagerInfo, IQueryable<Blogs>, BlogRight<IEnumerable<string>,
                    IQueryable<Blogs>>>(pager, temp.Skip<Blogs>((pageIndex - 1) * pageSize).Take<Blogs>(pageSize), blogright);
            return View(query);
        }

        //public ActionResult Search(string searchKey = "", int pageIndex = 1)
        //{
        //    searchKey=searchKey.ToUpper();
        //    int pageSize = 3;
        //    //从数据库在取得数据，并返回总记录数
        //    var temp = from d in db.Blogs
        //               select d;
        //    temp = temp.Where<Blog>(s => s.title.Contains(searchKey) || s.content.Contains(searchKey) ||
        //        s.author.Contains(searchKey) || s.keyword.Contains(searchKey));
        //    int count = temp.Count();
        //    var tag = from d in db.Blogs
        //              select d.keyword;
        //    List<string> list = new List<string>();
        //    foreach (var item in tag)
        //    {
        //        list.AddRange(item.Split('|'));
        //    }
        //    var best = from d in db.Blogs
        //               orderby d.viewCount descending
        //               select d;
        //    BlogRight<IEnumerable<string>, IQueryable<Blog>> blogright =
        //        new BlogRight<IEnumerable<string>, IQueryable<Blog>>(list.Distinct<string>(), best.Take(5));
        //    PagerInfo pager = new PagerInfo();
        //    pager.CurrentPageIndex = pageIndex;
        //    pager.PageSize = pageSize;
        //    pager.RecordCount = count;
        //    PagerQuery<PagerInfo, IQueryable<Blog>, BlogRight<IEnumerable<string>, IQueryable<Blog>>> query
        //        = new PagerQuery<PagerInfo, IQueryable<Blog>, BlogRight<IEnumerable<string>,
        //            IQueryable<Blog>>>(pager, temp.OrderByDescending(s => s.publishTime).Skip<Blog>((pageIndex - 1) * pageSize).Take<Blog>(pageSize), blogright);
        //    return View("Index",query);
        //}

        //
        // GET: /Blog/Details/5

        public ActionResult Details(int id = 0)
        {
            ViewBag.skin = GlobalVar.skin;
            Blogs blog = db.Blogs.Find(id);
            if (blog == null)
            {
                return HttpNotFound();
            }
            Blogs before = db.Blogs.Where(s => s.id < id).OrderByDescending(s => s.id).FirstOrDefault();
            Blogs after = db.Blogs.Where(s => s.id > id).OrderBy(s => s.id).FirstOrDefault();
            before = (before == null ? blog : before);
            after = (after == null ? blog : after);
            blog.viewCount += 1;
            db.Entry(blog).State = EntityState.Modified;
            db.SaveChanges();

            List<Blogs> blogs = new List<Blogs>();
            blogs.Add(blog);
            blogs.Add(before);
            blogs.Add(after);
            var tag = from d in db.Blogs
                      select d.keyword;
            List<string> list = new List<string>();
            foreach (var item in tag)
            {
                list.AddRange(item.Split('|'));
            }
            var best = from d in db.Blogs
                       orderby d.viewCount descending
                       select d;
            BlogRight<IEnumerable<string>, IQueryable<Blogs>> blogright =
                new BlogRight<IEnumerable<string>, IQueryable<Blogs>>(list.Distinct<string>(), best.Take(5));
            DetailQuery<List<Blogs>, BlogRight<IEnumerable<string>, IQueryable<Blogs>>> query
                = new DetailQuery<List<Blogs>, BlogRight<IEnumerable<string>,
                    IQueryable<Blogs>>>(blogs, blogright);
            return View(query);
        }
    }
}