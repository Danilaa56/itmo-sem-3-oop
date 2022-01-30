using System;
using System.Collections.Generic;
using Reports.Core.Entities;

namespace Reports.Core.Services
{
    public interface IPersonService
    {
        Guid CreatePerson(string name, string surname);
        void DeletePerson(Guid id);
        Person GetPersonById(Guid id);
        IEnumerable<Person> GetPersonsList();
        void EditPerson(Guid personId, string newName, string newSurname);
        void SetDirector(Guid personId, Guid directorId);
        IEnumerable<Person> GetWorkers(Guid directorId);
    }
}