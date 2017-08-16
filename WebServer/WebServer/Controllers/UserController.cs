using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Mvc;
using WebServer.Models;

namespace WebServer.Controllers
{
    public class UserController : Controller
    {
        // GET: User
        public ActionResult Login()
        {
            Session["id_user"] = null;
            Session["token"] = null;
            Session["email"] = null;
            return View();    
        }

        public async System.Threading.Tasks.Task<ActionResult> Auth(User model)
        {
            try
            {
                var httpClient = new HttpClient();
                var stringContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync("http://localhost:8000/api/auth_login"+"", stringContent);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var token = await response.Content.ReadAsStringAsync();
                    token = token.ToString().Replace('"', ' ').Trim();
                    String url = "http://localhost:8000/api/auth_user" + "?token="+token;
                    stringContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                    response = await httpClient.PostAsync(url, stringContent);
                    var id = response.Content.ReadAsStringAsync();
                    Session["id_user"] = int.Parse(id.Result.ToString());
                    Session["token"] = token;
                    Session["email"] = model.email;
                    return RedirectToAction("Index", "Profile");
                }
                else if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    ModelState.AddModelError(String.Empty, "Error: email o contraseña incorrectos");
                    return View("Login");
                }
                else
                {
                    ModelState.AddModelError(String.Empty, "Error: se produjo un problema en la conexión");
                    return View("Login");
                }
            }
            catch (Exception)
            {
                ModelState.AddModelError(String.Empty, "Error: se produjo un problema en la aplicación");
                return View("Login");
            }
        }

        // GET: User/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: User/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: User/Create
        [HttpPost]
        public async System.Threading.Tasks.Task<ActionResult> Create(Register model)
        {
            try
            {
                if (model.password.Equals(model.confirmPassword))
                {
                    var httpClient = new HttpClient();
                    var stringContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                    var response = await httpClient.PostAsync("http://localhost:8000/api/auth_email", stringContent);
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        stringContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                        response = await httpClient.PostAsync("http://localhost:8000/api/users", stringContent);
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            return RedirectToAction("Login", "User");
                        }
                        else
                        {
                            ModelState.AddModelError(String.Empty, "Error: se produjo un problema en la conexión");
                            return View();
                        }
                    }
                    else if(response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        ModelState.AddModelError(String.Empty, "Error: el email ingresado ya se encuentra en el sistema, por favor ingrese otro");
                        return View();
                    }
                    else
                    {
                        ModelState.AddModelError(String.Empty, "Error: se produjo un problema en la conexión");
                        return View();
                    }
                }
                else
                {
                    ModelState.AddModelError(String.Empty, "Error: los campos de contraseña y confirmar contraseña no coinciden");
                    return View();
                }            
            }
            catch (Exception)
            {
                throw;
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
