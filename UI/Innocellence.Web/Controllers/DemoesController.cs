using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Infrastructure.Web.Domain.Entity;
using Innocellence.Web.Models;
using Infrastructure.Core.Data;
using Infrastructure.Web.Domain.Contracts;

namespace Innocellence.Web.Controllers
{
    public class DemoesController : Controller
    {
        private MySqlDbContext db = new MySqlDbContext();
        private IDemoService _demoService;
        private DemoesController(IDemoService demoService)
        {
            _demoService = demoService;
        }

        // GET: Demoes
        public ActionResult Index()
        {
            return View(db.Demoes.ToList());
        }

        // GET: Demoes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Demo demo = db.Demoes.Find(id);
            if (demo == null)
            {
                return HttpNotFound();
            }
            return View(demo);
        }

        // GET: Demoes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Demoes/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,CreateTime,UpdateTime,State")] Demo demo)
        {
            if (ModelState.IsValid)
            {
                db.Demoes.Add(demo);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(demo);
        }

        // GET: Demoes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Demo demo = db.Demoes.Find(id);
            if (demo == null)
            {
                return HttpNotFound();
            }
            return View(demo);
        }

        // POST: Demoes/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,CreateTime,UpdateTime,State")] Demo demo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(demo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(demo);
        }

        // GET: Demoes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Demo demo = db.Demoes.Find(id);
            if (demo == null)
            {
                return HttpNotFound();
            }
            return View(demo);
        }

        // POST: Demoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Demo demo = db.Demoes.Find(id);
            db.Demoes.Remove(demo);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
