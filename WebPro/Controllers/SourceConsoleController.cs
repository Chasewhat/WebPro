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
    public class SourceConsoleController : Controller
    {
        private WebEntities db = new WebEntities();

        //
        // GET: /SourceConsole/

        public ActionResult Index()
        {
            return View(db.Sources.ToList());
        }

        public string GetData()
        {
            var pageSize = string.IsNullOrWhiteSpace(Request["limit"]) ? 10 : int.Parse(Request["limit"]);
            var offset = string.IsNullOrWhiteSpace(Request["offset"]) ? 0 : int.Parse(Request["offset"]);
            var sort = string.IsNullOrWhiteSpace(Request["sort"]) ? "id" : Request["sort"];
            var search = string.IsNullOrWhiteSpace(Request["search"]) ? "" : Request["search"];
            var order = string.IsNullOrWhiteSpace(Request["order"]) ? "asc" : Request["order"];

            var temp = from d in db.Sources
                       where d.title.Contains(search) || d.keyword.Contains(search)
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
            return JsonConvert.SerializeObject(new { total = temp.Count(), rows = temp.Skip(offset).Take(pageSize).ToList() });
            //return temp.ToJson();
        }
        //
        // GET: /SourceConsole/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /SourceConsole/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Create(Sources sources)
        {
            if (ModelState.IsValid)
            {
                var temp = db.Sources.Select(s => s.orderNum).Max();
                sources.orderNum = temp + 1;
                sources.downloadCount = 0;
                sources.author = "zuorx";
                sources.publishTime = DateTime.Now;
                db.Sources.Add(sources);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(sources);
        }

        //
        // GET: /SourceConsole/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Sources sources = db.Sources.Find(id);
            if (sources == null)
            {
                return HttpNotFound();
            }
            string initialPreview = "";
            IList<InitialPreviewConfig> list = new List<InitialPreviewConfig>();
            if (!string.IsNullOrEmpty(sources.imgurls))
            {
                int i=1;
                foreach (var img in sources.imgurls.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    list.Add(new InitialPreviewConfig(i, "/assets/ueditor/net/controller.ashx?action=deletefile", new DataExtra(img)));
                    initialPreview += "<img src='" + img + "' class='file-preview-image'>|";
                }
                initialPreview = initialPreview.Substring(0, initialPreview.Length - 1);
                i++;
            }
            ViewBag.initialPreview = initialPreview;
            ViewBag.initialPreviewConfig = JsonConvert.SerializeObject(list);
            return View(sources);
        }

        //
        // POST: /SourceConsole/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit(Sources sources)
        {
            if (ModelState.IsValid)
            {
                db.Entry(sources).State = EntityState.Modified;
                sources.publishTime = DateTime.Now;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(sources);
        }

        //
        // GET: /SourceConsole/Delete/5
        [HttpPost, ActionName("Delete")]
        public string Delete(int id = 0)
        {
            Sources sources = db.Sources.Find(id);
            if (sources == null)
            {
                return "F";
            }
            db.Sources.Remove(sources);
            db.SaveChanges();
            return "S";
        }

        //
        // POST: /SourceConsole/Delete/5

        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    Sources sources = db.Sources.Find(id);
        //    db.Sources.Remove(sources);
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