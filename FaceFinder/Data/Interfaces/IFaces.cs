using FaceFinder.Data.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FaceFinder.Data.Interfaces
{
    public interface IFaces
    {
        IEnumerable<Face> Faces{ get;/*set ;*/}
        IEnumerable<Face> GetFaces(int personId, bool withdata = false);
        int AddFace(int person_id, IFormFile photo);
        bool DeleteFace(int person_id,int id);

    }
}
