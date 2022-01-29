using System;
using System.Collections.Generic;
using Reports.Core.Entities;

namespace Reports.Core.Services
{
    public interface IPersonService
    {
        void DeletePerson(Guid id);
        Person GetPersonById(Guid id);
        IEnumerable<Person> GetPersonsList();
        void EditPerson(Guid personId, string newName, string newSurname);
    }
}