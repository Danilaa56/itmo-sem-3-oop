using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Reports.Core.Services;
using Reports.WebAPI.Models;

namespace Reports.WebAPI.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class WorkerController : ControllerBase
    {
        private readonly IPersonService _personService;

        public WorkerController(IPersonService personService)
        {
            _personService = personService;
        }

        [HttpGet]
        public IEnumerable<PersonModel> List()
        {
            return _personService.GetPersonsList().Select(PersonModel.ToModel);
        }

        [HttpGet("{id}")]
        public PersonModel GetById(Guid id)
        {
            return PersonModel.ToModel(_personService.GetPersonById(id));
        }

        [HttpGet("{directorId}/getWorkers")]
        public IEnumerable<PersonModel> GetWorkers(Guid directorId)
        {
            return _personService.GetWorkers(directorId).Select(PersonModel.ToModel);
        }

        [HttpPost]
        public Guid Create(string name, string surname)
        {
            return _personService.CreatePerson(name, surname);
        }

        [HttpPut("{id}")]
        public Guid Edit(Guid id, string name, string surname)
        {
            return _personService.CreatePerson(name, surname);
        }

        [HttpPut("{id}/setDirector")]
        public void SetDirector(Guid workerId, Guid directorId)
        {
            _personService.SetDirector(workerId, directorId);
        }

        [HttpDelete("{id}")]
        public void Delete(Guid id)
        {
            _personService.DeletePerson(id);
        }
    }
}