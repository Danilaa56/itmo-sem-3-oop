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
    public class ProblemController : ControllerBase
    {
        private readonly IProblemService _problemService;

        public ProblemController(IProblemService problemService)
        {
            _problemService = problemService;
        }

        [HttpGet]
        public IEnumerable<ProblemModel> List()
        {
            return _problemService.GetProblemsList().Select(ProblemModel.ToModel);
        }

        [HttpGet("{id}")]
        public ProblemModel GetById(Guid id)
        {
            return ProblemModel.ToModel(_problemService.GetProblemById(id));
        }

        [HttpPost]
        public void CreateProblem(string title, string content, Guid sprintId, Guid authorId)
        {
            _problemService.CreateProblem(title, content, authorId, sprintId);
        }

        [HttpPut("{id}")]
        public void Edit(Guid id, string title, string content, Guid sprintId)
        {
            _problemService.EditProblem(id, title, content, sprintId);
        }

        [HttpPut("{id}/setState")]
        public void SetState(Guid id, Problem.ProblemState state)
        {
            _problemService.SetState(id, state);
        }

        [HttpPost("{id}/addComment")]
        public void AddComment(Guid id, Guid authorId, string content)
        {
            _problemService.WriteComment(id, authorId, content);
        }
    }
}