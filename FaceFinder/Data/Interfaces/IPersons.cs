using FaceFinder.Data.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FaceFinder.Data.Interfaces
{
    public interface IPersons
    {
        IEnumerable<Person> AllPersons { get; }
        int AddPerson(Person person);
        int UpdatePerson(Person person);
        int DeletePerson(int id);
        Person FindPersonIdByPhoto(IFormFile photo);
    }
}
