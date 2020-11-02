using FaceFinder.Data.Interfaces;
using FaceFinder.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Npgsql;
using FaceFinder;
using System.Configuration;
using System.Collections.Specialized;
using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using System.Security.Cryptography;
using System.IO;
using System.Text;

namespace FaceFinder.Data.Mocks
{
    public class MockPerson : IPersons
    {
        public IEnumerable<Person> AllPersons
        {
            get
            {
                //return new List<Person>

                //ConfigurationManager.AppSettings[""
                List<Person> persons = new List<Person>();
                using (var sConn = new NpgsqlConnection(ConfigurationManager.ConnectionStrings["default"].ConnectionString))
                {
                    string queryString = "select id,surname,name,fathname from persons order by id";
                    var command = new NpgsqlCommand(queryString);
                    command.Connection = sConn;
                    sConn.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            persons.Add(new Person 
                            { 
                                id = (int)reader["id"],
                                surname = reader["surname"].ToString(), 
                                name = reader["name"].ToString(), 
                                fathname = reader["fathname"].ToString() 
                            });
                        }
                    }
                }
                
                return persons;
            }
        }
        public int AddPerson(Person person)
        {
            using (var sConn = new NpgsqlConnection(ConfigurationManager.ConnectionStrings["default"].ConnectionString))
            {
                string queryString = "insert into persons(surname,name,fathname) values (@surname,@name,@fathname)";
                var command = new NpgsqlCommand(queryString);
                command.Parameters.AddWithValue("surname", person.surname);
                command.Parameters.AddWithValue("name", person.name);
                command.Parameters.AddWithValue("fathname", person.fathname);
                command.Connection = sConn;
                sConn.Open();
                return command.ExecuteNonQuery();
            }
        }
        public int UpdatePerson(Person person)
        {
            using (var sConn = new NpgsqlConnection(ConfigurationManager.ConnectionStrings["default"].ConnectionString))
            {
                string queryString = "update persons set surname=@surname,name=@name,fathname=@fathname where id=@id";
                var command = new NpgsqlCommand(queryString);
                command.Parameters.AddWithValue("surname", person.surname);
                command.Parameters.AddWithValue("name", person.name);
                command.Parameters.AddWithValue("fathname", person.fathname);
                command.Parameters.AddWithValue("id", person.id);
                command.Connection = sConn;
                sConn.Open();
                return command.ExecuteNonQuery();
            }
        }
        public int DeletePerson(int id)
        {
            using (var sConn = new NpgsqlConnection(ConfigurationManager.ConnectionStrings["default"].ConnectionString))
            {
                string queryString = "delete from persons where id=@id";
                var command = new NpgsqlCommand(queryString);
                command.Parameters.AddWithValue("id", id);
                command.Connection = sConn;
                sConn.Open();
                return command.ExecuteNonQuery();
            }
        }

        public Person FindPersonIdByPhoto(IFormFile photo)
        {
            Debug.WriteLine("FindPersonById");

            if (photo != null)
            {
                Debug.WriteLine("Findperson. Photo!=null");
                byte[] hash;
                //byte[] buf1 = null;
                using (SHA256 sha256Hash = SHA256.Create())
                {
                    hash = sha256Hash.ComputeHash(photo.OpenReadStream());
                    //Debug.WriteLine("MockHash=" + Encoding.UTF8.GetString(hash));
                }
                int? id;
                using (var sConn = new NpgsqlConnection(ConfigurationManager.ConnectionStrings["default"].ConnectionString))
                {
                    string queryString = "select id_person as id,surname,name,fathname from faces join persons on id_person=persons.id where hash=@hash";
                    var command = new NpgsqlCommand(queryString);
                    command.Parameters.AddWithValue("hash", hash);
                    command.Connection = sConn;
                    sConn.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (!reader.Read())
                            return null;
                        return new Person
                        {
                            id = (int)reader["id"],
                            surname = reader["surname"].ToString(),
                            name = reader["name"].ToString(),
                            fathname = reader["fathname"].ToString()
                        };
                    }
                }
            }
            else
            {
                Debug.WriteLine("Findperson. Photo==null");
                return null;
            } 
        }
    }
}
