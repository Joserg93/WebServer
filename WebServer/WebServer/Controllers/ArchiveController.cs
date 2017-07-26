using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebServer.Controllers
{
    public class ArchiveController : Controller
    {
        // GET: Archive
        public ActionResult Index()
        {
            return View();
        }

        // GET: Archive/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Archive/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Archive/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Archive/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Archive/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Archive/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Archive/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
