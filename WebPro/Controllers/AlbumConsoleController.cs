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
    public class AlbumConsoleController : Controller
    {
        private WebEntities db = new WebEntities();

        //
        // GET: /Album/

        public ActionResult Index()
        {
            return View(db.PhotoGroups.ToList());
        }

        public string GetData()
        {
            var pageSize = string.IsNullOrWhiteSpace(Request["limit"]) ? 10 : int.Parse(Request["limit"]);
            var offset = string.IsNullOrWhiteSpace(Request["offset"]) ? 0 : int.Parse(Request["offset"]);
            var sort = string.IsNullOrWhiteSpace(Request["sort"]) ? "id" : Request["sort"];
            var search = string.IsNullOrWhiteSpace(Request["search"]) ? "" : Request["search"];
            var order = string.IsNullOrWhiteSpace(Request["order"]) ? "asc" : Request["order"];

            var temp = from d in db.PhotoGroups
                       where d.title.Contains(search) || d.content.Contains(search)
                       select new { d.id, d.content, d.title, d.imgFolder, d.publishTime };
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


        //
        // GET: /Album/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Album/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Create(PhotoGroups photogroups)
        {
            if (ModelState.IsValid)
            {
                photogroups.publishTime = DateTime.Now;
                var temp = db.PhotoGroups.Select(s => s.orderNum).Max();
                var urls = Request["imgurls"];
                photogroups.orderNum = temp + 1;
                if (!string.IsNullOrWhiteSpace(urls))
                {
                    photogroups.imgTitle = urls.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries)[0];
                }
                db.PhotoGroups.Add(photogroups);
                db.SaveChanges();

                //int albumId = photogroups.id;
                foreach (var img in urls.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    Photos p = new Photos();
                    p.title = photogroups.title;
                    p.content = photogroups.content;
                    p.photoGroup = photogroups.id;
                    p.publishTime = DateTime.Now;
                    p.imgurls = img;
                    p.orderNum = 1;
                    db.Photos.Add(p);
                }

                db.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(photogroups);
        }

        //
        // GET: /Album/Edit/5

        public ActionResult Edit(int id = 0)
        {
            PhotoGroups photogroups = db.PhotoGroups.Find(id);
            if (photogroups == null)
            {
                return HttpNotFound();
            }
            string initialPreview = "";
            string imgurls = "";
            IList<InitialPreviewConfig> list = new List<InitialPreviewConfig>();
            var temp = from d in db.Photos
                       where d.photoGroup == id
                       orderby d.id descending
                       select d;
            int i = 1;
            foreach (var p in temp)
            {
                if (!string.IsNullOrEmpty(p.imgurls))
                {
                    list.Add(new InitialPreviewConfig(i, "/assets/ueditor/net/controller.ashx?action=deletefile", new DataExtra(p.imgurls)));
                    initialPreview += "<img src='" + p.imgurls + "' class='file-preview-image'>|";
                    imgurls += p.imgurls + "|";
                    
                    i++;
                }
            }
            initialPreview = initialPreview.Substring(0, initialPreview.Length - 1);
            imgurls = imgurls.Substring(0, imgurls.Length - 1);
            ViewBag.imgurls = imgurls;
            ViewBag.initialPreview = initialPreview;
            ViewBag.initialPreviewConfig = JsonConvert.SerializeObject(list);
            return View(photogroups);
        }

        //
        // POST: /Album/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit(PhotoGroups photogroups)
        {
            if (ModelState.IsValid)
            {
                db.Entry(photogroups).State = EntityState.Modified;
                photogroups.publishTime = DateTime.Now;
                var urls = Request["imgurls"];
                if (!string.IsNullOrWhiteSpace(urls))
                {
                    photogroups.imgTitle = urls.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries)[0];
                }

                IList<string> list = urls.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                //int albumId = photogroups.id;
                foreach (var p in db.Photos.Where(s => s.photoGroup == photogroups.id).Distinct())
                {
                    if (list.Where(l => l == p.imgurls).Count() <= 0)
                        db.Photos.Remove(p);
                }

                foreach (var img in urls.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    var temp = from d in db.Photos
                               where d.imgurls == img
                               select d;

                    if (temp.Count() <= 0)
                    {
                        Photos p = new Photos();
                        p.title = photogroups.title;
                        p.content = photogroups.content;
                        p.photoGroup = photogroups.id;
                        p.publishTime = DateTime.Now;
                        p.imgurls = img;
                        p.orderNum = 1;
                        db.Photos.Add(p);
                    }

                }
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(photogroups);
        }

        //
        // GET: /Album/Delete/5
        [HttpPost, ActionName("Delete")]
        public string Delete(int id = 0)
        {
            PhotoGroups photogroups = db.PhotoGroups.Find(id);
            if (photogroups == null)
            {
                return "F";
            }
            //var
            db.PhotoGroups.Remove(photogroups);
            var temp = from d in db.Photos
                       where d.photoGroup == id
                       orderby d.id descending
                       select d;
            foreach (var p in temp)
            {
                db.Photos.Remove(p);
            }
            db.SaveChanges();
            return "S";
        }

        //
        // POST: /Album/Delete/5

        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    PhotoGroups photogroups = db.PhotoGroups.Find(id);
        //    db.PhotoGroups.Remove(photogroups);
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