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
    public class PhotoConsoleController : Controller
    {
        private WebEntities db = new WebEntities();

        //
        // GET: /PhotoConsole/

        public ActionResult Index()
        {
            return View(db.Photos.ToList());
        }

        //
        // GET: /PhotoConsole/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /PhotoConsole/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Create(Photos photos)
        {
            if (ModelState.IsValid)
            {
                db.Photos.Add(photos);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(photos);
        }

        //
        // GET: /PhotoConsole/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Photos photos = db.Photos.Find(id);
            if (photos == null)
            {
                return HttpNotFound();
            }
            return View(photos);
        }

        //
        // POST: /PhotoConsole/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit(Photos photos)
        {
            if (ModelState.IsValid)
            {
                db.Entry(photos).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(photos);
        }

        //
        // GET: /PhotoConsole/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Photos photos = db.Photos.Find(id);
            if (photos == null)
            {
                return HttpNotFound();
            }
            return View(photos);
        }

        //
        // POST: /PhotoConsole/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Photos photos = db.Photos.Find(id);
            db.Photos.Remove(photos);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}