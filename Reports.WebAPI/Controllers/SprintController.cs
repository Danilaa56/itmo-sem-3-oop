using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Reports.Core.Entities;
using Reports.Core.Services;

namespace Reports.WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SprintController : ControllerBase
    {
        private readonly ISprintService _sprintService;

        public SprintController(ISprintService sprintService)
        {
            _sprintService = sprintService;
        }

        [HttpGet]
        public IEnumerable<Sprint> List()
        {
            return _sprintService.GetSprintsList();
        }

        [HttpGet("{id}")]
        public Sprint GetById(Guid id)
        {
            return _sprintService.GetSprintById(id);
        }

        [HttpPost]
        public Guid GetById(string name, DateTime start, DateTime finish)
        {
            return _sprintService.CreateSprint(name, start, finish);
        }
    }
}