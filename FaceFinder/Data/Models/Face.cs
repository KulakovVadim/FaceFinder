using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FaceFinder.Data.Models
{
    public class Face
    {
        public int id { get; set; }
        public int id_person { get; set; }
        public string fileaddress { get; set; }
        public string base64string { get; set; }//сериализованная в string картинка
    }
}