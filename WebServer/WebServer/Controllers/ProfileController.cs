using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebServer.Models;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;
using System.IO;
using System.Net;

namespace WebServer.Controllers
{
    public class ProfileController : Controller
    {
        private async System.Threading.Tasks.Task<bool> validate()
        {
            try
            {
                if (Session["token"] != null)
                {
                    var token = Session["token"];
                    var httpClient = new HttpClient();
                    var response = await httpClient.GetAsync("http://localhost:8000/api/auth_session" + "?token=" + token);
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                ModelState.AddModelError(String.Empty, "Error: se perdio conexión con el servidor");
                RedirectToAction("Login", "User");
                return false;
            }          
        }
        // GET: Archive
        public async System.Threading.Tasks.Task<ActionResult> Index()
        {
            try
            {
                if (await validate() == true)
                {
                    var id = Session["id_user"];
                    var token = Session["token"];
                    Account model = new Account();
                    model.user_id = (int)id;
                    if (id != null)
                    {
                        List<Account> list = null;
                        var httpClient = new HttpClient();
                        var stringContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                        var response = await httpClient.PostAsync("http://localhost:8000/api/profile" + "?token=" + token, stringContent);
                        var jsonString = await response.Content.ReadAsStringAsync();
                        list = JsonConvert.DeserializeObject<List<Account>>(jsonString);
                        return View(list);
                    }
                    else
                    {
                        ModelState.AddModelError(String.Empty, "Error: se perdio conexión con el servidor");
                        return RedirectToAction("Login", "User");
                    }    
                }
                else
                {
                    ModelState.AddModelError(String.Empty, "Error: la sesión del servidor esta expirada, por favor vuelva a loguearse para renovar la sesión");
                    return RedirectToAction("Login", "User");
                }
                
            }
            catch (Exception)
            {
                ModelState.AddModelError(String.Empty, "Error: se perdio conexión con el servidor");
                return RedirectToAction("Login", "User");
            }
            
        }
        public async System.Threading.Tasks.Task<ActionResult> Create()
        {
            if (await validate() == true)
            {
                return View();
            }
            else
            {
                ModelState.AddModelError(String.Empty, "Error: la sesión del servidor esta expirada, por favor vuelva a loguearse para renovar la sesión");
                return RedirectToAction("Login", "User");
            }          
        }
        [HttpPost]
        public async System.Threading.Tasks.Task<ActionResult> Create(HttpPostedFileBase file)
        {
            try
            {
                if (file != null)
                {
                    if (file.ContentLength > 0)
                    {
                        var extension = Path.GetExtension(file.FileName);
                        if (extension == ".txt")
                        {
                            string result = new StreamReader(file.InputStream).ReadToEnd();
                            string[] content = result.Split('\n');
                            string[] lines = new string[content.Length];
                            DateTime fileTime = DateTime.Now;
                            var filePath = Path.Combine(Server.MapPath("~/Documents"), fileTime.ToString("dd_MM_yyyy_HH_mm_ss-") + Path.GetFileName(file.FileName));
                            int x = 0;
                            foreach (var item in content)
                            {
                                lines[x] = coder(item.Replace("\r", ""));
                                x = x + 1;
                            }
                            using (StreamWriter sw = new StreamWriter(filePath, true))
                            {
                                foreach (var item in lines)
                                {
                                    sw.WriteLine(item);
                                }
                                sw.Close();
                            }
                            var id = Session["id_user"];
                            var token = Session["token"];
                            Account model = new Account();
                            if (id != null)
                            {
                                model.name = file.FileName;
                                model.date = fileTime.ToString("dd/MM/yyyy HH:mm:ss");
                                model.text = filePath;
                                model.user_id = (int)id;
                                var httpClient = new HttpClient();
                                var stringContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                                var response = await httpClient.PostAsync("http://localhost:8000/api/archives" + "?token=" + token, stringContent);
                                if (response.StatusCode == HttpStatusCode.OK)
                                {
                                    return RedirectToAction("Index");
                                }
                            }
                            else
                            {
                                ModelState.AddModelError(String.Empty, "Error: fallo al subir el archivo");
                                return View();
                            }
                        }
                        else
                        {
                            ModelState.AddModelError(String.Empty, "Error: solo se permiten archivos con extención .txt");
                            return View();
                        }

                    }
                    else
                    {
                        ModelState.AddModelError(String.Empty, "Error: el archivo esta vacio");
                        return View();
                    }
                }
                ModelState.AddModelError(String.Empty, "Error: no se a seleccionado ningun archivo");
                return View();           
            }
            catch
            {
                ModelState.AddModelError(String.Empty, "Error: se produjo un problema en la aplicación");
                return View();
            }
        }
        public async System.Threading.Tasks.Task<ActionResult> Delete(int id)
        {
            try
            {
                Account list = null;
                var token = Session["token"];
                var httpClient = new HttpClient();
                var response = await httpClient.GetAsync("http://localhost:8000/api/archives/" + id + "?token=" + token);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    list = JsonConvert.DeserializeObject<Account>(jsonString);
                    System.IO.File.Delete(list.text);
                    response = await httpClient.DeleteAsync("http://localhost:8000/api/archives/" + id + "?token=" + token);
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        return RedirectToAction("Index");
                    }
                    ModelState.AddModelError(String.Empty, "Error: se produjo un problema en la conexión");
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError(String.Empty, "Error: se produjo un problema en la conexión");
                    return RedirectToAction("Index");
                }               
            }
            catch (Exception)
            {
                ModelState.AddModelError(String.Empty, "Error: se produjo un problema en la conexión");
                return RedirectToAction("Index");
            }
            
        }
        public async System.Threading.Tasks.Task<FileContentResult> Download(int id)
        {
            try
            {
                Account list = null;
                var token = Session["token"];
                var httpClient = new HttpClient();
                var response = await httpClient.GetAsync("http://localhost:8000/api/archives/" + id + "?token=" + token);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    list = JsonConvert.DeserializeObject<Account>(jsonString);
                    string result = new StreamReader(list.text).ReadToEnd();
                    string[] content = result.Split('\n');
                    string[] lines = new string[content.Length];
                    var filePath = Path.Combine(Server.MapPath("~/Temp"), list.name);
                    int x = 0;
                    foreach (var item in content)
                    {
                        lines[x] = decoder(item.Replace("\r", ""));
                        x = x + 1;
                    }
                    StringWriter sw = new StringWriter();
                    using (sw)
                    {
                        foreach (var item in lines)
                        {
                            sw.WriteLine(item);
                        }
                    }
                    String contenido = sw.ToString();
                    return File(new UTF8Encoding().GetBytes(contenido), "text/plain",list.name);
                }
                else
                {
                    ModelState.AddModelError(String.Empty, "Error: se produjo un problema en la conexión");
                    return null;
                }           
            }
            catch (Exception)
            {
                ModelState.AddModelError(String.Empty, "Error: se produjo un problema en la conexión");
                return null;
            }
            
        }
        private string coder(string line)
        {
            StringBuilder sb = new StringBuilder(line);
            sb = sb.Replace("a", "☺");
            sb = sb.Replace("a", "☺");
            sb = sb.Replace("b", "☻");
            sb = sb.Replace("c", "♥");
            sb = sb.Replace("d", "♦");
            sb = sb.Replace("e", "♣");
            sb = sb.Replace("f", "♠");
            sb = sb.Replace("g", "•");
            sb = sb.Replace("h", "◘");
            sb = sb.Replace("i", "◙");
            sb = sb.Replace("j", "♂");
            sb = sb.Replace("k", "♀");
            sb = sb.Replace("l", "♪");
            sb = sb.Replace("m", "♫");
            sb = sb.Replace("n", "►");
            sb = sb.Replace("o", "◄");
            sb = sb.Replace("p", "↕");
            sb = sb.Replace("q", "¶");
            sb = sb.Replace("r", "§");
            sb = sb.Replace("s", "▬");
            sb = sb.Replace("t", "↨");
            sb = sb.Replace("u", "↑");
            sb = sb.Replace("v", "↓");
            sb = sb.Replace("w", "→");
            sb = sb.Replace("x", "←");
            sb = sb.Replace("y", "∟");
            sb = sb.Replace("z", "↔");
            sb = sb.Replace("1", "▲");
            sb = sb.Replace("2", "▼");
            sb = sb.Replace("3", "ß");
            sb = sb.Replace("4", "Ô");
            sb = sb.Replace("5", "µ");
            sb = sb.Replace("6", "þ");
            sb = sb.Replace("7", "Þ");
            sb = sb.Replace("8", "Û");
            sb = sb.Replace("9", "±");
            sb = sb.Replace("0", "¾");
            sb = sb.Replace(",", "¹");
            sb = sb.Replace(".", "³");
            return Convert.ToString(sb);
        }
        private string decoder(string line)
        {
            StringBuilder sb = new StringBuilder(line);
            sb = sb.Replace("☺","a");
            sb = sb.Replace("☺","a");
            sb = sb.Replace("☻","b");
            sb = sb.Replace("♥","c");
            sb = sb.Replace("♦","d");
            sb = sb.Replace("♣","e");
            sb = sb.Replace("♠","f");
            sb = sb.Replace("•","g");
            sb = sb.Replace("◘","h");
            sb = sb.Replace("◙","i");
            sb = sb.Replace("♂","j");
            sb = sb.Replace("♀","k");
            sb = sb.Replace("♪","l");
            sb = sb.Replace("♫","m");
            sb = sb.Replace("►","n");
            sb = sb.Replace("◄","o");
            sb = sb.Replace("↕","p");
            sb = sb.Replace("¶","q");
            sb = sb.Replace("§","r");
            sb = sb.Replace("▬","s");
            sb = sb.Replace("↨","t");
            sb = sb.Replace("↑","u");
            sb = sb.Replace("↓","v");
            sb = sb.Replace("→","w");
            sb = sb.Replace("←","x");
            sb = sb.Replace("∟","y");
            sb = sb.Replace("↔","z");
            sb = sb.Replace("▲","1");
            sb = sb.Replace("▼","2");
            sb = sb.Replace("ß","3");
            sb = sb.Replace("Ô","4");
            sb = sb.Replace("µ","5");
            sb = sb.Replace("þ","6");
            sb = sb.Replace("Þ","7");
            sb = sb.Replace("Û","8");
            sb = sb.Replace("±","9");
            sb = sb.Replace("¾","0");
            sb = sb.Replace("¹",",");
            sb = sb.Replace("³",".");
            return Convert.ToString(sb);
        }
    }
}