using Newtonsoft.Json;
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
    public class BlogConsoleController : Controller
    {
        private WebEntities db = new WebEntities();

        //
        // GET: /BlogConsole/

        public ActionResult Index()
        {
            return View(db.Blogs.ToList());
        }

        public string GetData()
        {

            var pageSize = string.IsNullOrWhiteSpace(Request["limit"]) ? 10 : int.Parse(Request["limit"]);
            var offset = string.IsNullOrWhiteSpace(Request["offset"]) ? 0 : int.Parse(Request["offset"]);
            var sort = string.IsNullOrWhiteSpace(Request["sort"]) ? "id" : Request["sort"];
            var search = string.IsNullOrWhiteSpace(Request["search"]) ? "" : Request["search"];
            var order = string.IsNullOrWhiteSpace(Request["order"]) ? "asc" : Request["order"];

            var temp = from d in db.Blogs
                       where d.title.Contains(search) ||d.keyword.Contains(search)
                       select new { d.id, d.keyword, d.title, d.publishTime };
            switch (sort)
            {
                case "title":
                    temp = order == "desc" ? temp.OrderByDescending(s => s.title) : temp.OrderBy(s => s.title);
                    break;
                case "keyword":
                    temp = order == "desc" ? temp.OrderByDescending(s => s.keyword) : temp.OrderBy(s => s.keyword);
                    break;
                case "publishTime":
                    temp = order == "desc" ? temp.OrderByDescending(s => s.publishTime) : temp.OrderBy(s => s.publishTime);
                    break;
                default:
                    temp = order == "desc" ? temp.OrderByDescending(s => s.id) : temp.OrderBy(s => s.id);
                    break;
            }
            return JsonConvert.SerializeObject(new { total = temp.Count(), 
                rows = temp.Skip(offset).Take(pageSize).ToList() });
            //return temp.ToJson();
        }
        //
        // GET: /BlogConsole/Details/5

        public ActionResult Details(int id = 0)
        {
            Blogs blogs = db.Blogs.Find(id);
            if (blogs == null)
            {
                return HttpNotFound();
            }
            return View(blogs);
        }

        //
        // GET: /BlogConsole/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /BlogConsole/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Create(Blogs blogs)
        {
            if (ModelState.IsValid)
            {
                var temp = db.Blogs.Select(s => s.orderNum).Max();
                string tempStr = "";
                string[] imgs = HtmlSupport.GetHtmlImageUrlList(blogs.content);
                
                foreach (var str in imgs)
                    tempStr += str + "|";
                blogs.imgurls = (!string.IsNullOrEmpty(tempStr) ? tempStr.Substring(0, tempStr.Length - 1) : "/upload/image/blog/20160101/def.jpg");
                blogs.orderNum = temp + 1;
                blogs.viewCount = 0;
                blogs.author = "zuorx";
                blogs.publishTime = DateTime.Now;
                db.Blogs.Add(blogs);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(blogs);
        }

        //
        // GET: /BlogConsole/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Blogs blogs = db.Blogs.Find(id);
            if (blogs == null)
            {
                return HttpNotFound();
            }
            return View(blogs);
        }

        //
        // POST: /BlogConsole/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit(Blogs blogs)
        {
            if (ModelState.IsValid)
            {
                db.Entry(blogs).State = EntityState.Modified;
                string[] imgs = HtmlSupport.GetHtmlImageUrlList(blogs.content);
                string tempStr = "";
                foreach (var str in imgs)
                    tempStr += str + "|";
                blogs.imgurls = (!string.IsNullOrEmpty(tempStr) ? tempStr.Substring(0, tempStr.Length - 1) : "/upload/image/blog/20160101/def.jpg");
                blogs.publishTime = DateTime.Now;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(blogs);
        }

        //
        // GET: /BlogConsole/Delete/5
        [HttpPost, ActionName("Delete")]
        public string Delete(int id = 0)
        {
            Blogs blogs = db.Blogs.Find(id);
            if (blogs == null)
            {
                return "F";
            }
            db.Blogs.Remove(blogs);
            db.SaveChanges();
            return "S";
        }

        //
        // POST: /BlogConsole/Delete/5

        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    Blogs blogs = db.Blogs.Find(id);
        //    db.Blogs.Remove(blogs);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}