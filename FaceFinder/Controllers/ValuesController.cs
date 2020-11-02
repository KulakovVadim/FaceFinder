using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using FaceFinder.Data.Interfaces;
using FaceFinder.Data.Models;
using System.Net;
using System.Net.Http;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Hosting;
using System.Net.Http.Headers;

namespace FaceFinder.Controllers
{
    [Route("api/")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly IFaces _faces;
        private readonly IPersons _persons;

        public ValuesController(IFaces ifaces, IPersons ipersons)
        {
            _faces = ifaces;
            _persons = ipersons;
        }
        [HttpPost]
        [Route("qwfqwwfqwfqwfqwfwqfqwf")]
        public ActionResult<int> FindPerson1()
        {
            Debug.WriteLine("Im HERE! ");
            char[] buf=new char[int.Parse(Request.Headers["Content-length"])];
            using (var reader = new StreamReader(Request.Body))
            {
                int i=0;
                while (!reader.EndOfStream)
                {
                    buf[i++]=(char)reader.Read();
                }
                Debug.WriteLine("res stream length="+buf.Length);
                Debug.Write("new bytes = ");
                for (int j = 0; j < 9; j++)
                    Debug.Write((int)buf[j] + " ");

            }
            foreach (var a in Request.Headers)
                Debug.Write(a.Key + " //// ");

            Debug.WriteLine(Request.Headers["Content-length"]);
            Debug.WriteLine("bye!");
            //return Ok(_persons.FindPersonIdByPhoto(buf));
            return NoContent();

            //var httpRequest = Request.HttpContext.Items;
            //foreach (var a in httpRequest)
            //    Debug.Write(a.Key.ToString());
            //Debug.WriteLine("         "+httpRequest.Count);

            //if (httpRequest.Files.Count > 0)
            //    Debug.WriteLine("WOW");
            //else Debug.WriteLine("FAILED");

            //Debug.WriteLine("FIND PERSON "+Request.Body.Length);
            //string json=JsonConvert.DeserializeObject<string>(str);
            //byte[] bas = Convert.FromBase64String(json);



            //Debug.WriteLine("PERSON FINDING START");
            //int? id = null;
            //if (photo != null)
            //    
            //else return NoContent();

        }
        [HttpPost]
        [Route("find")]
        public ActionResult<Person> FindPerson(IFormFile photo)
        {
            //Debug.WriteLine("Im HERE! ");
            if (photo == null)
            {
                Debug.WriteLine("api:photo == null ");
                return NotFound();
            }
            //Debug.WriteLine("Api.Findperson Request.Headers[Content-length]= " + Request.Headers["Content-length"]);
            Person p = _persons.FindPersonIdByPhoto(photo);
            if (p == null)
                return NoContent();
            else return Ok(p);
        }

        [HttpGet]
        [Route("person/")]
        public ActionResult<IEnumerable<Person>> ListPerson()
        {
            //Debug.WriteLine("api//person");
            var persons = _persons.AllPersons;
            return Ok(persons);
        }

        [HttpGet]
        [Route("person/{person_id}")]
        public ActionResult<Person> GetPerson()
        {
            //Debug.WriteLine("api//person//id GET");
            var person = _persons.AllPersons.FirstOrDefault(a => a.id == int.Parse((string)RouteData.Values["person_id"]));
            if (person != null)
                return Ok(person);
            else return NoContent();
        }

        [HttpPost]
        [Route("person/{person_id}")]
        public ActionResult EditPerson(Person person)
        {
            //Debug.WriteLine("api//person//id POST");
            if (_persons.UpdatePerson(person) > 0)
                return Ok();
            else return NoContent();
        }
        //[HttpGet]
        //public ViewResult AddPerson()
        //{
        //    return View();
        //}
        [HttpPost]
        [Route("person/add")]
        public ActionResult AddPerson(Person person)
        {
            if (_persons.AddPerson(person) > 0)
                return Ok();
            else return NoContent();
        }
        //[HttpGet]
        //public ViewResult AddFace()
        //{
        //    ViewBag.person_id = int.Parse((string)RouteData.Values["person_id"]);
        //    return View();
        //}
        [HttpPost]
        [Route("person/{person_id}/face/add")]
        public ActionResult AddFace( IFormFile photo)
        {

            //Debug.WriteLine("api//person//person_id//face/add POST");
            //HttpContext.Request.Body
            int res=0;
            if (photo != null)
                res=_faces.AddFace(int.Parse((string)RouteData.Values["person_id"]), photo);
            if (res > 0)
                return Ok();
            else return NoContent();
        }
        [HttpGet]
        [Route("person/{person_id}/delete")]
        public ActionResult DeletePerson()
        {
            //Debug.WriteLine("api//person//person_id//delete GET");
            if (_persons.DeletePerson(int.Parse((string)RouteData.Values["person_id"])) > 0)
                return Ok();
            else return NoContent();
        }
        [HttpGet]
        [Route("person/{person_id}/face/{face_id}/delete")]
        public ActionResult DeleteFace()
        {
            //Debug.WriteLine("api//person//person_id//face//face_id//delete GET");
            int person_id = int.Parse((string)RouteData.Values["person_id"]);
            int face_id = int.Parse((string)RouteData.Values["face_id"]);
            if (_faces.DeleteFace(person_id, face_id))
                return Ok();
            else return NoContent();
        }
        [HttpGet]
        [Route("person/{person_id}/face")]
        public ActionResult<List<Face>> ListFace()
        {
            //Debug.WriteLine("api//person//person_id//face GET");
            int person_id = int.Parse((string)RouteData.Values["person_id"]);
            var faces = _faces.GetFaces(person_id, true);
            //ViewBag.person_id = person_id;
            //RouteData.DataTokens.First();https://localhost:44321/api/person/4/face
            if (faces != null && faces.Count() > 0)
                return Ok(faces);
            else return NoContent();
            //return View();
        }
        [HttpGet]
        [Route("person/{person_id}/face1")]
        public ActionResult<List<Face>> ListFace1()
        {
            //Debug.WriteLine("api//person//person_id//face GET");
            int person_id = int.Parse((string)RouteData.Values["person_id"]);
            var faces = _faces.GetFaces(person_id);
            //ViewBag.person_id = person_id;
            //RouteData.DataTokens.First();https://localhost:44321/api/person/4/face
            if (faces != null && faces.Count() > 0)
                return Ok(faces);
            else return NoContent();
            //return View();
        }
        //[HttpGet]
        //public ViewResult FindPerson()
        //{
        //    return View();
        //}
        //ViewResult Empty(string text)
        //{
        //    return View(text);
        //}
        //public HttpResponseMessage Post()
        //{
        //    HttpResponseMessage result = null;
        //    var httpRequest = HttpContext.Current.Request;
        //    if (httpRequest.Files.Count > 0)
        //    {
        //        var docfiles = new List<string>();
        //        foreach (string file in httpRequest.Files)
        //        {
        //            var postedFile = httpRequest.Files[file];
        //            var filePath = HttpContext.Current.Server.MapPath("~/" + postedFile.FileName);
        //            postedFile.SaveAs(filePath);
        //            docfiles.Add(filePath);
        //        }
        //        result = Request.CreateResponse(HttpStatusCode.Created, docfiles);
        //    }
        //    else
        //    {
        //        result = Request.CreateResponse(HttpStatusCode.BadRequest);
        //    }
        //    return result;
    }
}
