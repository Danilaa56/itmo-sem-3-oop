using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Reports.Core.Entities;
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

        [HttpPost("/createTeamLeader")]
        public Guid CreateTeamLeader(string name, string surname)
        {
            return _personService.CreatePersonTeamLeader(name, surname);
        }

        [HttpPost]
        public Guid Create(string name, string surname, Guid directorId)
        {
            return _personService.CreatePerson(name, surname, directorId);
        }

        [HttpPut("{id}")]
        public void Edit(Guid id, string name, string surname)
        {
            _personService.EditPerson(id, name, surname);
        }

        [HttpPut("{id}/setDirector")]
        public void SetDirector(Guid workerId, Guid directorId)
        {
            _personService.SetDirector(workerId, directorId);
        }

        [HttpGet("getHierarchy")]
        public HierarchyElement GetHierarchy()
        {
            Dictionary<Guid, HierarchyElement> personsMap = new Dictionary<Guid, HierarchyElement>();
            List<Person> persons = _personService.GetPersonsList().ToList();

            foreach (Person person in persons)
            {
                personsMap[person.Id] = new HierarchyElement
                {
                    Id = person.Id,
                    Name = person.Name,
                    Surname = person.Surname,
                    Workers = new List<HierarchyElement>()
                };
            }

            HierarchyElement? teamLead = null;
            foreach (Person person in persons)
            {
                if (person.Director is null)
                {
                    teamLead = personsMap[person.Id];
                }
                else
                {
                    personsMap[person.Director.Id].Workers.Add(personsMap[person.Id]);
                }
            }

            return teamLead!;
        }
    }
}