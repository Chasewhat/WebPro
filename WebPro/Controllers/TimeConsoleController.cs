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
    public class TimeConsoleController : Controller
    {
        private WebEntities db = new WebEntities();

        //
        // GET: /TimeConsole/

        public ActionResult Index()
        {
            return View(db.Essays.ToList());
        }

        public string GetData()
        {
            var pageSize = string.IsNullOrWhiteSpace(Request["limit"]) ? 10 : int.Parse(Request["limit"]);
            var offset = string.IsNullOrWhiteSpace(Request["offset"]) ? 0 : int.Parse(Request["offset"]);
            var sort = string.IsNullOrWhiteSpace(Request["sort"]) ? "id" : Request["sort"];
            var search = string.IsNullOrWhiteSpace(Request["search"]) ? "" : Request["search"];
            var order = string.IsNullOrWhiteSpace(Request["order"]) ? "asc" : Request["order"];

            var temp = from d in db.Essays
                       where d.title.Contains(search) || d.content.Contains(search)
                       select new { d.id, d.content, d.title, d.publishTime };
            //temp = temp.Skip(pageNum * pageSize).Take(pageSize);
            //temp = temp.Where(s => s.content.Contains(search));
            switch (sort)
            {
                case "title":
                    temp = order == "desc" ? temp.OrderByDescending(s => s.title) : temp.OrderBy(s => s.title);
                    break;
                case "content":
                    temp = order == "desc" ? temp.OrderByDescending(s => s.content) : temp.OrderBy(s => s.content);
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

        //public string GetData()
        //{
        //    var temp = from d in db.Essays
        //               orderby d.id
        //               select new { d.id, d.content, d.title, d.publishTime };
        //    return JsonConvert.SerializeObject(new { total = temp.Count(), rows = temp.ToList() });
        //    //return temp.ToJson();
        //}

        //
        // GET: /TimeConsole/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /TimeConsole/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Create(Essays essays)
        {
            if (ModelState.IsValid)
            {
                var temp = db.Essays.Select(s => s.orderNum).Max();
                essays.orderNum = temp + 1;
                essays.publishTime = DateTime.Now;
                db.Essays.Add(essays);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(essays);
        }

        //
        // GET: /TimeConsole/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Essays essays = db.Essays.Find(id);
            if (essays == null)
            {
                return HttpNotFound();
            }
            string initialPreview = "";
            IList<InitialPreviewConfig> list = new List<InitialPreviewConfig>();
            if (!string.IsNullOrEmpty(essays.imgurls))
            {
                int i = 1;
                foreach (var img in essays.imgurls.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    list.Add(new InitialPreviewConfig(i, "/assets/ueditor/net/controller.ashx?action=deletefile", new DataExtra(img)));
                    initialPreview += "<img src='" + img + "' class='file-preview-image'>|";
                }
                initialPreview = initialPreview.Substring(0, initialPreview.Length - 1);
                i++;
            }
            ViewBag.initialPreview = initialPreview;
            ViewBag.initialPreviewConfig = JsonConvert.SerializeObject(list);
            return View(essays);
        }

        //
        // POST: /TimeConsole/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit(Essays essays)
        {
            if (ModelState.IsValid)
            {
                db.Entry(essays).State = EntityState.Modified;
                essays.publishTime = DateTime.Now;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(essays);
        }

        //
        // GET: /TimeConsole/Delete/5
        [HttpPost, ActionName("Delete")]
        public string Delete(int id = 0)
        {
            Essays essays = db.Essays.Find(id);
            if (essays == null)
            {
                return "F";
            }
            db.Essays.Remove(essays);
            db.SaveChanges();
            return "S";
        }

        //
        // POST: /TimeConsole/Delete/5

        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    Essays essays = db.Essays.Find(id);
        //    db.Essays.Remove(essays);
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