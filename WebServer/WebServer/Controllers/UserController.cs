using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using WebServer.Models;

namespace WebServer.Controllers
{
    public class UserController : Controller
    {
        // GET: User
        public async System.Threading.Tasks.Task<ActionResult> Index()
        {
            var httpClient = new HttpClient();
            var json = await httpClient.GetStringAsync("http://localhost:8000/api/users");
            var listArchives = JsonConvert.DeserializeObject<List<User>>(json);
            return View(listArchives);
        }

        // GET: User/Details/5
        public async System.Threading.Tasks.Task<ActionResult> Details(int id)
        {
            var httpClient = new HttpClient();
            var json = await httpClient.GetStringAsync("http://localhost:8000/api/users/" + id);
            var listArchives = JsonConvert.DeserializeObject<List<User>>(json);
            return View();
        }

        // GET: User/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: User/Create
        [HttpPost]
        public async System.Threading.Tasks.Task<ActionResult> Create(User user)
        {
            try
            {
                var httpClient = new HttpClient();
                HttpResponseMessage response = await httpClient.PostAsync("http://localhost:8000/api/users",user);
                response.EnsureSuccessStatusCode();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: User/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: User/Edit/5
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

        // GET: User/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: User/Delete/5
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
