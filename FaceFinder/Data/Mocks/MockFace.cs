using FaceFinder.Data.Interfaces;
using FaceFinder.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Configuration;
using System.Collections.Specialized;
using Npgsql;
using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using System.Security.Cryptography;
using Newtonsoft.Json;
using System.Security.Policy;

namespace FaceFinder.Data.Mocks
{
    public class MockFace : IFaces
    {
        private readonly IPersons _person = new MockPerson();
        
        public IEnumerable<Face> Faces 
        { 
            get
            {
                //return new List<Person>

                //ConfigurationManager.AppSettings[""
                List<Face> faces = new List<Face>();
                using (var sConn = new NpgsqlConnection(ConfigurationManager.ConnectionStrings["default"].ConnectionString))
                {
                    string queryString = "select id,id_person,filename from persons order by id";
                    var command = new NpgsqlCommand(queryString);
                    command.Connection = sConn;
                    sConn.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            faces.Add(new Face
                            {
                                id = (int)reader["id"],
                                id_person = (int)reader["id_person"],
                                fileaddress = "~/photos/"+ reader["id_person"].ToString() +"_"+ reader["id"].ToString()+ "_" + reader["filename"].ToString()
                            });
                        }
                    }
                }

                return faces;
            }
            //set; 
        }

        public int AddFace(int id_person,IFormFile photo)
        {
            if (photo != null)
            {
                byte[] hash;
                using (SHA256 sha256Hash = SHA256.Create())
                {
                    hash = sha256Hash.ComputeHash(photo.OpenReadStream());

                }
                bool cancreate = false;// true, если личность с таким id существует, а лицо с таким hash отсутствует
                using (var sConn = new NpgsqlConnection(ConfigurationManager.ConnectionStrings["default"].ConnectionString))
                {
                    string queryString = "select (personexists=1 and imageexists=0)as cancreate from (select count(*) as personexists from persons where id=@id_person)as q1 ,(select count(*) as imageexists from faces where hash = @hash) as q2";
                    var command = new NpgsqlCommand(queryString);
                    command.Parameters.AddWithValue("id_person", id_person);
                    command.Parameters.AddWithValue("hash", hash);
                    command.Connection = sConn;
                    sConn.Open();
                    cancreate = (bool)command.ExecuteScalar();
                }
                if (cancreate)
                {
                    int id;
                    using (var sConn = new NpgsqlConnection(ConfigurationManager.ConnectionStrings["default"].ConnectionString))
                    {
                        string queryString = "insert into faces(id_person,filename,hash) values (@id_person,@filename,@hash) returning id";
                        var command = new NpgsqlCommand(queryString);
                        command.Parameters.AddWithValue("id_person", id_person);
                        command.Parameters.AddWithValue("filename", photo.FileName);
                        command.Parameters.AddWithValue("hash", hash);
                        command.Connection = sConn;
                        sConn.Open();
                        id = (int)command.ExecuteScalar();
                    }
                    string path = Directory.GetCurrentDirectory() + "//wwwroot//photos//" + id_person + "_" + id + "_" + photo.FileName;
                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        photo.CopyTo(fileStream);

                    }
                    return id;
                }
                else
                {
                    Debug.WriteLine("!canreate");
                    return 0;
                }
            }
            else
            {
                Debug.WriteLine("Mockface. photo==null");

                return 0; 
            }
        }

        public bool DeleteFace(int person_id,int id)
        {
            string filename="";
            using (var sConn = new NpgsqlConnection(ConfigurationManager.ConnectionStrings["default"].ConnectionString))
            {
                string queryString = "delete from faces where id=@id returning filename";
                var command = new NpgsqlCommand(queryString);
                command.Parameters.AddWithValue("id", id);
                command.Connection = sConn;
                sConn.Open();
                filename = command.ExecuteScalar().ToString();
            }
            if (filename != "")
            {
                string path = Directory.GetCurrentDirectory() + "//wwwroot//photos//" + person_id + "_" + id + "_" + filename;
                File.Delete(path);
                return true;
            }
            else return false;
        }

        public IEnumerable<Face> GetFaces(int personId, bool withdata=false)
        {
            List<Face> faces = new List<Face>();
            using (var sConn = new NpgsqlConnection(ConfigurationManager.ConnectionStrings["default"].ConnectionString))
            {
                string queryString = "select id,id_person,filename from faces where id_person=@id_person order by id";
                var command = new NpgsqlCommand(queryString);
                command.Parameters.AddWithValue("id_person", personId);
                command.Connection = sConn;
                sConn.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        faces.Add(new Face
                        {
                            id = (int)reader["id"],
                            id_person = (int)reader["id_person"],
                            fileaddress = "/photos/" + reader["id_person"].ToString() + "_" + reader["id"].ToString() + "_" + reader["filename"].ToString()
                        });
                    }
                }
            }
            if (withdata)
            {
                foreach (Face face in faces)
                {
                    
                    byte[] imageArray = System.IO.File.ReadAllBytes(@"D:\Trash\projects\FaceFinder\FaceFinder\wwwroot\"+face.fileaddress);
                    face.base64string = Convert.ToBase64String(imageArray);
                    
                }
            }
            return faces;

        }
    }
}
