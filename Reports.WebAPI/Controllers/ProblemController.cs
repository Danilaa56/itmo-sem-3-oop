using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Reports.Core.Entities;
using Reports.Core.Services;

namespace Reports.WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProblemController
    {
        private readonly IProblemService _problemService;

        public ProblemController(IProblemService problemService)
        {
            _problemService = problemService;
        }

        [HttpGet(Name = "GetProblemsList")]
        public IEnumerable<Problem> Get()
        {
            return _problemService.GetProblemsList();
        }
    }
}