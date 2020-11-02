using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System.Diagnostics;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;

using FaceFinder.Data.Interfaces;
using FaceFinder.Data.Models;
using System.Net.Http;
using System.Net;
using Newtonsoft.Json;
using System.Text;
using System.Buffers.Text;
using System.IO;
using System.Security.Cryptography;
using System.Net.Http.Headers;

//using System.Web;
//using System.Web.Mvc;
//

namespace FaceFinder.Controllers
{
    public class HomeController : Controller
    {
        //private readonly IFaces _faces;
        //private readonly IPersons _persons;
        HttpClient _api;
        public HomeController(IFaces ifaces, IPersons ipersons)
        {
            //_faces = ifaces;
            //_persons = ipersons;
            _api = new HttpClient();
            _api.BaseAddress = new Uri("https://localhost:44321/");

        }
        public async Task<ViewResult> ListPerson()
        {
            HttpResponseMessage response = await _api.GetAsync("api/person/");
            List<Person> persons = null;
            if (response.IsSuccessStatusCode)
                persons = JsonConvert.DeserializeObject<List<Person>>(response.Content.ReadAsStringAsync().Result);
            return View(persons);
        }
        //public ViewResult GetFace()
        //{
        //    var persons = _persons.AllPersons;
        //    return View(persons);
        //}
        [HttpGet]
        public async Task<ViewResult> EditPerson()
        {
            HttpResponseMessage response = await _api.GetAsync("api/person/" + (string)RouteData.Values["person_id"]);
            Person person = null;
            if (response.IsSuccessStatusCode)
                person = JsonConvert.DeserializeObject<Person>(response.Content.ReadAsStringAsync().Result);
            //var person = _persons.AllPersons.FirstOrDefault(a=>a.id==int.Parse((string)RouteData.Values["person_id"]));
            return View(person);
        }
        [HttpPost]
        public async Task<ActionResult> EditPerson(Person person)
        {
            //_persons.UpdatePerson(person);
            string json = JsonConvert.SerializeObject(person);
            StringContent stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");
            string uri = "api/person/" + (string)RouteData.Values["person_id"];
            HttpResponseMessage response = await _api.PostAsync(uri, stringContent);
            return RedirectToAction("ListPerson");
        }
        [HttpGet]
        public ViewResult AddPerson()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> AddPerson(Person person)
        {
            //_persons.UpdatePerson(person);
            string json = JsonConvert.SerializeObject(person);
            StringContent stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");
            string uri = "api/person/add";
            HttpResponseMessage response = await _api.PostAsync(uri, stringContent);
            return RedirectToAction("ListPerson");
        }
        [HttpGet]
        public ViewResult AddFace()
        {
            ViewBag.person_id = int.Parse((string)RouteData.Values["person_id"]);
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> AddFace(Face face, IFormFile photo)
        {
            //StreamContent str = new StreamContent(photo.OpenReadStream());
            //var response = _api.PostAsync("api/person/" + face.id_person.ToString() + "/face/add", str);

            if (photo!=null)
            {

                HttpResponseMessage response;
                using (var formContent = new MultipartFormDataContent())//"NKdKd9Yk"
                {
                    StreamContent sc = new StreamContent(photo.OpenReadStream());
                    formContent.Headers.ContentType.MediaType = "multipart/form-data";
                    formContent.Add(sc, "photo", photo.FileName);
                    response = await _api.PostAsync("api/person/" + face.id_person.ToString() + "/face/add", formContent);
                }

                Debug.WriteLine("response code = " + response.StatusCode);
                return Redirect("~/person/" + face.id_person.ToString() + "/face");

            }
            else return NotFound();
            //if (photo != null)
            //    _faces.AddFace(face, photo);

        }
        public async Task<ActionResult> DeletePerson()
        {
            HttpResponseMessage response = await _api.GetAsync("api/person/" + (string)RouteData.Values["person_id"] + "/delete");
            //_persons.DeletePerson(int.Parse((string)RouteData.Values["person_id"]));
            return RedirectToAction("ListPerson");
        }
        public async Task<ActionResult> DeleteFace()
        {
            HttpResponseMessage response = await _api.GetAsync("api/person/" + (string)RouteData.Values["person_id"] + "/face/"+ (string)RouteData.Values["face_id"] +"/delete");

            //_faces.DeleteFace(person_id, int.Parse((string)RouteData.Values["face_id"]));
            return Redirect("~/person/" + (string)RouteData.Values["person_id"] + "/face/");
        }
        public async Task<ViewResult> ListFace()
        {

            HttpResponseMessage response = await _api.GetAsync("api/person/" + (string)RouteData.Values["person_id"] + "/face");
            List<Face> faces = null;
            if (response.StatusCode == HttpStatusCode.OK)
                faces = JsonConvert.DeserializeObject<List<Face>>(response.Content.ReadAsStringAsync().Result);
            else faces = new List<Face>();
            ViewBag.person_id = int.Parse((string)RouteData.Values["person_id"]);
            //Convert.(faces[0].base64string)
            //Debug.WriteLine("SO WHAT "+faces[0].base64string.Length);

            //var img = Convert.FromBase64String(faces[0].base64string);
            //using (var ms = new MemoryStream(img))
            //{
            //    var fs=System.IO.File.Create("555555"+Path.GetFileName(faces[0].fileaddress));                
            //    ms.CopyTo(fs, img.Length);
            //}
            //return View(faces);

            //int person_id = int.Parse((string)RouteData.Values["person_id"]);
            //var faces=_faces.GetFaces(person_id);
            //
            return View(faces);
        }
        [HttpGet]
        public ViewResult FindPerson()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> FindPerson(IFormFile photo)
        {
            if (photo==null)
            {
                Debug.WriteLine("Home.FindPerson photo == null");
                return Redirect("~/person/0");
            }
            else
            {
                HttpResponseMessage response;
                using (var formContent = new MultipartFormDataContent())
                {
                    StreamContent sc = new StreamContent(photo.OpenReadStream());
                    formContent.Add(sc, "photo", photo.FileName);
                    response = await _api.PostAsync("api/find", formContent);
                }

                if (response.StatusCode == HttpStatusCode.OK)
                    return Redirect("~/person/"+ JsonConvert.DeserializeObject<Person>(response.Content.ReadAsStringAsync().Result).id);
                else return Redirect("~/person/0");


            }

        }
        ViewResult Empty(string text)
        {
            //ViewBag.Text = text;
            return View(text);
        } 
        
    }
}
