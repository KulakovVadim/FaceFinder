using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FaceFinder.Data.Models
{
    public class Person
    {
        public int id { get; set; }
        public string surname { get; set; }
        public string name { get; set; }
        public string fathname { get; set; }
        List<Face> faces { get; set; }
    }
}
